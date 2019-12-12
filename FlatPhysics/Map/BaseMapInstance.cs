using System.Collections.Generic;
using System.Numerics;

namespace FlatPhysics.Map
{
    public abstract class BaseMapInstance<TMapObject> where TMapObject : class, IGuidable
    {
        private readonly IDictionary<ulong, BaseMapTile<TMapObject>> m_tiles = new Dictionary<ulong, BaseMapTile<TMapObject>>();
        private readonly IDictionary<ulong, BaseMapTile<TMapObject>> m_toCheckRemove = new Dictionary<ulong, BaseMapTile<TMapObject>>();
        private readonly int m_tileSize;
        private readonly int m_halfTileSize;

        public static ulong GetTileId(float x, float y, float tileSize)
        {
            return GetTileId(Mathf.Floor(x / tileSize), Mathf.Floor(y / tileSize));
        }

        [System.Obsolete]
        public static ulong SafeGetTileId(int x, int y)
        {
            byte[] xBytes = System.BitConverter.GetBytes(x);
            byte[] yBytes = System.BitConverter.GetBytes(y);

            byte[] resultBytes = new byte[8];
            System.Buffer.BlockCopy(xBytes, 0, resultBytes, 0, 4);
            System.Buffer.BlockCopy(yBytes, 0, resultBytes, 4, 4);

            return System.BitConverter.ToUInt64(resultBytes, 0);
        }

        public static ulong GetTileId(int x, int y)
        {
            return (ulong)(uint)y << 32 | (ulong)(uint)x;
        }

        public int TileSize
        {
            get { return m_tileSize; }
        }

        protected BaseMapInstance(int tileSize)
        {
            m_tileSize = tileSize;
            m_halfTileSize = tileSize / 2;
        }

        protected abstract BaseMapTile<TMapObject> GetObjectTile(TMapObject obj);
        protected abstract BaseMapTile<TMapObject> GetObjectTileByPosition(TMapObject obj);
        protected abstract void SetObjectTile(TMapObject obj, BaseMapTile<TMapObject> tile);

        public ICollection<BaseMapTile<TMapObject>> GetAllTiles()
        {
            return m_tiles.Values;
        }

        protected virtual BaseMapTile<TMapObject> CreateTile(int x, int y)
        {
            return new BaseMapTile<TMapObject>(this, x, y);
        }

        public void Enter(TMapObject objectBase)
        {
            BaseMapTile<TMapObject> oldTile = GetObjectTile(objectBase);

            BaseMapTile<TMapObject> mapTile = GetObjectTileByPosition(objectBase);

            if (oldTile == mapTile)
            {
                mapTile.Move(objectBase);
                return; // already entered
            }

            if (oldTile != null)
                oldTile.Leave(objectBase);

            SetObjectTile(objectBase, mapTile);

            mapTile.Enter(objectBase);
        }

        public void Leave(TMapObject objectBase)
        {
            BaseMapTile<TMapObject> oldTile = GetObjectTile(objectBase);

            if (oldTile != null)
                oldTile.Leave(objectBase);

            SetObjectTile(objectBase, null);
        }

        #region Tile getters

        public BaseMapTile<TMapObject> GetTile(int tilex, int tiley, bool autoCreate)
        {
            ulong id = GetTileId(tilex, tiley);

            BaseMapTile<TMapObject> result;
            if (m_tiles.TryGetValue(id, out result))
                return result;

            if (autoCreate)
                m_tiles[id] = result = CreateTile(tilex, tiley);

            return result;
        }

        /*public BaseMapTile<TMapObject> GetTileByPosition(Vector3 position, bool autoCreate)
		{
			return GetTileByPosition(position.X, position.Y, autoCreate);
		}
*/
        public BaseMapTile<TMapObject> GetTileByPosition(float x, float y, bool autoCreate)
        {
            int tilex = Mathf.Floor(x / m_tileSize);
            int tiley = Mathf.Floor(y / m_tileSize);

            ulong id = GetTileId(tilex, tiley);

            BaseMapTile<TMapObject> result;
            if (m_tiles.TryGetValue(id, out result))
                return result;

            if (autoCreate)
                m_tiles[id] = result = CreateTile(tilex, tiley);

            return result;
        }

        public ICollection<BaseMapTile<TMapObject>> GetTiles(float vfromX, float vfromY, float vtoX, float vtoY) // box
        {
            int fromX = Mathf.Floor(Mathf.Min(vfromX, vtoX) / m_tileSize);
            int fromY = Mathf.Floor(Mathf.Min(vfromY, vtoY) / m_tileSize);
            int toX = Mathf.Floor(Mathf.Max(vfromX, vtoX) / m_tileSize);
            int toY = Mathf.Floor(Mathf.Max(vfromY, vtoY) / m_tileSize);

            if (fromX == toX && fromY == toY)
            {
                BaseMapTile<TMapObject> tile = GetTile(fromX, fromY, true);
                if (tile == null)
                    return new BaseMapTile<TMapObject>[0];
                return new[] { tile };
            }

            List<BaseMapTile<TMapObject>> result = new List<BaseMapTile<TMapObject>>((toX - fromX + 1) * (toY - fromY + 1));

            for (int tileX = fromX; tileX <= toX; tileX++)
                for (int tileY = fromY; tileY <= toY; tileY++)
                {
                    BaseMapTile<TMapObject> tile = GetTile(tileX, tileY, true);
                    if (tile != null)
                        result.Add(tile);
                }

            return result;
        }

        public ICollection<BaseMapTile<TMapObject>> GetTiles(float x, float y, float radius, bool autoCreate) // circle
        {
            int fromX = Mathf.Floor((x - radius) / m_tileSize);
            int fromY = Mathf.Floor((y - radius) / m_tileSize);
            int toX = Mathf.Floor((x + radius) / m_tileSize);
            int toY = Mathf.Floor((y + radius) / m_tileSize);

            if (fromX == toX && fromY == toY)
            {
                BaseMapTile<TMapObject> tile = GetTile(fromX, fromY, autoCreate);
                if (tile == null)
                    return new BaseMapTile<TMapObject>[0];
                return new BaseMapTile<TMapObject>[] { tile };
            }

            float radiusSq = radius * radius;
            List<BaseMapTile<TMapObject>> result = new List<BaseMapTile<TMapObject>>();

            for (int tileX = fromX; tileX <= toX; tileX++)
                for (int tileY = fromY; tileY <= toY; tileY++)
                {
                    float left = tileX * m_tileSize;
                    float top = tileY * m_tileSize;
                    float right = left + m_tileSize;
                    float bottom = top + m_tileSize;


                    float ix = x < left ? left : x > right ? right : x;
                    float iy = y < top ? top : y > bottom ? bottom : y;

                    if ((x - ix) * (x - ix) + (y - iy) * (y - iy) <= radiusSq)
                    {
                        BaseMapTile<TMapObject> tile = GetTile(tileX, tileY, autoCreate);
                        if (tile != null)
                        {
                            result.Add(tile);
                        }
                    }
                }

            return result;
        }

        public TMapObject GetObject(BaseMapTile<TMapObject> sourceTile, ulong id)
        {
            foreach (BaseMapTile<TMapObject> tile in GetNearestTiles(sourceTile, false))
                if (tile != null)
                {
                    TMapObject obj = tile.GetObject(id);
                    if (obj != null)
                        return obj;
                }
            return default(TMapObject);
        }



        /*
		public ICollection<Pair<int, int>> GetTileIndexes(Vector2 from, Vector2 to) // box
		{
			int nfromX = Mathf.Floor(from.X / m_tileSize);
			int nfromY = Mathf.Floor(from.Y / m_tileSize);
			int ntoX = Mathf.Ceiling(to.X / m_tileSize);
			int ntoY = Mathf.Ceiling(to.Y / m_tileSize);

			if (nfromX == ntoX && nfromY == ntoY)
			{
				BaseMapTile tile = GetTile(nfromX, nfromY, true);
				if (tile == null)
					return new Pair<int, int>[0];
				return new Pair<int, int>[] { new Pair<int, int>(tile.X, tile.Y) };
			}

			int fromX = Mathf.Min(nfromX, ntoX);
			int fromY = Mathf.Min(nfromY, ntoY);
			int toX = Mathf.Max(nfromX, ntoX);
			int toY = Mathf.Max(nfromY, ntoY);

			List<Pair<int, int>> result = new List<Pair<int, int>>((toX - fromX + 1) * (toY - fromY + 1));

			for (int tileX = fromX; tileX <= toX; tileX++)
				for (int tileY = fromY; tileY <= toY; tileY++)
				{
					result.Add(new Pair<int, int>(tileX, tileY));
				}

			return result;
		}*/

        /// <summary>
        /// Gets 4 nearest tiles from this point, including current tile
        /// </summary>
        /// <param name="point"></param>
        /// <param name="autoCreate"></param>
        /// <returns></returns>
        public ICollection<BaseMapTile<TMapObject>> GetNearestTiles(float pointX, float pointY, bool autoCreate)
        {
            List<BaseMapTile<TMapObject>> result = new List<BaseMapTile<TMapObject>>(4);

            int x = Mathf.Floor(pointX / m_tileSize);
            int y = Mathf.Floor(pointY / m_tileSize);

            int shiftX = (pointX - x * m_tileSize) > m_halfTileSize ? 1 : -1;
            int shiftY = (pointY - y * m_tileSize) > m_halfTileSize ? 1 : -1;

            {
                BaseMapTile<TMapObject> tile = GetTile(x, y, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile<TMapObject> tile = GetTile(x + shiftX, y, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile<TMapObject> tile = GetTile(x + shiftX, y + shiftY, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile<TMapObject> tile = GetTile(x, y + shiftY, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }

            return result;
        }

        /// <summary>
        /// Gets all adjacent tiles, including requesting tile
        /// </summary>
        /// <param name="sourceTile"></param>
        /// <param name="autoCreate"></param>
        /// <returns></returns>
        public ICollection<BaseMapTile<TMapObject>> GetNearestTiles(BaseMapTile<TMapObject> sourceTile, bool autoCreate)
        {
            List<BaseMapTile<TMapObject>> result = new List<BaseMapTile<TMapObject>>(9);
            int x = sourceTile.X;
            int y = sourceTile.Y;
            result.Add(sourceTile);
            {
                BaseMapTile<TMapObject> tile = GetTile(x + 1, y, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile<TMapObject> tile = GetTile(x - 1, y, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile<TMapObject> tile = GetTile(x + 1, y + 1, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile<TMapObject> tile = GetTile(x - 1, y + 1, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile<TMapObject> tile = GetTile(x + 1, y - 1, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile<TMapObject> tile = GetTile(x - 1, y - 1, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile<TMapObject> tile = GetTile(x, y + 1, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile<TMapObject> tile = GetTile(x, y - 1, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }

            return result;
        }
        #endregion

        public virtual void Update()
        {
            foreach (var tile in m_tiles.Values)
            {
                if (tile.IsDirty)
                    tile.Update();

                if (tile.IsEmpty)
                {
                    ICollection<BaseMapTile<TMapObject>> nearest = GetNearestTiles(tile, false);

                    bool empty = true;

                    foreach (BaseMapTile<TMapObject> nearestTile in nearest)
                        if (!nearestTile.IsEmpty)
                        {
                            empty = false;
                            break;
                        }

                    if (empty)
                    {
                        m_toCheckRemove[tile.Id] = tile;
                    }
                }
            }

            if (m_toCheckRemove.Count > 0)
            {
                foreach (var tile in m_toCheckRemove.Values)
                {
                    tile.Clean();
                    m_tiles.Remove(tile.Id);
                }
                m_toCheckRemove.Clear();
            }
        }
    }
}


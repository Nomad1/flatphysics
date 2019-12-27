using System.Collections.Generic;
using System.Numerics;

namespace FlatPhysics.Map
{
    public abstract class Base3dMapInstance
    {
        public const int DefaultTileExpireTime = 30000;

        private readonly IDictionary<ulong, Base3dMapTile> m_tiles = new Dictionary<ulong, Base3dMapTile>();
        private readonly IDictionary<ulong, Base3dMapTile> m_toCheckRemove = new Dictionary<ulong, Base3dMapTile>();
        private readonly int m_tileSize;
        private readonly int m_halfTileSize;
        private readonly int m_tileExpireTime;

        public static ulong GetTileId(float x, float y, float z, float tileSize)
        {
            return GetTileId((short)Mathf.Floor(x / tileSize), (short)Mathf.Floor(y / tileSize), (short)Mathf.Floor(z / tileSize));
        }

        public static ulong GetTileId(short x, short y, short z)
        {
            return (ulong)(ushort)y << 32 | (ulong)(ushort)x << 16 | (ulong)(ushort)z;
        }

        public int TileSize
        {
            get { return m_tileSize; }
        }

        public int TileExpireTime
        {
            get { return m_tileExpireTime; }
        }

        protected Base3dMapInstance(int tileSize, int tileExpireTime = DefaultTileExpireTime)
        {
            m_tileSize = tileSize;
            m_halfTileSize = tileSize / 2;
            m_tileExpireTime = tileExpireTime;
        }

        public ICollection<Base3dMapTile> GetAllTiles()
        {
            return m_tiles.Values;
        }

        protected virtual Base3dMapTile CreateTile(short x, short y, short z)
        {
            return new Base3dMapTile(this, x, y, z);
        }

        public void Enter(IBaseMapObject objectBase)
        {
            Base3dMapTile oldTile = objectBase.MapTile;

            Base3dMapTile mapTile = GetTileByPosition(objectBase.Position.X, objectBase.Position.Y, true);

            if (oldTile == mapTile)
            {
                mapTile.Move(objectBase);
                return; // already entered
            }

            if (oldTile != null)
                oldTile.Leave(objectBase);

            objectBase.MapTile = mapTile;

            mapTile.Enter(objectBase);
        }

        public void Leave(IBaseMapObject objectBase)
        {
            Base3dMapTile oldTile = objectBase.MapTile;

            if (oldTile != null)
                oldTile.Leave(objectBase);

            objectBase.MapTile = null;
        }

        #region Tile getters

        public BaseMapTile GetTile(int tilex, int tiley, bool autoCreate)
        {
            ulong id = GetTileId(tilex, tiley);

            BaseMapTile result;
            if (m_tiles.TryGetValue(id, out result))
                return result;

            if (autoCreate)
                m_tiles[id] = result = CreateTile(tilex, tiley);

            return result;
        }

        public BaseMapTile GetTileByPosition(float x, float y, bool autoCreate)
        {
            int tilex = Mathf.Floor(x / m_tileSize);
            int tiley = Mathf.Floor(y / m_tileSize);

            ulong id = GetTileId(tilex, tiley);

            BaseMapTile result;
            if (m_tiles.TryGetValue(id, out result))
                return result;

            if (autoCreate)
                m_tiles[id] = result = CreateTile(tilex, tiley);

            return result;
        }

        public ICollection<BaseMapTile> GetTiles(float vfromX, float vfromY, float vtoX, float vtoY) // box
        {
            int fromX = Mathf.Floor(Mathf.Min(vfromX, vtoX) / m_tileSize);
            int fromY = Mathf.Floor(Mathf.Min(vfromY, vtoY) / m_tileSize);
            int toX = Mathf.Floor(Mathf.Max(vfromX, vtoX) / m_tileSize);
            int toY = Mathf.Floor(Mathf.Max(vfromY, vtoY) / m_tileSize);

            if (fromX == toX && fromY == toY)
            {
                BaseMapTile tile = GetTile(fromX, fromY, true);
                if (tile == null)
                    return new BaseMapTile[0];
                return new[] { tile };
            }

            List<BaseMapTile> result = new List<BaseMapTile>((toX - fromX + 1) * (toY - fromY + 1));

            for (int tileX = fromX; tileX <= toX; tileX++)
                for (int tileY = fromY; tileY <= toY; tileY++)
                {
                    BaseMapTile tile = GetTile(tileX, tileY, true);
                    if (tile != null)
                        result.Add(tile);
                }

            return result;
        }

        public ICollection<BaseMapTile> GetTiles(float x, float y, float radius, bool autoCreate) // circle
        {
            int fromX = Mathf.Floor((x - radius) / m_tileSize);
            int fromY = Mathf.Floor((y - radius) / m_tileSize);
            int toX = Mathf.Floor((x + radius) / m_tileSize);
            int toY = Mathf.Floor((y + radius) / m_tileSize);

            if (fromX == toX && fromY == toY)
            {
                BaseMapTile tile = GetTile(fromX, fromY, autoCreate);
                if (tile == null)
                    return new BaseMapTile[0];
                return new BaseMapTile[] { tile };
            }

            float radiusSq = radius * radius;
            List<BaseMapTile> result = new List<BaseMapTile>();

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
                        BaseMapTile tile = GetTile(tileX, tileY, autoCreate);
                        if (tile != null)
                        {
                            result.Add(tile);
                        }
                    }
                }

            return result;
        }

        public IBaseMapObject GetObject(BaseMapTile sourceTile, ulong id)
        {
            foreach (BaseMapTile tile in GetNearestTiles(sourceTile, false))
                if (tile != null)
                {
                    IBaseMapObject obj = tile.GetObject(id);
                    if (obj != null)
                        return obj;
                }
            return null;
        }

        /// <summary>
        /// Gets 4 nearest tiles from this point, including current tile
        /// </summary>
        /// <param name="point"></param>
        /// <param name="autoCreate"></param>
        /// <returns></returns>
        public ICollection<BaseMapTile> GetNearestTiles(float pointX, float pointY, bool autoCreate)
        {
            List<BaseMapTile> result = new List<BaseMapTile>(4);

            int x = Mathf.Floor(pointX / m_tileSize);
            int y = Mathf.Floor(pointY / m_tileSize);

            int shiftX = (pointX - x * m_tileSize) > m_halfTileSize ? 1 : -1;
            int shiftY = (pointY - y * m_tileSize) > m_halfTileSize ? 1 : -1;

            {
                BaseMapTile tile = GetTile(x, y, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile tile = GetTile(x + shiftX, y, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile tile = GetTile(x + shiftX, y + shiftY, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile tile = GetTile(x, y + shiftY, autoCreate);
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
        public ICollection<BaseMapTile> GetNearestTiles(BaseMapTile sourceTile, bool autoCreate)
        {
            List<BaseMapTile> result = new List<BaseMapTile>(9);
            int x = sourceTile.X;
            int y = sourceTile.Y;
            result.Add(sourceTile);
            {
                BaseMapTile tile = GetTile(x + 1, y, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile tile = GetTile(x - 1, y, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile tile = GetTile(x + 1, y + 1, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile tile = GetTile(x - 1, y + 1, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile tile = GetTile(x + 1, y - 1, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile tile = GetTile(x - 1, y - 1, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile tile = GetTile(x, y + 1, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                BaseMapTile tile = GetTile(x, y - 1, autoCreate);
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
                    ICollection<BaseMapTile> nearest = GetNearestTiles(tile, false);

                    bool empty = true;

                    foreach (BaseMapTile nearestTile in nearest)
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


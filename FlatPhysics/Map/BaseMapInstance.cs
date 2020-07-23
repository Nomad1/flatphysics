using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FlatPhysics.Map
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct IdUnion
    {
        [FieldOffset(0)]
        private short X;
        [FieldOffset(2)]
        private short Y;
        [FieldOffset(4)]
        private ushort MapID;
        [FieldOffset(0)]
        public ulong ID;

        public IdUnion(uint mapId, int x, int y)
            : this()
        {
            MapID = (ushort)mapId;
            X = (short)x;
            Y = (short)y;
        }
    }

    public abstract class BaseMapInstance<TMapTile> where TMapTile : BaseMapTile<TMapTile>
    {
        public const int DefaultTileExpireTime = 30000;

        private readonly IDictionary<ulong, TMapTile> m_tiles = new Dictionary<ulong, TMapTile>();
        private readonly IDictionary<ulong, TMapTile> m_toCheckRemove = new Dictionary<ulong, TMapTile>();

        private readonly int m_tileSize;
        private readonly int m_halfTileSize;
        private readonly int m_tileExpireTime;
        private readonly uint m_mapId;

        public static ulong GetTileId(uint mapId, float x, float y, float tileSize)
        {
            return GetTileId(mapId, Mathf.Floor(x / tileSize), Mathf.Floor(y / tileSize));
        }

        public static ulong GetTileId(uint mapId, int x, int y)
        {
            return new IdUnion(mapId, x, y).ID;
        }

        public int TileSize
        {
            get { return m_tileSize; }
        }

        public int TileExpireTime
        {
            get { return m_tileExpireTime; }
        }

        public uint MapId
        {
            get { return m_mapId; }
        }

        protected BaseMapInstance(uint mapId, int tileSize, int tileExpireTime = DefaultTileExpireTime)
        {
            m_mapId = mapId;
            m_tileSize = tileSize;
            m_halfTileSize = tileSize / 2;
            m_tileExpireTime = tileExpireTime;
        }

        public ICollection<TMapTile> GetAllTiles()
        {
            return m_tiles.Values;
        }

        protected abstract TMapTile CreateTile(int x, int y);

        public void Enter(IBaseMapObject<TMapTile> objectBase)
        {
            TMapTile oldTile = objectBase.MapTile;

            TMapTile mapTile = GetTileByPosition(objectBase.Position.X, objectBase.Position.Y, true);

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

        public void Leave(IBaseMapObject<TMapTile> objectBase)
        {
            TMapTile oldTile = objectBase.MapTile;

            if (oldTile != null)
                oldTile.Leave(objectBase);

            objectBase.MapTile = null;
        }

        #region Tile getters

        protected TMapTile GetTile(int tilex, int tiley, bool autoCreate)
        {
            ulong id = GetTileId(m_mapId, tilex, tiley);

            TMapTile result;
            if (m_tiles.TryGetValue(id, out result))
                return result;

            if (autoCreate)
                m_tiles[id] = result = CreateTile(tilex, tiley);

            return result;
        }

        public TMapTile GetTileByPosition(float x, float y, bool autoCreate)
        {
            int tilex = Mathf.Floor(x / m_tileSize);
            int tiley = Mathf.Floor(y / m_tileSize);

            ulong id = GetTileId(m_mapId, tilex, tiley);

            TMapTile result;
            if (m_tiles.TryGetValue(id, out result))
                return result;

            if (autoCreate)
                m_tiles[id] = result = CreateTile(tilex, tiley);

            return result;
        }

        public ICollection<TMapTile> GetTiles(float vfromX, float vfromY, float vtoX, float vtoY) // box
        {
            int fromX = Mathf.Floor(Mathf.Min(vfromX, vtoX) / m_tileSize);
            int fromY = Mathf.Floor(Mathf.Min(vfromY, vtoY) / m_tileSize);
            int toX = Mathf.Floor(Mathf.Max(vfromX, vtoX) / m_tileSize);
            int toY = Mathf.Floor(Mathf.Max(vfromY, vtoY) / m_tileSize);

            if (fromX == toX && fromY == toY)
            {
                TMapTile tile = GetTile(fromX, fromY, true);
                if (tile == null)
                    return new TMapTile[0];
                return new[] { tile };
            }

            List<TMapTile> result = new List<TMapTile>((toX - fromX + 1) * (toY - fromY + 1));

            for (int tileX = fromX; tileX <= toX; tileX++)
                for (int tileY = fromY; tileY <= toY; tileY++)
                {
                    TMapTile tile = GetTile(tileX, tileY, true);
                    if (tile != null)
                        result.Add(tile);
                }

            return result;
        }

        public ICollection<TMapTile> GetTiles(float x, float y, float radius, bool autoCreate) // circle
        {
            int fromX = Mathf.Floor((x - radius) / m_tileSize);
            int fromY = Mathf.Floor((y - radius) / m_tileSize);
            int toX = Mathf.Floor((x + radius) / m_tileSize);
            int toY = Mathf.Floor((y + radius) / m_tileSize);

            if (fromX == toX && fromY == toY)
            {
                TMapTile tile = GetTile(fromX, fromY, autoCreate);
                if (tile == null)
                    return new TMapTile[0];
                return new TMapTile[] { tile };
            }

            float radiusSq = radius * radius;
            List<TMapTile> result = new List<TMapTile>((toX - fromX + 1) * (toY - fromY + 1));

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
                        TMapTile tile = GetTile(tileX, tileY, autoCreate);
                        if (tile != null)
                        {
                            result.Add(tile);
                        }
                    }
                }

            return result;
        }

        public IBaseMapObject<TMapTile> GetObject(TMapTile sourceTile, ulong id)
        {
            foreach (TMapTile tile in GetNearestTiles(sourceTile, false))
                if (tile != null)
                {
                    IBaseMapObject<TMapTile> obj = tile.GetObject(id);
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
        public ICollection<TMapTile> GetNearestTiles(float pointX, float pointY, bool autoCreate)
        {
            List<TMapTile> result = new List<TMapTile>(4);

            int x = Mathf.Floor(pointX / m_tileSize);
            int y = Mathf.Floor(pointY / m_tileSize);

            int shiftX = (pointX - x * m_tileSize) > m_halfTileSize ? 1 : -1;
            int shiftY = (pointY - y * m_tileSize) > m_halfTileSize ? 1 : -1;

            {
                TMapTile tile = GetTile(x, y, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                TMapTile tile = GetTile(x + shiftX, y, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                TMapTile tile = GetTile(x + shiftX, y + shiftY, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                TMapTile tile = GetTile(x, y + shiftY, autoCreate);
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
        public ICollection<TMapTile> GetNearestTiles(TMapTile sourceTile, bool autoCreate)
        {
            List<TMapTile> result = new List<TMapTile>(9);
            int x = sourceTile.X;
            int y = sourceTile.Y;
            result.Add(sourceTile);
            {
                TMapTile tile = GetTile(x + 1, y, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                TMapTile tile = GetTile(x - 1, y, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                TMapTile tile = GetTile(x + 1, y + 1, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                TMapTile tile = GetTile(x - 1, y + 1, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                TMapTile tile = GetTile(x + 1, y - 1, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                TMapTile tile = GetTile(x - 1, y - 1, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                TMapTile tile = GetTile(x, y + 1, autoCreate);
                if (tile != null)
                    result.Add(tile);
            }
            {
                TMapTile tile = GetTile(x, y - 1, autoCreate);
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
                    ICollection<TMapTile> nearest = GetNearestTiles(tile, false);

                    bool empty = true;

                    foreach (TMapTile nearestTile in nearest)
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


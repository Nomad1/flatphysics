﻿using RunMobile.Utility;

namespace FlatPhysics.Map
{
    public class BaseMapTile<TMapObject> where TMapObject : class, IGuidable
    {
        public const int TileExpireTime = 30000;

        private readonly ObjectArray<TMapObject> m_objects = new ObjectArray<TMapObject>();
        private readonly BaseMapInstance<TMapObject> m_map;

        private readonly int m_x;
        private readonly int m_y;
        private readonly ulong m_id;

        private long m_expireTime;

        private bool m_dirty;

        public BaseMapInstance<TMapObject> Map
        {
            get { return m_map; }
        }

        public int X
        {
            get { return m_x; }
        }

        public int Y
        {
            get { return m_y; }
        }

        public ulong Id
        {
            get { return m_id; }
        }

        public bool IsDirty
        {
            get { return m_dirty; }
        }

        public virtual bool IsEmpty
        {
            get { return m_objects.Empty && IsExpired; }
        }

        public bool IsExpired
        {
            get { return m_expireTime < GameTime.Now; }
        }

        public BaseMapTile(BaseMapInstance<TMapObject> map, int x, int y)
        {
            m_map = map;
            m_x = x;
            m_y = y;
            m_id = BaseMapInstance<TMapObject>.GetTileId(x, y);
            Advance();
        }

        public void Advance()
        {
            m_expireTime = GameTime.Now + TileExpireTime;
            m_dirty = true;
        }

        public virtual void Enter(TMapObject objectBase)
        {
            m_objects.Add(objectBase);
            Advance();
            Move(objectBase);
        }

        public virtual void Leave(TMapObject objectBase)
        {
            m_objects.Remove(objectBase);
            Advance();
        }

        public virtual void Move(TMapObject obj)
        {
            Advance();
        }

        public TMapObject[] GetObjects()
        {
            return m_objects.Array;
        }

        public TMapObject GetObject(ulong id)
        {
            return m_objects[id];
        }

        public override string ToString()
        {
            return string.Format("MapTile: X={0}, Y={1}, Dirty={2}, Objects={3}", X, Y, IsDirty, m_objects.Array.Length);
        }

        public virtual void Clean()
        {
            m_objects.Clear();
        }

        public virtual void Update()
        {
            m_dirty = false;
        }
    }
}

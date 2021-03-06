﻿using RunMobile.Utility;

namespace FlatPhysics.Map
{
    public class Base3dMapTile<TMapTile> where TMapTile : Base3dMapTile<TMapTile>
    {
        private readonly ObjectArray<IBaseMapObject<TMapTile>> m_objects = new ObjectArray<IBaseMapObject<TMapTile>>();
        private readonly Base3dMapInstance<TMapTile> m_map;

        private readonly short m_x;
        private readonly short m_y;
        private readonly short m_z;
        private readonly ulong m_id;

        private long m_expireTime;

        private bool m_dirty;

        public Base3dMapInstance<TMapTile> Map
        {
            get { return m_map; }
        }

        public short X
        {
            get { return m_x; }
        }

        public short Y
        {
            get { return m_y; }
        }

        public short Z
        {
            get { return m_z; }
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
            get { return m_objects.IsEmpty && IsExpired; }
        }

        public bool IsExpired
        {
            get { return m_expireTime < GameTime.Now; }
        }

        public Base3dMapTile(Base3dMapInstance<TMapTile> map, short x, short y, short z)
        {
            m_map = map;
            m_x = x;
            m_y = y;
            m_z = z;
            m_id = Base3dMapInstance<TMapTile>.GetTileId(x, y, z);
            Advance();
        }

        public void Advance()
        {
            m_expireTime = GameTime.Now + m_map.TileExpireTime;
            m_dirty = true;
        }

        public virtual void Enter(IBaseMapObject<TMapTile> objectBase)
        {
            m_objects.Add(objectBase);
            Advance();
            Move(objectBase);
        }

        public virtual void Leave(IBaseMapObject<TMapTile> objectBase)
        {
            m_objects.Remove(objectBase);
            Advance();
        }

        public virtual void Move(IBaseMapObject<TMapTile> obj)
        {
            Advance();
        }

        public IBaseMapObject<TMapTile>[] GetObjects()
        {
            return m_objects.Array;
        }

        public IBaseMapObject<TMapTile> GetObject(ulong id)
        {
            return m_objects[id];
        }

        public override string ToString()
        {
            return string.Format("MapTile: X={0}, Y={1}, Z={2}, Dirty={3}, Objects={4}", X, Y, Z, IsDirty, m_objects.Array.Length);
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

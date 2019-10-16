using System.Collections.Generic;

namespace FlatPhysics.Map
{
    internal class ObjectArray<TObject> where TObject : IGuidable
    {
        private readonly Dictionary<ulong, TObject> m_objectsList;

        private TObject[] m_objectsArray;

        private int m_arrayVersion;

        private int m_listVersion;

        public bool Empty
        {
            get { return Array.Length == 0; }
        }

        public TObject this[ulong guid]
        {
            get
            {
                TObject result;
                if (m_objectsList.TryGetValue(guid, out result))
                    return result;

                return default(TObject);
            }
        }

        public TObject[] Array
        {
            get
            {
                if (m_listVersion != m_arrayVersion)
                {
                    m_arrayVersion = m_listVersion;
                    m_objectsArray = new TObject[m_objectsList.Count];

                    int i = 0;
                    foreach (TObject objectBase in m_objectsList.Values)
                    {
                        m_objectsArray[i++] = objectBase;
                    }
                }
                return m_objectsArray;
            }
        }

        public ObjectArray()
        {
            m_objectsList = new Dictionary<ulong, TObject>();
            m_objectsArray = null;
            m_arrayVersion = 0;
            m_listVersion = 1;
        }

        public void Clear()
        {
            m_arrayVersion = 0;
            m_listVersion = 1;
            m_objectsArray = null;
            m_objectsList.Clear();
        }

        public void Remove(TObject objectBase)
        {
            if (m_objectsList.Remove(objectBase.Guid))
                m_listVersion++;
        }

        public void Add(TObject value)
        {
            m_objectsList[value.Guid] = value;
            m_listVersion++;
        }
    }
}

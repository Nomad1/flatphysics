using System.Collections.Generic;

namespace FlatPhysics.Map
{
    /// <summary>
    /// Simple collection that uses dictionary to store IGuidable objects and
    /// also have an array for fast (and non thread-safe!) access.
    /// </summary>
    public class ObjectArray<TObject> where TObject : IGuidable
    {
        private readonly IDictionary<ulong, TObject> m_objectsList;

        private TObject[] m_objectsArray;

        private uint m_arrayVersion;
        private uint m_dataVersion;

        public bool IsEmpty
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
                if (m_dataVersion != m_arrayVersion)
                {
                    m_arrayVersion = m_dataVersion;

                    m_objectsArray = new TObject[m_objectsList.Count];
                    m_objectsList.Values.CopyTo(m_objectsArray, 0);
                }
                return m_objectsArray;
            }
        }

        public ObjectArray()
        {
            m_objectsList = new Dictionary<ulong, TObject>();
            m_objectsArray = null;
            m_arrayVersion = 0;
            m_dataVersion = 1;
        }

        public void Clear()
        {
            m_arrayVersion = 0;
            m_dataVersion = 1;
            m_objectsArray = null;
            m_objectsList.Clear();
        }

        public bool Remove(TObject objectBase)
        {
            if (m_objectsList.Remove(objectBase.Guid))
            {
                m_dataVersion++;
                return true;
            }
            return false;
        }

        public void Add(TObject value)
        {
            m_objectsList[value.Guid] = value;
            m_dataVersion++;
        }
    }
}

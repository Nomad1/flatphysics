#if RUNSERVER
using RunServer.Common;
#else
using System.Numerics;
#endif

namespace FlatPhysics.Collision
{
    /// <summary>
    /// Helper class that incapsulates both transform and reference to the shape
    /// </summary>
    public class Fixture
    {
        private readonly IShape m_shape;

        private Transform m_transform;
        
        private bool m_changed;
        
        private Vector2 m_aabbMin;
        private Vector2 m_aabbMax;
        
        public IShape Shape
        {
            get { return m_shape; }
        }
        
        public Transform Transform
        {
            get { return m_transform; }
            set
            {
                m_transform = value;
                m_changed = true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FlatPhysics.Collision.Fixture"/> class.
        /// </summary>
        /// <param name="shape">Shape.</param>
        public Fixture(IShape shape)
            : this(shape, Transform.Empty)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FlatPhysics.Collision.Fixture"/> class.
        /// </summary>
        /// <param name="shape">Shape.</param>
        /// <param name="transform">Transform.</param>
        public Fixture(IShape shape, Transform transform)
        {
            m_shape = shape;
            m_transform = transform;
        }

        /// <summary>
        /// Gets the Axis-Aligned Bounding Box.
        /// </summary>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Max.</param>
        public void GetAABB(out Vector2 min, out Vector2 max)
        {
            if (m_changed)
            {
                m_shape.GetAABB(m_transform, out m_aabbMin, out m_aabbMax);
                m_changed = false;
            }

            min = m_aabbMin;
            max = m_aabbMax;
        }
    }
}

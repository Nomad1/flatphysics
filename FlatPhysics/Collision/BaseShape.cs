#if RUNSERVER
using RunServer.Common;
#else
using System.Numerics;
#endif

namespace FlatPhysics.Collision
{
    public abstract class BaseShape : IShape
    {
        private readonly ShapeType m_shapeType;
        private float m_boundingRadius;

        public ShapeType ShapeType
        {
            get { return m_shapeType; }
        }

        public float BoundingRadius
        {
            get { return m_boundingRadius; }
            protected set { m_boundingRadius = value; }
        }

        protected BaseShape(ShapeType shapeType, float boundingRadius = 0)
        {
            m_shapeType = shapeType;
            m_boundingRadius = boundingRadius;
        }

        public bool IntersectsWith(Transform ownTransform, IShape shape, Transform shapeTransform, out CollisionResult result)
        {
            return CollisionHelper.CheckIntersection(this, ownTransform, shape, shapeTransform, out result);
        }

        public abstract void GetAABB(Transform ownTransform, out Vector2 min, out Vector2 max);

        public abstract bool RayCast(Transform ownTransform, Vector2 from, Vector2 to, out float fraction,
            out Vector2 normal);
    }
}

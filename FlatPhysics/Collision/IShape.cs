#if RUNSERVER
using RunServer.Common;
#else
using System.Numerics;
#endif

namespace FlatPhysics.Collision
{
    /// <summary>
    /// Interface for collision shapes
    /// </summary>
    public interface IShape
    {
        ShapeType ShapeType { get; }
        
        float BoundingRadius { get; }

        void GetAABB(Transform ownTransform, out Vector2 min, out Vector2 max);

        bool RayCast(Transform ownTransform, Vector2 from, Vector2 to, out float fraction, out Vector2 normal);

        bool IntersectsWith(Transform ownTransform, IShape shape, Transform shapeTransform, out CollisionResult result);
    }
}
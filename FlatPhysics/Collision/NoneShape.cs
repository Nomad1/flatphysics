#if RUNSERVER
using RunServer.Common;
#else
using System.Numerics;
#endif

namespace FlatPhysics.Collision
{
    /// <summary>
    /// No shape dummy
    /// </summary>
    public class NoneShape : BaseShape
    {
        public NoneShape()
            : base(ShapeType.None, 0)
        {
        }

        public override void GetAABB(Transform ownTransform, out Vector2 min, out Vector2 max)
        {
            min = Vector2.Zero;
            max = Vector2.Zero;
        }

        public override bool RayCast(Transform ownTransform, Vector2 from, Vector2 to, out float fraction, out Vector2 normal)
        {
            fraction = 0;
            normal = Vector2.Zero;
            return false;
        }

    }
}

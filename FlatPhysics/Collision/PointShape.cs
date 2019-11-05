#if RUNSERVER
using RunServer.Common;
#else
using System.Numerics;
#endif

using System;

namespace FlatPhysics.Collision
{
    public class PointShape : BaseShape
    {
        private readonly Vector2 m_position;

        public Vector2 Position
        {
            get { return m_position; }
        }

        public PointShape(Vector2 point)
            : base(ShapeType.Point, point.Length() + float.Epsilon * 2)
        {
            m_position = point;
        }

        public override void GetAABB(Transform ownTransform, out Vector2 min, out Vector2 max)
        {
            Vector2 center = ownTransform.Mul(m_position);
            min = center - new Vector2(float.Epsilon, float.Epsilon);
            max = center + new Vector2(float.Epsilon, float.Epsilon);
        }

        public override bool RayCast(Transform ownTransform, Vector2 from, Vector2 to, out float fraction, out Vector2 normal)
        {
            fraction = 0;
            normal = Vector2.Zero;

            Vector2 center = ownTransform.Mul(m_position);

            if (center.X < Math.Min(to.X, from.X) || center.Y < Math.Min(to.Y, from.Y) || center.X > Math.Max(to.X, from.X) || center.Y > Math.Max(to.Y, from.Y))
                return false;

            Vector2 ray = to - from;
            float len = ray.Length(); // normalization with storing previous length value
            ray /= len;

            Vector2 cray = center - from;
            float clen = cray.Length(); // normalization with storing previous length value
            cray /= clen;

            if (Math.Abs(ray.X - cray.X) > float.Epsilon || Math.Abs(ray.Y - cray.Y) > float.Epsilon)
                return false;

            fraction = clen / len;

            normal = to; // Nomad: I'm not sure we have normalized vector here. Check if it breaks something

            return true;
        }
    }
}

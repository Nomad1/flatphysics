#if RUNSERVER
using RunServer.Common;
#else
using System.Numerics;
#endif

#if USE_MATHF
using Math = FlatPhysics.Mathf;
#else
using Math = System.Math;
#endif

namespace FlatPhysics.Collision
{
    public class CircleShape : BaseShape
    {
        private readonly Vector2 m_center;
        private readonly float m_radius;

        public Vector2 Center
        {
            get { return m_center; }
        }

        public float Radius
        {
            get { return m_radius; }
        }

        public CircleShape(Vector2 center, float radius)
        : base(ShapeType.Circle, radius + center.Length())
        {
            m_center = center;
            m_radius = radius;
        }

        public override void GetAABB(Transform ownTransform, out Vector2 min, out Vector2 max)
        {
            Vector2 center = ownTransform.Mul(m_center);
            min = center - new Vector2(m_radius, m_radius);
            max = center + new Vector2(m_radius, m_radius);
        }

        public override bool RayCast(Transform ownTransform, Vector2 from, Vector2 to, out float fraction, out Vector2 normal)
        {
            // TODO: I'm copying this code from FarSeer engine. That means we need either to open source this engine
            // or rewrite this method completely

            // Collision Detection in Interactive 3D Environments by Gino van den Bergen
            // From Section 3.1.2
            // x = s + a * r
            // norm(x) = radius

            fraction = 0;
            normal = Vector2.Zero;


            Vector2 center = ownTransform.Mul(m_center);
            Vector2 s = from - center;
            Vector2 r = to - from;

            bool invert = false;

            float b = s.LengthSquared() - Radius * Radius;
            if (b < 0)
            {
                s = to - center;
                b = s.LengthSquared() - Radius * Radius;
                r = -r;
                invert = true;
            }

            // Solve quadratic equation.
            float c = Vector2.Dot(s, r);
            float rr = r.LengthSquared();
            float sigma = c * c - rr * b;

            // Check for negative discriminant and short segment.
            if (sigma < 0.0f || rr < float.Epsilon)
            {
                return false;
            }

            // Find the point of intersection of the line with the circle.
            float a = -(c + (float)Math.Sqrt(sigma)); //TODO: Move to mathhelper?

            // Is the intersection point on the segment?
            if (0.0f <= a && a <= rr)
            {
                a /= rr;
                fraction = a;
                if (invert)
                    fraction = 1.0f - fraction;

                normal = s + r * a;
                normal = Vector2.Normalize(normal);
                return true;
            }

            return false;
        }

    }
}

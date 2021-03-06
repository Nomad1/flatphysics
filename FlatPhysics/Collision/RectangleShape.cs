﻿#if RUNSERVER
using RunServer.Common;
#else
using System.Numerics;
#endif

namespace FlatPhysics.Collision
{
    public class RectangleShape : BaseShape
    {
        internal static readonly Vector2[] RectangleNormals = { new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1) };
        // left, top, right, bottom

        private readonly Vector2 m_center;
        private readonly float m_width;
        private readonly float m_height;
        private readonly Vector2[] m_points;

        public Vector2 Center
        {
            get { return m_center; }
        }

        public float Width
        {
            get { return m_width; }
        }

        public float Height
        {
            get { return m_height; }
        }

        public Vector2[] Points
        {
            get { return m_points; }
        }

        public Vector2[] Normals
        {
            get { return RectangleNormals; }
        }

        public RectangleShape(Vector2 center, float width, float height)
        : base(ShapeType.Rectangle)
        {
            m_center = center;
            m_width = width;
            m_height = height;

            // Rectangle actualy becomes CW polygon here
            // 
            //        ^ y, top
            //       ___
            //      |   |
            // left | 0 | ->x, right
            //      |___| 
            //     bottom

            m_points = new Vector2[]
            {
                m_center + new Vector2(-m_width / 2, -m_height / 2), // bottom left
                m_center + new Vector2(-m_width / 2, m_height / 2), // top left
                m_center + new Vector2(m_width / 2, m_height / 2), // top right
                m_center + new Vector2(m_width / 2, -m_height / 2), // bottom right
            };

            BoundingRadius = m_points[0].Length();
        }

        public override void GetAABB(Transform ownTransform, out Vector2 min, out Vector2 max)
        {
            Vector2 zeroPoint = ownTransform.Mul(m_points[0]);

            float minx = zeroPoint.X;
            float miny = zeroPoint.Y;
            float maxx = zeroPoint.X;
            float maxy = zeroPoint.Y;

            for (int i = 1; i < m_points.Length; i++)
            {
                Vector2 point = ownTransform.Mul(m_points[i]);

                if (point.X < minx)
                    minx = point.X;

                if (point.Y < miny)
                    miny = point.Y;

                if (point.X > maxx)
                    maxx = point.X;

                if (point.Y > maxy)
                    maxy = point.Y;
            }

            min = new Vector2(minx, miny);
            max = new Vector2(maxx, maxy);
        }

        public override bool RayCast(Transform ownTransform, Vector2 from, Vector2 to, out float fraction, out Vector2 normal)
        {
            fraction = 0;
            normal = Vector2.Zero;

            // Put the ray into the polygon's frame of reference.
            Vector2 p1 = ownTransform.MulT(from);
            Vector2 p2 = ownTransform.MulT(to);
            Vector2 d = p2 - p1;

            float lower = 0.0f, upper = 1.0f;

            int lowerIndex = -1;
            int upperIndex = -1;

            for (int i = 0; i < Points.Length; ++i)
            {
                // p = p1 + a * d
                // dot(normal, p - v) = 0
                // dot(normal, p1 - v) + a * dot(normal, d) = 0
                float numerator = Vector2.Dot(Normals[i], Points[i] - p1);
                float denominator = Vector2.Dot(Normals[i], d);

                if (denominator == 0.0f)
                {
                    if (numerator < 0.0f)
                    {
                        return false;
                    }
                }
                else
                {
                    // Note: we want this predicate without division:
                    // lower < numerator / denominator, where denominator < 0
                    // Since denominator < 0, we have to flip the inequality:
                    // lower < numerator / denominator <==> denominator * lower > numerator.
                    if (denominator < 0.0f && numerator < lower * denominator)
                    {
                        // Increase lower.
                        // The segment enters this half-space.
                        lower = numerator / denominator;
                        lowerIndex = i;
                    }
                    else if (denominator > 0.0f && numerator < upper * denominator)
                    {
                        // Decrease upper.
                        // The segment exits this half-space.
                        upper = numerator / denominator;
                        upperIndex = i;
                    }
                }

                // The use of epsilon here causes the assert on lower to trip
                // in some cases. Apparently the use of epsilon was to make edge
                // shapes work, but now those are handled separately.
                //if (upper < lower - b2_epsilon)
                if (upper < lower)
                {
                    return false;
                }
            }

            //Debug.Assert(0.0f <= lower && lower <= input.MaxFraction);

            if (lowerIndex >= 0)
            {
                fraction = lower;
                normal = ownTransform.Mul(Normals[lowerIndex]);
                normal = Vector2.Normalize(normal);
                return true;
            }

            if (upperIndex >= 0)
            {
                fraction = upper;
                normal = ownTransform.Mul(Normals[upperIndex]);
                normal = Vector2.Normalize(normal);
                return true;
            }

            return false;
        }
    }
}
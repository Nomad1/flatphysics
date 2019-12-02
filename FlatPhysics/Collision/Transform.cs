#if RUNSERVER
using RunServer.Common;
#else
using System.Numerics;
#endif

namespace FlatPhysics.Collision
{
    /// <summary>
    /// Helper immutable struct containing offset point and rotation angle among with it's sine and cosine values
    /// </summary>
    public struct Transform
    {
        public readonly static Transform Empty = default(Transform);

        private readonly Vector2 m_offset;

        private readonly float m_sin;
        private readonly float m_cos;
        private readonly float m_angle;

        public Vector2 Offset
        {
            get { return m_offset; }
        }

        public float Angle
        {
            get { return m_angle; }
        }

        public Vector2 Direction
        {
            get { return new Vector2(m_cos, -m_sin); }
        }

        public bool IsValid
        {
            get { return m_cos != 0.0f && m_sin != 0.0f; } // there is no way sin and cos could be zero
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FlatPhysics.Collision.Transform"/> struct
        /// </summary>
        /// <param name="offset">Offset.</param>
        /// <param name="angle">Angle.</param>
        public Transform(Vector2 offset, float angle)
        {
            m_offset = offset;
            m_angle = angle;
#if USE_MATHF
            Mathf.SinCos(angle, out m_sin, out m_cos);
#else
            m_sin = (float)System.Math.Sin(angle);
            m_cos = (float)System.Math.Cos(angle);
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FlatPhysics.Collision.Transform"/> struct
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="angle">Angle.</param>
        public Transform(float x, float y, float angle)
            : this(new Vector2(x, y), angle)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FlatPhysics.Collision.Transform"/> struct using new shift point and angle from other transform
        /// </summary>
        /// <param name="offset">Offset.</param>
        /// <param name="otherTransform">Other transform.</param>
        public Transform(Vector2 offset, Transform otherTransform)
        {
            m_offset = offset;
            m_angle = otherTransform.m_angle;
            m_sin = otherTransform.m_sin;
            m_cos = otherTransform.m_cos;
        }


        /// <summary>
        /// Rotate and shift the point
        /// </summary>
        /// <returns>The transformed point</returns>
        /// <param name="v">Source point</param>
        public Vector2 Mul(Vector2 v)
        {
            float x = (m_cos * v.X - m_sin * v.Y);
            float y = (m_sin * v.X + m_cos * v.Y);

            return new Vector2(x, y) + m_offset;
        }

        /// <summary>
        /// Rotate the point without shifting
        /// </summary>
        /// <returns>The transformed point</returns>
        /// <param name="v">Source point</param>
        public Vector2 MulR(Vector2 v)
        {
            float x = (m_cos * v.X - m_sin * v.Y);
            float y = (m_sin * v.X + m_cos * v.Y);

            return new Vector2(x, y);
        }

        /// <summary>
        /// Transposed operation to Mul
        /// </summary>
        /// <returns>The transformed point</returns>
        /// <param name="v">Source point</param>
        public Vector2 MulT(Vector2 v)
        {
            float px = v.X - m_offset.X;
            float py = v.Y - m_offset.Y;
            float x = (m_cos * px + m_sin * py);
            float y = (-m_sin * px + m_cos * py);

            return new Vector2(x, y);
        }

        /// <summary>
        /// Transposed operation to MulR
        /// </summary>
        /// <returns>The transformed point</returns>
        /// <param name="v">Source point</param>
        public Vector2 MulTR(Vector2 v)
        {
            float x = (m_cos * v.X + m_sin * v.Y);
            float y = (-m_sin * v.X + m_cos * v.Y);

            return new Vector2(x, y);
        }

        // static helper methods
       
        public static Vector2 Mul(Transform t, Vector2 v)
        {
            return t.Mul(v);
        }

        public static Vector2 MulT(Transform t, Vector2 v)
        {
            return t.MulT(v);
        }

        public static Vector2 MulR(Transform t, Vector2 v)
        {
            return t.MulR(v);
        }
    }
}


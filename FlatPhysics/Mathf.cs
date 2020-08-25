//#if __IOS__
//#define PRECISE   // uncomment this one to use System.Math instead
//#endif

namespace FlatPhysics
{
    /// <summary>
    /// Helper class for floating point geometry. Note if could be slower than 
    /// native System.Math because Math is to be optimized with SSE/AVX on some runtimes
    /// </summary>
    public static class Mathf
    {
        #region Constants

        public const float PI = 3.14159265358979323846f;
        public const float TwoPI = 6.283185307179586f;
        public const float InvTwoPI = 0.1591549f;
        public const float HalfPI = 1.57079632679489661923f;
        public const float ThreeHalfPI = 4.7123889f;

        public const float Deg2Rad = 0.017453293f;
        public const float Rad2Deg = 57.29577793f;

        public const float E = 2.7182818284590451f;

        public const float Epsilon = 1.192092896e-07f;

        public const float TwoPower24 = 16777216.0f;

        #endregion

        #region Fast Methods

        private static float FastInvSqrt(float x)
        {
            if (float.IsNaN(x) || x < 0)
                return float.NaN;

            if (x == 0)
                return float.PositiveInfinity;

            // int-based calculation, taken from Doom code
            unsafe
            {
                float xhalf = 0.5f * x;
                int i = *(int*)&x;
                i = 0x5f3759df - (i >> 1);
                x = *(float*)&i;
                x = x * (1.5f - (xhalf * x * x));
                return x;
            }
        }

        private static float FastSqrt(float x)
        {
            if (float.IsNaN(x) || x < 0)
                return float.NaN;

            if (x > 20)
                return FastPow2(0.5f * FastLog2(x));

            // int-based calculation
            unsafe
            {
                int i = *(int*)&x;
                i = (1 << 29) + (i >> 1) - (1 << 22);
                return *(float*)&i;
            }
        }

        private static float FastPow2(float p)
        {
            if (float.IsNaN(p) || float.IsPositiveInfinity(p))
                return p;

            if (float.IsNegativeInfinity(p))
                return 0;

            if (p > 127)
                return float.PositiveInfinity;

            // approx calculation
            float offset = (p < 0) ? 1.0f : 0.0f;
            float clipp = (p < -126) ? -126.0f : p;
            float z = Frac(clipp) + offset;

            unsafe
            {
                uint i = (uint)((1 << 23) * (clipp + 121.2740575f + 27.7280233f / (4.84252568f - z) - 1.49012907f * z));
                return *(float*)&i;
            }
        }

        private static float FastExp(float p)
        {
            // pow2-based calculation
            return FastPow2(1.442695040f * p);
        }

        private static float FastLog2(float x)
        {
            if (float.IsNaN(x) || x < 0)
                return float.NaN;

            if (x == 0)
                return float.NegativeInfinity;

            // approx calculation
            float f, y;
            unsafe
            {
                uint i = *(uint*)&x;
                uint mx = (i & 0x007FFFFF) | 0x3f000000;
                f = *(float*)&mx;
                y = i;
            }

            y *= 1.1920928955078125e-7f;

            return y - 124.22551499f
                     - 1.498030302f * f
                     - 1.72587999f / (0.3520887068f + f);
        }

        private static float FastLog(float x)
        {
            // log2-based calculation
            return 0.69314718f * FastLog2(x);
        }

        #endregion

        #region Mathematics

        /// <summary>
        /// Square root for the specified value.
        /// </summary>
        /// <returns>The sqrt.</returns>
        /// <param name="value">Value.</param>
        public static float Sqrt(float value)
        {
#if PRECISE
            return (float)System.Math.Sqrt(value);
#else
            return FastSqrt(value);
#endif
        }

        /// <summary>
        /// Inverted square root
        /// </summary>
        /// <returns>The math result</returns>
        /// <param name="value">Value.</param>
        public static float InvSqrt(float value)
        {
#if PRECISE
            return (float)(1.0 / System.Math.Sqrt(value));
#else
            return FastInvSqrt(value);
#endif
        }

        public static void SinCos(float angle, out float s, out float c)
        {
#if PRECISE
            s = (float)System.Math.Sin(angle);
            c = (float)System.Math.Cos(angle);
#elif true || FAILSAFE
            if (angle == 0.0f)
            {
                s = 0.0f;
                c = 1.0f;
                return;
            }

            s = Sin(angle);
            c = Cos(angle);
#else
            angle = NormalizeAngle(angle);

            if (angle == 0.0f)
            {
                s = 0.0f;
                c = 1.0f;
                return;
            }

            c = Cos(angle);
            s = Sqrt(1.0f - c * c) * (angle > 0.0f && angle < PI ? 1.0f : -1.0f);
#endif
        }
        /// <summary>
        /// Sine of the specified angle.
        /// </summary>
        /// <returns>The sin.</returns>
        /// <param name="angle">Angle.</param>
        public static float Sin(float angle)
        {
#if PRECISE
            return (float)System.Math.Sin(angle);
#else
            float x;
            float xx;
            float ret;

            angle = NormalizeAngle(angle);

            xx = angle * angle;
            x = angle;                      //1
            ret = 0.9999997192673006f * x;
            x *= xx;                        //3
            ret -= 0.1666657564532464f * x;
            x *= xx;                        //5
            ret += 0.008332803647181511f * x;
            x *= xx;                        //7
            ret -= 0.00019830197237204295f * x;
            x *= xx;                        //9
            ret += 2.7444305061093514e-6f * x;
            x *= xx;                        //11
            ret -= 2.442176561869478e-8f * x;
            x *= xx;                        //13
            ret += 1.407555708887347e-10f * x;
            x *= xx;                        //15
            ret -= 4.240664814288337e-13f * x;

            return (float)ret;
#endif
        }

        /// <summary>
        /// Cosine of the specified angle.
        /// </summary>
        /// <returns>The cos.</returns>
        /// <param name="angle">Angle.</param>
        public static float Cos(float angle)
        {
#if PRECISE
            return (float)System.Math.Cos(angle);
#else
            return Sin(angle + HalfPI);
#endif
        }

        public static float Acos(float x)
        {
#if PRECISE
            return (float)System.Math.Acos(x);
#else
            float negate = x < 0 ? 1.0f : 0.0f;
            x = Abs(x);
            float ret = -0.0187293f;
            ret = ret * x;
            ret = ret + 0.0742610f;
            ret = ret * x;
            ret = ret - 0.2121144f;
            ret = ret * x;
            ret = ret + HalfPI;
            ret = ret * Sqrt(1.0f - x);
            ret = ret - 2 * negate * ret;
            return negate * PI + ret;
#endif
        }

        /// <summary>
        /// Arctangent for the specified y/x value. Y and X are separated for correct calculation when x == 0
        /// </summary>
        /// <returns>The atan.</returns>
        /// <param name="y">The y coordinate.</param>
        /// <param name="x">The x coordinate.</param>
        public static float Atan2(float y, float x)
        {
#if PRECISE
            return (float)System.Math.Atan2(y, x);
#else
            float t0, t1, t3, t4;

            t3 = Abs(x);
            t1 = Abs(y);
            t0 = Max(t3, t1);
            t1 = Min(t3, t1);
            t3 = 1.0f / t0;
            t3 = t1 * t3;

            t4 = t3 * t3;
            t0 = -0.013480470f;
            t0 = t0 * t4 + 0.057477314f;
            t0 = t0 * t4 - 0.121239071f;
            t0 = t0 * t4 + 0.195635925f;
            t0 = t0 * t4 - 0.332994597f;
            t0 = t0 * t4 + 0.999995630f;
            t3 = t0 * t3;

            t3 = (Abs(y) > Abs(x)) ? HalfPI - t3 : t3;
            t3 = (x < 0) ? PI - t3 : t3;
            t3 = (y < 0) ? -t3 : t3;

            return t3;
#endif
        }

        /// <summary>
        /// Exponent for the specified value.
        /// </summary>
        /// <returns>The exp.</returns>
        /// <param name="value">Value.</param>
        public static float Exp(float value)
        {
#if PRECISE
            return (float)System.Math.Exp(value);
#else
            return FastExp(value);
#endif
        }

        /// <summary>
        /// Log the specified value.
        /// </summary>
        /// <returns>The log.</returns>
        /// <param name="value">Value.</param>
        public static float Log(float value)
        {
#if PRECISE
            return (float)System.Math.Log(value);
#else
            return FastLog(value);
#endif
        }

        /// <summary>
        /// Calculate x^p
        /// </summary>
        /// <returns>The power</returns>
        /// <param name="x">The x value.</param>
        /// <param name="p">power.</param>
        public static float Pow(float x, float p)
        {
#if PRECISE
            return (float)System.Math.Pow(x, p);
#else
            if (float.IsNaN(x) || float.IsNaN(p) || (x < 0 && Abs(p) < 1))
                return float.NaN;

            if (p == 0)
                return 1;
            if (x == 0)
                return 0;

            return FastPow2(p * FastLog2(x));
#endif
        }

        #endregion

        #region Helpers

        public static float NormalizeAngle(float angle)
        {
            return angle - TwoPI * Floor(angle * InvTwoPI + 0.5f);
        }

        public static float Abs(float value)
        {
            return value < 0 ? -value : value;
        }

        public static int Abs(int value)
        {
            return value < 0 ? -value : value;
        }

        public static int Sign(float value)
        {
            return value < 0 ? -1 : value > 0 ? 1 : 0;
        }

        public static int Sign(int value)
        {
            return value < 0 ? -1 : value > 0 ? 1 : 0;
        }

        public static float Frac(float value)
        {
            return value - (int)value;
        }

        public static float Round(float value)
        {
            return (float)System.Math.Round(value, System.MidpointRounding.AwayFromZero);
        }

        public static int Floor(float x)
        {
#if PRECISE
            return (int)System.Math.Floor(x);
#else
            if (float.IsNaN(x))
                return 0;

            int y = (int)x;

            if (x >= TwoPower24 || x <= -TwoPower24)
                return y;

            if (x < 0 && y != x)
                y--;

            //if (y == 0)
            //return x * y;

            return y;
#endif
        }

        public static int Ceiling(float value)
        {
            return (int)System.Math.Ceiling(value);
        }

        public static int Min(int v1, int v2)
        {
            return v1 < v2 ? v1 : v2;
        }

        public static float Min(float v1, float v2)
        {
            return v1 < v2 ? v1 : v2;
        }

        public static int Max(int v1, int v2)
        {
            return v1 > v2 ? v1 : v2;
        }

        public static float Max(float v1, float v2)
        {
            return v1 > v2 ? v1 : v2;
        }

        public static bool FloatEquals(float v1, float v2, float epsilon = Epsilon)
        {
            if (float.IsNaN(v1) || float.IsNaN(v2))
                return false;

            float diff = Abs(v1 - v2);
            if (diff <= epsilon)
                return true;

            v1 = Abs(v1);
            v2 = Abs(v2);

            return diff <= Max(v1, v2) * epsilon;
        }

        #endregion
    }
}


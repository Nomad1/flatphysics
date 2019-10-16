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

namespace FlatPhysics
{
    public static class MoveMathVector
    {
        public const float Epsilon = 1E-4f;

        private static void SinCos(float facing, out float sin, out float cos)
        {
#if !USE_MATHF
            sin = (float)Math.Sin(facing);
            cos = (float)Math.Cos(facing);
#else
            Math.SinCos(facing, out sin, out cos);
#endif
        }

        /// <summary>
        /// Calculates position with variable linear speed and constant angular speed
        /// </summary>
        /// <returns>The variable linear constant angular shift.</returns>
        /// <param name="velocity">Linear velocity</param>
        /// <param name="acceleration">Linear acceleration</param>
        /// <param name="position">Position</param>
        /// <param name="angularVelocity">Constant angular velocity</param>
        /// <param name="facing">Angle</param>
        /// <param name="time">Time</param>
        public static void GetVarLinearConstantAngularPosition(ref Vector2 velocity, float maxVelocity, Vector2 accelerationDirection, float acceleration, ref Vector2 position, float angularVelocity, ref float facing, float time)
        {
            if (Math.Abs(angularVelocity) < Epsilon)
            {
                GetVarLinearPosition(ref velocity, maxVelocity, accelerationDirection, acceleration, ref position, facing, time);
                return;
            }

            float sinb;
            float cosb;

            SinCos(facing, out sinb, out cosb);

            float endFacing = facing + angularVelocity * time;

            float sinend;
            float cosend;

            SinCos(endFacing, out sinend, out cosend);

            float exp = (float)Math.Exp(-acceleration * time / maxVelocity);
            float vw = maxVelocity * angularVelocity;
            float divider = maxVelocity * maxVelocity / (acceleration * acceleration + vw * vw);

            float vx = maxVelocity * cosend - (maxVelocity * cosend - velocity.X) * exp;
            float vy = maxVelocity * sinend - (maxVelocity * sinend - velocity.Y) * exp;

            Vector2 endVelocity = new Vector2(vx, vy);

            Vector2 shift = velocity * maxVelocity * (1 - exp) / acceleration;

            float x =
                      (-sinb + sinend) * maxVelocity / angularVelocity +
                      divider * (exp * (acceleration * cosend - vw * sinend) - (acceleration * cosb - vw * sinb));

            float y =
                      (cosb - cosend) * maxVelocity / angularVelocity -
                      divider * (-exp * (acceleration * sinend + vw * cosend) + (acceleration * sinb + vw * cosb));

            position += shift + new Vector2(x, y);
            velocity = endVelocity;

            facing = endFacing;
        }

        /// <summary>
        /// Calculates distance that object can move with current speed and zero acceleration (damping only)
        /// </summary>
        /// <returns>The damping distance.</returns>
        /// <param name="velocity">Velocity.</param>
        /// <param name="maxVelocity">Max velocity.</param>
        /// <param name="damping">Damping.</param>
        public static float GetDampingDistance(float velocity, float maxVelocity, float damping)
        {
            float speedPct = velocity / maxVelocity;

            return maxVelocity * maxVelocity * (speedPct - (float)Math.Log(1 + speedPct)) / Math.Abs(damping);
        }

        /// <summary>
        /// Calculates average movement time to the point
        /// </summary>
        /// <returns>The average time.</returns>
        /// <param name="velocity">Velocity.</param>
        /// <param name="maxVelocity">Max velocity.</param>
        /// <param name="position">Position.</param>
        /// <param name="target">Target.</param>
        public static float GetAverageMoveTime(Vector2 velocity, float maxVelocity, Vector2 position, Vector2 target)
        {
            Vector2 direction = (target - position);
            float distance = direction.Length();
            direction /= distance;

            float v = Vector2.Dot(velocity, direction);

            return distance / ((maxVelocity - v) / 2.0f);
        }

#if RUNSERVER
        
        private static float s_oneMinusLog2 = (1 - (float)Math.Log(2.0f));
        
        /// <summary>
        /// Calculates average movement time to the point
        /// </summary>
        /// <returns>The average time.</returns>
        /// <param name="velocity">Velocity.</param>
        /// <param name="maxVelocity">Max velocity.</param>
        /// <param name="position">Position.</param>
        /// <param name="target">Target.</param>
        public static float GetAverageMoveTime(Vector velocity, float maxVelocity, float damping, float acceleration, Vector position, Vector target, float epsilon)
        {
            Vector direction = (target - position);
            float distance = direction.Normalize();

            float v = Vector.Dot(velocity, direction);
            
            float dampingDistance = maxVelocity * maxVelocity * s_oneMinusLog2 / damping;

            float t;
            
            if (distance > dampingDistance)
                t = (distance - dampingDistance) / maxVelocity + (maxVelocity - v) / acceleration; // rought acceleration time, will be smaller than actual time
            else
                t = 0.1f;
            
            float ndistance;
            float u;
            int iterations = 10; // limit the cycle
            do
            {
                float exp = (float)Math.Exp(-acceleration * t / maxVelocity);
                
                u = maxVelocity - (maxVelocity - v) * exp;
                
                ndistance = maxVelocity * (t - (u - v) / acceleration) + GetDampingDistance(u, maxVelocity, damping);
                
                if (ndistance >= distance + epsilon) // average goal
                    break;
                
                t += 0.1f;
                
            } while(ndistance < distance && --iterations >= 0);
            
            return t + maxVelocity * (float)Math.Log(1.0f + u / maxVelocity) / damping;
        }
#endif
        /// <summary>
        /// Calculates position with variable linear speed and zero angular speed
        /// </summary>
        /// <param name="velocity">Linear velocity</param>
        /// <param name="acceleration">Linear acceleration</param>
        /// <param name="position">Position</param>
        /// <param name="facing">Angle</param>
        /// <param name="time">Time</param>
        public static void GetVarLinearPosition(ref Vector2 velocity, float maxVelocity, Vector2 accelerationDirection, float acceleration, ref Vector2 position, float facing, float time)
        {
            float exp = (float)Math.Exp(-acceleration * time / maxVelocity);

            Vector2 endVelocity = accelerationDirection * maxVelocity * (1 - exp) + velocity * exp;

            float k = maxVelocity * (1 - exp) / acceleration;

            position += accelerationDirection * maxVelocity * (time - k) + velocity * k;

            velocity = endVelocity;
        }

        /// <summary>
        /// Calculates angle with variable angular speed and zero linear speed
        /// </summary>
        /// <param name="angularVelocity">Angular velocity</param>
        /// <param name="acceleration">Angular acceleration</param>
        /// <param name="facing">Facing</param>
        /// <param name="time">Time</param>
        public static void GetVarAngularFacing(ref float angularVelocity, float acceleration, ref float facing, float time)
        {
            float endVelocity = angularVelocity + time * acceleration;

            float endFacing = facing + (angularVelocity + endVelocity) * time / 2.0f;

            facing = endFacing;
            angularVelocity = endVelocity;
        }

        /// <summary>
        /// Calculates angle with constant angular speed and zero linear speed
        /// </summary>
        /// <param name="angularVelocity">Angular velocity</param>
        /// <param name="facing">Facing</param>
        /// <param name="time">Time</param>
        public static void GetConstantAngularFacing(float angularVelocity, ref float facing, float time)
        {
            facing = facing + angularVelocity * time;
        }

#if NO_DISSIPATIVE_FORCE
        /// <summary>
        /// Gets the variable linear variable angular position.
        /// </summary>
        /// <param name="v">Velocity.</param>
        /// <param name="a">Acceleration.</param>
        /// <param name="position">Position.</param>
        /// <param name="w">Angular velocity.</param>
        /// <param name="n">Angular acceleration.</param>
        /// <param name="b">Facing.</param>
        /// <param name="t">Time.</param>
        public static void GetVarLinearVarAngularPosition(ref Vector2 velocity, float maxVelocity, Vector2 accelerationDirection, float acceleration, ref Vector2 position, ref float angularVelocity, float angularAcceleration, ref float facing, float time)
        {
//            if (Math.Abs(angularAcceleration) < Epsilon)
            {
                GetVarLinearConstantAngularPosition(ref velocity, maxVelocity, accelerationDirection, acceleration, ref position, angularVelocity, ref facing, time);
                return;
            }

            Vector2 endVelocity = velocity + accelerationDirection * acceleration * time;
            //Vector2 endVelocity = accelerationDirection*(Vector2.Dot(velocity, accelerationDirection) + acceleration * time);
            
//            Vector2 endVelocity = v + (a > 0 ? Vector2.Rotation(b) : Vector2.Normalize(v)) * t * a;
            float endAngularVelocity = angularVelocity + angularAcceleration * time;
            float endFacing = facing + (angularVelocity + endAngularVelocity) * time / 2.0f;
            
            float sn = (float)Math.Sqrt(Math.Abs(angularAcceleration));
            float spi = (float)Math.Sqrt(Math.PI);
            
            float divider = acceleration / (sn * Math.Abs(angularAcceleration)); // common denominator, a/n^1.5

            float bwn = facing - angularVelocity * angularVelocity / (2 * angularAcceleration); // b - w^2/2n, some kind of angle?
            
            float nvaw = angularVelocity * spi;

            float cosBwn = (float)Math.Cos(bwn);
            float sinBwn = (float)Math.Sin(bwn);


            float ws = angularVelocity / (spi * sn); // StartAngularVelocity / sqrt(2*PI*n)
            float ntws = (angularAcceleration * time + angularVelocity) / (spi * sn); // 

            float frenC1, frenC2, frenS1, frenS2;
            Fresnel(ws, out frenC1, out frenS1);
            Fresnel(ntws, out frenC2, out frenS2);

            float frenC = (float)(frenC1 - frenC2);
            float frenS = (float)(frenS1 - frenS2);

            float sinDiff = sn * ((float)Math.Sin(facing) - (float)Math.Sin(endFacing));
            float cosDiff = sn * ((float)Math.Cos(facing) - (float)Math.Cos(endFacing));

            float x, y;
            if (angularAcceleration > 0)
            {
                x = (nvaw * (cosBwn * frenC - sinBwn * frenS) - sinDiff) * divider;
                y = (nvaw * (cosBwn * frenS + sinBwn * frenC) + cosDiff) * divider;
            } else
            {
                x = (nvaw * (cosBwn * frenC + sinBwn * frenS) + sinDiff) * divider;
                y = (nvaw * (-cosBwn * frenS + sinBwn * frenC) - cosDiff) * divider;
            }

            position += velocity * time + new Vector2(x, y);
            velocity = endVelocity;
            angularVelocity = endAngularVelocity;
            facing = endFacing;

        }
        
        private static void Fresnel(float x,
            out float c,
            out float s)
        {
            double xxa = 0;
            double f = 0;
            double g = 0;
            double t = 0;
            double u = 0;
            double sn = 0;
            double sd = 0;
            double cn = 0;
            double cd = 0;
            double fn = 0;
            double fd = 0;
            double gn = 0;
            double gd = 0;

            xxa = x;
            x = Math.Abs(x);
            
            float x2 = x * x;
            
            if (x2 < 2.5625)
            {
                t = x2 * x2;
                sn = -2.99181919401019853726E3;
                sn = sn * t + 7.08840045257738576863E5;
                sn = sn * t - 6.29741486205862506537E7;
                sn = sn * t + 2.54890880573376359104E9;
                sn = sn * t - 4.42979518059697779103E10;
                sn = sn * t + 3.18016297876567817986E11;
                sd = 1.00000000000000000000E0;
                sd = sd * t + 2.81376268889994315696E2;
                sd = sd * t + 4.55847810806532581675E4;
                sd = sd * t + 5.17343888770096400730E6;
                sd = sd * t + 4.19320245898111231129E8;
                sd = sd * t + 2.24411795645340920940E10;
                sd = sd * t + 6.07366389490084639049E11;
                cn = -4.98843114573573548651E-8;
                cn = cn * t + 9.50428062829859605134E-6;
                cn = cn * t - 6.45191435683965050962E-4;
                cn = cn * t + 1.88843319396703850064E-2;
                cn = cn * t - 2.05525900955013891793E-1;
                cn = cn * t + 9.99999999999999998822E-1;
                cd = 3.99982968972495980367E-12;
                cd = cd * t + 9.15439215774657478799E-10;
                cd = cd * t + 1.25001862479598821474E-7;
                cd = cd * t + 1.22262789024179030997E-5;
                cd = cd * t + 8.68029542941784300606E-4;
                cd = cd * t + 4.12142090722199792936E-2;
                cd = cd * t + 1.00000000000000000118E0;
                s = (float)(Math.Sign(xxa) * x * x2 * sn / sd);
                c = (float)(Math.Sign(xxa) * x * cn / cd);
                return;
            }
            if (x > 36974.0)
            {
                c = Math.Sign(xxa) * 0.5f;
                s = Math.Sign(xxa) * 0.5f;
                return;
            }
            t = Math.PI * x2;
            u = 1 / (t * t);
            t = 1 / t;
            fn = 4.21543555043677546506E-1;
            fn = fn * u + 1.43407919780758885261E-1;
            fn = fn * u + 1.15220955073585758835E-2;
            fn = fn * u + 3.45017939782574027900E-4;
            fn = fn * u + 4.63613749287867322088E-6;
            fn = fn * u + 3.05568983790257605827E-8;
            fn = fn * u + 1.02304514164907233465E-10;
            fn = fn * u + 1.72010743268161828879E-13;
            fn = fn * u + 1.34283276233062758925E-16;
            fn = fn * u + 3.76329711269987889006E-20;
            fd = 1.00000000000000000000E0;
            fd = fd * u + 7.51586398353378947175E-1;
            fd = fd * u + 1.16888925859191382142E-1;
            fd = fd * u + 6.44051526508858611005E-3;
            fd = fd * u + 1.55934409164153020873E-4;
            fd = fd * u + 1.84627567348930545870E-6;
            fd = fd * u + 1.12699224763999035261E-8;
            fd = fd * u + 3.60140029589371370404E-11;
            fd = fd * u + 5.88754533621578410010E-14;
            fd = fd * u + 4.52001434074129701496E-17;
            fd = fd * u + 1.25443237090011264384E-20;
            gn = 5.04442073643383265887E-1;
            gn = gn * u + 1.97102833525523411709E-1;
            gn = gn * u + 1.87648584092575249293E-2;
            gn = gn * u + 6.84079380915393090172E-4;
            gn = gn * u + 1.15138826111884280931E-5;
            gn = gn * u + 9.82852443688422223854E-8;
            gn = gn * u + 4.45344415861750144738E-10;
            gn = gn * u + 1.08268041139020870318E-12;
            gn = gn * u + 1.37555460633261799868E-15;
            gn = gn * u + 8.36354435630677421531E-19;
            gn = gn * u + 1.86958710162783235106E-22;
            gd = 1.00000000000000000000E0;
            gd = gd * u + 1.47495759925128324529E0;
            gd = gd * u + 3.37748989120019970451E-1;
            gd = gd * u + 2.53603741420338795122E-2;
            gd = gd * u + 8.14679107184306179049E-4;
            gd = gd * u + 1.27545075667729118702E-5;
            gd = gd * u + 1.04314589657571990585E-7;
            gd = gd * u + 4.60680728146520428211E-10;
            gd = gd * u + 1.10273215066240270757E-12;
            gd = gd * u + 1.38796531259578871258E-15;
            gd = gd * u + 8.39158816283118707363E-19;
            gd = gd * u + 1.86958710162783236342E-22;
            f = 1 - u * fn / fd;
            g = t * gn / gd;
            
            float tt = Mathf.HalfPI * x2;
            
            float cc = (float)Math.Cos((float)t);
            float ss = (float)Math.Sin((float)t);
            tt = Mathf.HalfPI * x;
            
            c = (float)(0.5f + (f * ss - g * cc) / tt);
            s = (float)(0.5f - (f * cc + g * ss) / tt);
            c = c * Math.Sign(xxa);
            s = s * Math.Sign(xxa);
        }
#endif
    }
}


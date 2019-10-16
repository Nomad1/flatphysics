using System;
using FlatPhysics.Collision;
using NUnit.Framework;
using System.Numerics;

// ReSharper disable UseStringInterpolation

namespace FlatPhysics.NUnit
{
    [TestFixture]
    public class TestRaycast
    {
        // Point to RayCast test : Failed in find intr dot. fration = NaN
        [Test]
        public void TestPointRaycast()
        {
            // lines
            Vector2[][] lines =
            {
                //simple data
                new[] {new Vector2(-20.0f, 5.0f), new Vector2(-5.0f, 5.0f)},
                new[] {new Vector2(-15.0f, 25.0f), new Vector2(-15.0f, 10.0f)}
            };
            Vector2[] intrDot =
            {
                new Vector2(-15.0f, 5.0f),
                new Vector2(-15.0f, 20.0f)
            };
            // True fraction data
            float[] trueFraction = {0.3333333f, 0.3333333f};

            for (var obj = 0; obj < lines.Length; obj++)
            {
                PointShape pointShape = new PointShape(Vector2.Zero);
                Collision.Transform pointTranform =
                    new Collision.Transform(intrDot[obj], 34.654f * Mathf.Deg2Rad);

                float fraction;
                Vector2 normal;
                bool rayCast = pointShape.RayCast(pointTranform, lines[obj][0], lines[obj][1], out fraction,
                    out normal);
                Vector2 point = lines[obj][0] + (lines[obj][1] - lines[obj][0]) * fraction;

                Assert.That(rayCast, "RayCast to Point test failed. Result {0} is incorrect", obj);
                string err = String.Format(
                    "RayCast to Point test({0}) failed; Intersect: trueFraction {1} (testFraction: {2}), intrPoint({3}), testPoint {4}, normal {5}",
                    obj, trueFraction[obj], fraction, intrDot[obj], point, normal);
                Assert.That(Mathf.FloatEquals(fraction, trueFraction[obj], 0.005f), err);
            }
        }

        // RayCast to Circle test
        [Test]
        public void TestRaycastCircle()
        {
            // lines
            Vector2[][] lines =
            {
                new[] {new Vector2(5.023f, -5.024f), new Vector2(12.04f, -9.72f)},
                new[] {new Vector2(12.04f, -9.72f), new Vector2(5.023f, -5.024f)},
                new[] {new Vector2(20.0001f, -4.999f), new Vector2(15.100001f, -20.0202f)}
            };
            Vector2[] intrDot =
            {
                new Vector2(10.23f, -8.51f),
                new Vector2(10.23f, -8.51f),
                new Vector2(19.22f, -7.33f)
            };
            // True fraction data
            float[] trueFraction = {0.7421412f, 0.2578588f, 0.1555718f};

            //Create circle obj
            CircleShape circleShape = new CircleShape(Vector2.Zero, 5.02302f);
            Collision.Transform circleTransform =
                new Collision.Transform(new Vector2(15.05f, -10.06f), 33.34f * Mathf.Deg2Rad);

            for (var obj = 0; obj < lines.Length; obj++)
            {
                float fraction;
                Vector2 normal;
                bool rayCast = circleShape.RayCast(circleTransform, lines[obj][0], lines[obj][1], out fraction,
                    out normal);
                Vector2 point = lines[obj][0] + (lines[obj][1] - lines[obj][0]) * fraction;

                Assert.That(rayCast, "RayCast to Circle test failed. Result {0} is incorrect", obj);
                string err =
                    String.Format(
                        "RayCast to Circle test({0}) failed; Intersect: trueFraction {1} (testFraction: {2}), intrPoint({3}), testPoint {4}, normal {5}",
                        obj, trueFraction[obj], fraction, intrDot[obj], point, normal);
                Assert.That(Mathf.FloatEquals(fraction, trueFraction[obj], 0.005f), err);
            }
        }

        // RayCast to Rectangle test
        [Test]
        public void TestRaycastRectangle()
        {
            // lines
            Vector2[][] lines =
            {
                new[] {new Vector2(37.39518f, 27.87007f), new Vector2(32.02394f, 22.17816f)},
                new[] {new Vector2(32.02394f, 22.17816f), new Vector2(37.39518f, 27.87007f)},
                new[] {new Vector2(40.66259f, 1.94544f), new Vector2(5.58571f, 33.33002f)}
            };
            Vector2[] intrDot =
            {
                new Vector2(34.6901f, 25.0f),
                new Vector2(34.6901f, 25.0f),
                new Vector2(31.66f, 10.0013f)
            };
            // True fraction data
            float[] trueFraction = {0.5039476f, 0.4960526f, 0.256666f};
            //Create circle obj
            RectangleShape rectShape = new RectangleShape(Vector2.Zero, 30.0f, 15.0f);
            Collision.Transform rectTransform =
                new Collision.Transform(new Vector2(25.03758f, 17.48668f), 0 * Mathf.Deg2Rad);

            for (var obj = 0; obj < lines.Length; obj++)
            {
                float fraction;
                Vector2 normal;
                bool rayCast = rectShape.RayCast(rectTransform, lines[obj][0], lines[obj][1], out fraction,
                    out normal);
                Assert.That(rayCast, "RayCast to Rectangle test failed. Result {0} is incorrect", obj);
                var point = lines[obj][0] + (lines[obj][1] - lines[obj][0]) * fraction;
                string err =
                    String.Format(
                        "RayCast to Rectangle test failed; Intersect: trueFraction {0} (testFraction: {1}), intrPoint({2}), testPoint {3}, normal {4}",
                        trueFraction[obj], fraction, intrDot[obj], point, normal);
                Assert.That(Mathf.FloatEquals(fraction, trueFraction[obj], 0.005f), err);
            }
        }

        // RayCast to Polygon test
        [Test]
        public void TestRaycastPolygon()
        {
            // lines
            Vector2[][] lines =
            {
                new[] {new Vector2(-20.02394f, -5.17816f), new Vector2(-15.39518f, -10.87007f)},
                new[] {new Vector2(-15.39518f, -10.87007f), new Vector2(-20.02394f, -5.17816f)},
                new[] {new Vector2(-5.008571f, -5.03002f), new Vector2(-15.0059f, -25.00544f)}
            };
            Vector2[] intrDot =
            {
                new Vector2(-17.85f, -7.86f),
                new Vector2(-17.85f, -7.86f),
                new Vector2(-6.6701f, -8.35f)
            };
            Vector2[] polPoints =
            {
                new Vector2(-20.001f, -10.0002f), new Vector2(-15.001f, -5.0234f),
                new Vector2(-10.0231f, -5.002f), new Vector2(-5.003f, -10.02234f),
                new Vector2(-5.10021f, -15.0091f), new Vector2(-10.000001f, -20.00002f),
                new Vector2(-15.10221f, -20.0056f), new Vector2(-20.0034f, -15.1234f)
            };
            // True fraction data
            float[] trueFraction = {0.4705673f, 0.5294338f, 0.1662021f};

            //Create circle obj
            PolygonShape polShape = new PolygonShape(polPoints);
            Collision.Transform polTransform =
                new Collision.Transform(new Vector2(0, 0), 0 * Mathf.Deg2Rad);

            for (var obj = 0; obj < lines.Length; obj++)
            {
                float fraction;
                Vector2 normal;
                bool rayCast = polShape.RayCast(polTransform, lines[obj][0], lines[obj][1], out fraction,
                    out normal);
                Assert.That(rayCast, "RayCast to Polygon test failed. Result {0} is incorrect", obj);
                Vector2 point = lines[obj][0] + (lines[obj][1] - lines[obj][0]) * fraction;
                string err =
                    String.Format(
                        "RayCast to Polygon test({0}) failed; Intersect: trueFraction {1} (testFraction: {2}), intrPoint({3}), testPoint {4}, normal {5}",
                        obj, trueFraction[obj], fraction, intrDot[obj], point, normal);
                Assert.That(Mathf.FloatEquals(fraction, trueFraction[obj], 0.005f), err);
            }
        }

        // RayCast to Complex test
        [Test]
        public void TestRaycastComplex()
        {
            // lines
            Vector2[][] lines =
            {
                new[] {new Vector2(5.01675f, 21.14647f), new Vector2(9.35574f, 23.23984f)},
                new[] {new Vector2(9.35574f, 23.23984f), new Vector2(5.01675f, 21.14647f)},
                new[] {new Vector2(4.6742f, 12.27819f), new Vector2(18.37627f, 18.32993f)}
            };
            Vector2[] intrDot =
            {
                new Vector2(7.66f, 22.42f),
                new Vector2(7.66f, 22.42f),
                new Vector2(7.16f, 13.38f)
            };

            // Polygon points
            Vector2[] rect =
            {
                new Vector2(6.0f, 8.0f), new Vector2(4.0f, 12.0f)
            };
            Vector2[] polPoints =
            {
                new Vector2(8.0f, 6.0f), new Vector2(14.0f, 6.0f),
                new Vector2(14.0f, 2.0f), new Vector2(8.0f, 2.0f)
            };
            //Create Rectangles
            RectangleShape rectShape = new RectangleShape(rect[0], rect[1].X, rect[1].Y);
            //Create Polygons
            PolygonShape polyShape = new PolygonShape(polPoints);
            //Create Complex obj
            IShape[] shapes = {rectShape, polyShape};
            ComplexShape compShape = new ComplexShape(shapes);
            Collision.Transform compTranform =
                new Collision.Transform(new Vector2(5.0456f, 8.1234543f), 23.023f * Mathf.Deg2Rad);
            Console.WriteLine(23.023f * Mathf.Deg2Rad);

            // True fraction data
            float[] trueFraction = {0.6090305f, 0.3909697f, 0.1815236f};

            for (var obj = 0; obj < lines.Length; obj++)
            {
                float fraction;
                Vector2 normal;
                bool rayCast = compShape.RayCast(compTranform, lines[obj][0], lines[obj][1], out fraction,
                    out normal);
                Assert.That(rayCast, "RayCast to Complex test({0}) failed.", obj);
                Vector2 point = lines[obj][0] + (lines[obj][1] - lines[obj][0]) * fraction;
                string err = String.Format(
                    "Intersect RayCast to Complex test({0}) failed; Intersect: trueFraction {1} (testFraction: {2}), intrPoint({3}), testPoint {4}, normal {5}",
                    obj, trueFraction[obj], fraction, intrDot[obj], point, normal);
                Assert.That(Mathf.FloatEquals(fraction, trueFraction[obj], 0.005f), err);
            }
        }
    }
}
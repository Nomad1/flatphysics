using System;
using FlatPhysics.Collision;
using NUnit.Framework;
using System.Numerics;

namespace FlatPhysics.NUnit
{
    [TestFixture]
    public class TestCollisions
    {
        //        Point to Point test
        [Test]
        public void TestPointPointCollision()
        {
            // true results: not touching, touching in zero, touching
            bool[] trueResults = {false, true, true};
            // Points coordinates for first Shape
            float[] firstX = {-4.0f, 0, 7.0f};
            float[] firstY = {4.0f, 0, -1.0f};
            // Points coordinates for second Shape
            float[] secondX = {2.0f, 0, 7.0f};
            float[] secondY = {2.0f, 0, -1.0f};

            //Create points and check collisions
            for (var obj = 0; obj < trueResults.Length; obj++)
            {
                CollisionResult result;
                //Create point objects
                PointShape firstPointShape = new PointShape(Vector2.Zero);
                PointShape secondPointShape = new PointShape(Vector2.Zero);

                // Transform points
                Collision.Transform firstPointTransform =
                    new Collision.Transform(new Vector2(firstX[obj], firstY[obj]), 230 * Mathf.Deg2Rad);
                Collision.Transform secondCPointTransform =
                    new Collision.Transform(new Vector2(secondX[obj], secondY[obj]), 22 * Mathf.Deg2Rad);
                var testResult = firstPointShape.IntersectsWith(firstPointTransform, secondPointShape,
                    secondCPointTransform, out result);
                string err = String.Format("Point to Point test({0}) failed; Point1({1}, {2}) Point2 ({3}, {4})",
                    obj, firstX[obj], firstY[obj], secondX[obj], secondY[obj]);

                Assert.That(trueResults[obj] == testResult, err);
            }
        }

        // Point to Circle test
        [Test]
        public void TestPointCircleCollision()
        {
            // true results
            bool[] trueResults = {false, true, false, false, true};

            // Points coordinates
            float[] pointX = {-2.0f, 0, 2.0f, 2.0f, 0};

            float[] pointY = {-2.0f, 0, 2.0f, -2.0f, 2.0f};

            //Create circle obj
            CircleShape circleShape = new CircleShape(Vector2.Zero, 2.0f);

            // Transform circle to x=0, y=0, r=2
            Collision.Transform circleTransform = new Collision.Transform(Vector2.Zero, 0);

            // Create point obj
            PointShape point = new PointShape(Vector2.Zero);

            //Create points and check collisions
            for (var obj = 0; obj < trueResults.Length; obj++)
            {
                CollisionResult result;
                Collision.Transform pointTranform =
                    new Collision.Transform(new Vector2(pointX[obj], pointY[obj]), 0);
                var testResult = circleShape.IntersectsWith(circleTransform, point, pointTranform, out result);
                string err = String.Format("Point to Circle test({0}) failed; Circle:(0,0,2), Point({1}, {2})",
                    obj, pointX[obj], pointY[obj]);
                Assert.That(trueResults[obj] == testResult, err);
            }
        }

        // Point to Rectangle test
        [Test]
        public void TestPointRectangleCollision()
        {
            // true results
            bool[] trueResults = {true, true, true, true, false};
            // Points coordinates
            Vector2[] points =
            {
                new Vector2(-1.23f, 0.9834f),
                new Vector2(1.234f, 1.199f),
                new Vector2(1.00009f, -0.999f),
                new Vector2(-0.96001f, -0.86023f),
                new Vector2(234.432f, 99.66f)
            };
            float[] pointTurn = {234.12f, 12.234f, 11.11f, 66.66f, 111.111f};
            //Create circle obj
            RectangleShape rectShape = new RectangleShape(Vector2.Zero, 6.340010f, 4.020202f);
            Collision.Transform rectTransform =
                new Collision.Transform(new Vector2(-0.0005f, -0.345f), 1.34f * Mathf.Deg2Rad);

            //Create points and check collisions
            for (var obj = 0; obj < trueResults.Length; obj++)
            {
                CollisionResult result;

                PointShape pointShape = new PointShape(Vector2.Zero);
                Collision.Transform pointTranform =
                    new Collision.Transform(points[obj], pointTurn[obj] * Mathf.Deg2Rad);
                var testResult = rectShape.IntersectsWith(rectTransform, pointShape, pointTranform, out result);
                string err =
                    String.Format(
                        "Point to Rectangle test({0}) failed. Rect:(center=6.340010f, 4.020202f : WH= -0.0005f, " +
                        "-0.345f)1.34f°, Point({1})", obj, points[obj]);
                Assert.That(trueResults[obj] == testResult, err);
            }
        }

        // Point to Rectangle test
        [Test]
        public void TestPointPolygonCollision()
        {
            // true results
            bool[] trueResults = {true, true, true, true, false};
            // Points coordinates
            Vector2[] points =
            {
                new Vector2(-1.23f, 0.9834f),
                new Vector2(1.234f, 1.199f),
                new Vector2(1.00009f, -0.999f),
                new Vector2(-0.96001f, -0.86023f),
                new Vector2(234.432f, 99.66f)
            };
            float[] pointTurn = {234.12f, 12.234f, 11.11f, 66.66f, 111.111f};
            // Polygon points
            Vector2[] polPoints =
            {
                new Vector2(-15.30f, -4.4560f), new Vector2(-9.9910f, -5.25101f),
                new Vector2(-4.99f, -10.05f), new Vector2(-5.001f, -15.081f),
                new Vector2(-10.0333f, -19.8301f), new Vector2(-15.234f, -22.4598f),
                new Vector2(-20.90f, -15.0f), new Vector2(-20.0123f, -10.3398f)
            };
            //Create circle obj
            PolygonShape polShape = new PolygonShape(polPoints);
            Collision.Transform polTransform =
                new Collision.Transform(new Vector2(-0.0005f, 15.045f), 45.34f * Mathf.Deg2Rad);

            //Create points and check collisions
            for (var obj = 0; obj < trueResults.Length; obj++)
            {
                CollisionResult result;

                PointShape pointShape = new PointShape(Vector2.Zero);
                Collision.Transform pointTranform =
                    new Collision.Transform(points[obj], pointTurn[obj] * Mathf.Deg2Rad);
                var testResult = polShape.IntersectsWith(polTransform, pointShape, pointTranform, out result);
                string err = String.Format("Point to Polygon test({0}) failed. Polygon:()2.34f°, Point({1})", obj,
                    points[obj]);
                Assert.That(trueResults[obj] == testResult, err);
            }
        }

        // Point to Comples test
        [Test]
        public void TestPointComplexCollision()
        {
            // true results
            bool[] trueResults = {false, true, true, false};
            // Points coordinates
            Vector2[] points =
            {
                new Vector2(3.07563f, 25.33322f),
                new Vector2(6.80563f, 18.67249f),
                new Vector2(13.35218f, 16.38881f),
                new Vector2(17.34862f, 12.54462f)
            };
            float[] pointTurn = {234.12f, 12.234f, 11.11f, 66.66f, 111.111f};
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
            //Create Rectangle
            RectangleShape rectShape = new RectangleShape(rect[0], rect[1].X, rect[1].Y);
            //Create Polygon
            PolygonShape polShape = new PolygonShape(polPoints);
            //Create Complex obj
            IShape[] shapes = {rectShape, polShape};
            ComplexShape compShape = new ComplexShape(shapes);
            Collision.Transform compTranform =
                new Collision.Transform(new Vector2(5.0456f, 8.1234543f), 23.023f * Mathf.Deg2Rad);

            //Create points and check collisions
            for (var obj = 0; obj < trueResults.Length; obj++)
            {
                CollisionResult result;
                PointShape pointShape = new PointShape(Vector2.Zero);
                Collision.Transform pointTranform =
                    new Collision.Transform(points[obj], pointTurn[obj] * Mathf.Deg2Rad);
                var testResult = compShape.IntersectsWith(compTranform, pointShape, pointTranform, out result);
                string err = String.Format("Point to Complex test({0}) failed for Point({1})", obj, points[obj]);
                Assert.That(trueResults[obj] == testResult, err);
            }
        }

        // Circle to Circle test
        [Test]
        public void TestCircleCircleCollision()
        {
            // true results: not touching, intersection, touching
            bool[] trueResults = {false, true, true};
            // Points coordinates for first Shape
            float[] firstX = {-456.001f, 0, 7.00001f};
            float[] firstY = {1024.945f, 0, -1.00002f};
            float[] firstRad = {123.4056f, 2.0f, 2.0000f};
            float[] firstTurn = {36.554f, 137.2345f, 11.001f};
            // Points coordinates for second Shape
            float[] secondX = {23433.7f, 2.1234f, 11.00f};
            float[] secondY = {-90.12345f, -1.000001f, -1.00002f};
            float[] secondRad = {1.00001f, 2.0034f, 2.00001f};
            float[] secondTurn = {345.01f, 9.001f, 111.001f};


            //Create points and check collisions
            for (var obj = 0; obj < trueResults.Length; obj++)
            {
                CollisionResult result;
                //Create circle objects
                CircleShape firstCircleShape = new CircleShape(Vector2.Zero, firstRad[obj]);
                CircleShape secondCircleShape = new CircleShape(Vector2.Zero, secondRad[obj]);

                // Transform circles
                Collision.Transform firstCircleTransform =
                    new Collision.Transform(new Vector2(firstX[obj], firstY[obj]),
                        firstTurn[obj] * Mathf.Deg2Rad);
                Collision.Transform secondCircleTransform =
                    new Collision.Transform(new Vector2(secondX[obj], secondY[obj]),
                        secondTurn[obj] * Mathf.Deg2Rad);
                var testResult = firstCircleShape.IntersectsWith(firstCircleTransform, secondCircleShape,
                    secondCircleTransform, out result);
                string err = String.Format(
                    "Circle to Circle test({0}) failed; Circle1({1}, {2},{3}){4}° Circle2({5}, {6}, {7}){8}°", obj,
                    firstX[obj], firstY[obj], firstRad[obj], firstTurn[obj], secondX[obj], secondY[obj],
                    secondRad[obj], secondTurn[obj]);
                Assert.That(trueResults[obj] == testResult, err);
            }
        }

        // Rectangle to rectangle test
        [Test]
        public void TestRectRectCollision()
        {
            // true results: not touching, intersection, touching
            bool[] trueResults = {false, true, true};
            // First rectangle values
            Vector2[][] firstRec =
            {
                new[] {new Vector2(-590.333f, -23456.0f), new Vector2(99.99f, 1111.01f)},
                new[] {new Vector2(-73.999f, 60.001f), new Vector2(10.012f, 109.0002f)},
                new[] {new Vector2(0, 8.0f), new Vector2(4.0f, 2.0f)}
            };
            // Second rectangle values
            Vector2[][] secondRec =
            {
                new[] {new Vector2(1050.009f, 907.039f), new Vector2(2.00123f, 222.33f)},
                new[] {new Vector2(3.23f, 53.0101f), new Vector2(97.0301f, 3.99991f)},
                new[] {new Vector2(3.0f, 8.0f), new Vector2(2.0f, 2.0f)}
            };
            //Turns for first and second Shapes
            Vector2[] turnShape =
            {
                new Vector2(355.0f, 1.98765f),
                new Vector2(72.909f, 22.0001f),
                new Vector2(360.0f, 360.0f) //wery hard make touching by hand, so use 360°
            };

            for (var obj = 0; obj < trueResults.Length; obj++)
            {
                CollisionResult result;
                //Create rectangle objects
                RectangleShape firstRectShape =
                    new RectangleShape(Vector2.Zero, firstRec[obj][1].X, firstRec[obj][1].Y);
                RectangleShape secondRectShape = new RectangleShape(Vector2.Zero, secondRec[obj][1].X,
                    secondRec[obj][1].Y);
                // Transform Shapes
                Collision.Transform firstRectTransform =
                    new Collision.Transform(firstRec[obj][0], turnShape[obj].X * Mathf.Deg2Rad);
                Collision.Transform secondRectTransform =
                    new Collision.Transform(secondRec[obj][0], turnShape[obj].Y * Mathf.Deg2Rad);
                // Make test
                var testResult = firstRectShape.IntersectsWith(firstRectTransform, secondRectShape,
                    secondRectTransform, out result);

                string err = String.Format(
                    "Rectangle to Rectangle test({0}) failed; Rect1: center({1}), size({2}x{3}){4}° - Rect2: center ({5}), size({6}x{7}){8}°",
                    obj, firstRec[obj][0], firstRec[obj][1].X, firstRec[obj][1].Y, turnShape[obj].X,
                    secondRec[obj][0], secondRec[obj][1].X, secondRec[obj][1].Y, turnShape[obj].Y);
                Assert.That(trueResults[obj] == testResult, err);
            }
        }


        // Rectangle to rectangle single test
        [Test]
        public void SingleTestRectRectCollision()
        {
            Vector2[] firstPol =
            {
                new Vector2(-58.99589f, -180.19639f), new Vector2(-173.56437f, 156.44204f),
                new Vector2(-116.36934f, 143.46347f),
                new Vector2(85.68483f, 64.82733f), new Vector2(175.23662f, -90.592f)
            };
            Vector2[] secondPol =
            {
                new Vector2(-196.38893f, -132.43044f), new Vector2(43.23116f, 78.08958f),
                new Vector2(-65.40522f, -98.55061f)
            };

            CollisionResult result;
            //Create rectangle objects
            PolygonShape firstPolShape = new PolygonShape(firstPol);
            PolygonShape secondPolShape = new PolygonShape(secondPol);
            // Transform Shapes
            Collision.Transform firstPolTransform =
                new Collision.Transform(new Vector2(152.17828f, -197.50353f), 3.57942f);
            Collision.Transform secondPolTransform =
                new Collision.Transform(new Vector2(-55.65048f, -119.74799f), 4.94887f);
            // Make test
            var testResult = firstPolShape.IntersectsWith(firstPolTransform, secondPolShape,
                secondPolTransform, out result);

            Assert.That(testResult, "Error Kobzar");
            Console.Write("Point: {0} \n", result.Points[0]);
            Console.Write("Normal: {0}", result.Normals[0]);
        }


        // Polygon to Polygon test
        [Test]
        public void TestPolyPolyCollision()
        {
            // true results: not touching, intersection, touching
            bool[] trueResults = {false, true, true};
            // First Polygon values (triangle)
            Vector2[][] firstPol =
            {
                new[]
                {
                    new Vector2(-934.934f, 7222.009f), new Vector2(-1022.2f, 6909.892f),
                    new Vector2(-1200.29f, 1005.92f)
                },
                new[]
                {
                    new Vector2(99.29f, 302.924f), new Vector2(66.002f, 180.892f), new Vector2(210.09f, 103.92f),
                    new Vector2(333.09f, 111.2f), new Vector2(399.009f, 193.0f), new Vector2(410.0901f, 303.192f),
                    new Vector2(300.0f, 400.0f), new Vector2(190.099f, 347.9201f)
                },
                new[]
                {
                    new Vector2(1.0f, 1.0f), new Vector2(1.0f, 3.0f), new Vector2(3.0f, 3.0f),
                    new Vector2(3.0f, 1.0f)
                }
            };

            Vector2[][] secondPol =
            {
                new[]
                {
                    new Vector2(934.934f, -7222.009f), new Vector2(1022.2f, -6909.892f),
                    new Vector2(1200.29f, -1005.92f)
                },
                new[]
                {
                    new Vector2(45.001f, 450.92f), new Vector2(301.002f, 197.8924f), new Vector2(390.29f, 345.92f),
                    new Vector2(410.0123f, 499.9999f)
                },
                new[]
                {
                    new Vector2(3.0f, 3.0f), new Vector2(3.0f, 5.0f),
                    new Vector2(5.0f, 5.0f), new Vector2(5.0f, 3.0f)
                }
            };

            //Turns for first and second Shapes
            Vector2[] turnShape =
            {
                new Vector2(281.2345f, 101.765f),
                new Vector2(22.098f, 4.76f),
                new Vector2(360.0f, 360.0f) //wery hard make touching by hand, so use 360°
            };

            //Displacement for first and second Shapes
            Vector2[][] dispShape =
            {
                new[] {new Vector2(-1500.234f, -2345.76f), new Vector2(2111.2345f, 1111.765f)},
                new[] {new Vector2(220.0f, 330.0f), new Vector2(220.0f, 330.0f)},
                new[] {new Vector2(2.0f, 2.0f), new Vector2(2.0f, 2.0f)}
            };

            for (var obj = 0; obj < trueResults.Length; obj++)
            {
                CollisionResult result;
                // Create polygon objects
                PolygonShape firstPolShape = new PolygonShape(firstPol[obj]);
                PolygonShape secondPolShape = new PolygonShape(secondPol[obj]);
                // Transform Shapes
                Collision.Transform firstPolTransform =
                    new Collision.Transform(dispShape[obj][0], turnShape[obj].X * Mathf.Deg2Rad);
                Collision.Transform secondPolTransform =
                    new Collision.Transform(dispShape[obj][1], turnShape[obj].Y * Mathf.Deg2Rad);
                // Make test
                var testResult = firstPolShape.IntersectsWith(firstPolTransform, secondPolShape,
                    secondPolTransform, out result);
                string p1 = "";
                foreach (var points in firstPol[obj])
                {
                    p1 = p1 + points + ";";
                }
                string p2 = "";
                foreach (var points in secondPol[obj])
                {
                    p2 = p2 + points + ";";
                }

                string err =
                    String.Format("Polygon to Polygon test({0}) failed; Shape1:({1}){2}° Shape2:({3}) {4}°", obj,
                        p1, turnShape[obj].X, p2, turnShape[obj].Y);
                Assert.That(trueResults[obj] == testResult, err);
            }
        }

        // Circle to Complex test
        [Test]
        public void TestComplexComplexCollision()
        {
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
            RectangleShape firstRectShape = new RectangleShape(rect[0], rect[1].X, rect[1].Y);
            RectangleShape secondRectShape = new RectangleShape(rect[0], rect[1].X, rect[1].Y);
            //Create Polygons
            PolygonShape firstPolShape = new PolygonShape(polPoints);
            PolygonShape secondPolShape = new PolygonShape(polPoints);
            //Create Complex obj
            IShape[] firstComplex = {firstRectShape, firstPolShape};
            IShape[] secondComplex = {secondRectShape, secondPolShape};
            ComplexShape firstCompShape = new ComplexShape(firstComplex);
            ComplexShape secondCompShape = new ComplexShape(secondComplex);
            Collision.Transform firstCompTranform =
                new Collision.Transform(new Vector2(5.0456f, 8.1234543f), 23.023f * Mathf.Deg2Rad);
            Collision.Transform secondCompTranform =
                new Collision.Transform(new Vector2(5.0456f, 8.1234543f), 66.6666f * Mathf.Deg2Rad);

            CollisionResult result;
            var testResult = firstCompShape.IntersectsWith(firstCompTranform, secondCompShape, secondCompTranform,
                out result);
            string err = "Complex to Complex test failed.";
            Assert.That(testResult, err);
        }

        // Circle and rectangle test
        [Test]
        public void TestCircleRectangleCollision()
        {
            // Circles params: center coordiantes x,y ; radius, turn
            Vector2[][] circles =
            {
                new[] {new Vector2(5.0f, 5.0f), new Vector2(2.0f, 34.098f)},
                new[] {new Vector2(-6.0f, 6.0f), new Vector2(3.0f, 324.081f)},
                new[] {new Vector2(-8.0f, -7.0f), new Vector2(4.0f, 7.00098f)},
                new[] {new Vector2(10.0f, -8.0f), new Vector2(5.0f, 104.3398f)}
            };
            //Not intersect rectangle coordinates
            Vector2[][] freeRec =
            {
                new[] {new Vector2(2.023f, 3.00009f), new Vector2(130.0099f, 9900.019f)},
                new[] {new Vector2(102.999f, 60.0017f), new Vector2(-210.012f, 109.0002f)},
                new[] {new Vector2(534.001f, 12.0001f), new Vector2(-1024.99f, -634.091f)},
                new[] {new Vector2(2.09343f, 2.02321f), new Vector2(99.99f, 100.1f)}
            };
            // Intersect rectangle coordinates
            Vector2[][] intrRec =
            {
                new[] {new Vector2(6.0123f, 6.0321f), new Vector2(5.0001f, 5.0001f)},
                new[] {new Vector2(2.0009f, 2.00192f), new Vector2(-6.012f, 6.2202f)},
                new[] {new Vector2(7.013f, 1.980001f), new Vector2(-6.007f, -7.1230f)},
                new[] {new Vector2(4.0012f, 4.1001f), new Vector2(6.034f, -8.000009f)}
            };
            // Turns for first/second rect's
            Vector2[] turnRec =
            {
                new Vector2(27.304f, 0.0001f),
                new Vector2(124.303f, 66.6f),
                new Vector2(322.022f, 132.546f),
                new Vector2(66.6666f, 12.012f)
            };
            //Create circles and check collisions
            for (var obj = 0; obj < circles.Length; obj++)
            {
                //Work vals
                CollisionResult result;

                // Circle objects: circles[obj][0] - coords, circles[obj][1][0] - radius, circles[obj][1][1] - turn
                CircleShape circleShape = new CircleShape(Vector2.Zero, circles[obj][1].X);
                Collision.Transform circleTransform =
                    new Collision.Transform(circles[obj][0], circles[obj][1].Y * Mathf.Deg2Rad);

                //Create free rectangle objects: freeRec[obj][0] - width/heigth, freeRec[obj][1] - coordinates
                //Create intersect rectangle objects: intrRec[obj][0] - width/heigth, intrRec[obj][1] - coordinates
                // Turns for Shapes turnRec[obj][0-1]

                // Free Rect
                RectangleShape freeRectShape =
                    new RectangleShape(Vector2.Zero, freeRec[obj][0].X, freeRec[obj][0].Y);
                Collision.Transform freeRectTransform =
                    new Collision.Transform(freeRec[obj][1], turnRec[obj].X * Mathf.Deg2Rad);

                //Intersection Rect
                RectangleShape intrRectShape =
                    new RectangleShape(Vector2.Zero, intrRec[obj][0].X, intrRec[obj][0].Y);
                Collision.Transform intrdRectTransform =
                    new Collision.Transform(intrRec[obj][1], turnRec[obj].Y * Mathf.Deg2Rad);

                // Make free test
                var freeResult = circleShape.IntersectsWith(circleTransform, freeRectShape,
                    freeRectTransform, out result);
                // Make Intersection test
                var intrResult = circleShape.IntersectsWith(circleTransform, intrRectShape,
                    intrdRectTransform, out result);
                // Generate error messages

                string freeErr =
                    String.Format(
                        "Circle to Rectangle test failed; Circle({0})R:({1}) {2}° and freeRect({3} : {4}){5}°",
                        circles[obj][0], circles[obj][1].X, circles[obj][1].Y, freeRec[obj][0], freeRec[obj][1],
                        turnRec[obj].X);


                string intrErr = String.Format(
                    "Circle to Rectangle Interssept failed; Circle({0})R:({1}) {2}° and intrRect({3} : {4}){5}°",
                    circles[obj][0], circles[obj][1].X, circles[obj][1].Y, intrRec[obj][0], intrRec[obj][1],
                    turnRec[obj].Y);
                // Assert results
                Assert.That(freeResult == false, freeErr);
                Assert.That(intrResult, intrErr);
            }
        }

        // Circle and polygons test
        [Test]
        public void TestCirclePolygonCollision()
        {
            // Circles params: center coordiantes x,y ; radius, turn
            Vector2[][] circles =
            {
                new[] {new Vector2(5.0f, 5.0f), new Vector2(2.0f, 34.098f)},
                new[] {new Vector2(-6.0f, 6.0f), new Vector2(3.0f, 324.081f)},
                new[] {new Vector2(-8.0f, -7.0f), new Vector2(4.0f, 7.00098f)},
                new[] {new Vector2(10.0f, -8.0f), new Vector2(5.0f, 104.3398f)}
            };
            //Not intersect polygon coordinates
            Vector2[][] freePol =
            {
                new[]
                {
                    new Vector2(10.3123f, 12.5321f), new Vector2(13.1701f, 15.15001f),
                    new Vector2(16.76001f, 15.15001f), new Vector2(18.8001f, 13.5701f),
                    new Vector2(17.031f, 11.091f), new Vector2(13.1001f, 11.5001f)
                },
                new[]
                {
                    new Vector2(-13.3209f, 12.00192f), new Vector2(-17.812f, 12.8202f),
                    new Vector2(-16.4001f, 17.5001f)
                },
                new[]
                {
                    new Vector2(-15.013f, -14.980001f), new Vector2(-15.1001f, -18.5001f),
                    new Vector2(-18.1001f, -18.5001f), new Vector2(-18.007f, -14.1230f)
                },
                new[]
                {
                    new Vector2(20.0012f, -4.9001f), new Vector2(26.034f, -5.000009f),
                    new Vector2(23.1001f, -6.5001f), new Vector2(20.1001f, -7.5001f)
                }
            };
            // Intersect polygon coordinates
            Vector2[][] intrPol =
            {
                new[]
                {
                    new Vector2(1.03123f, 5.0321f), new Vector2(3.0701f, 7.99501f),
                    new Vector2(7.06001f, 8.25001f), new Vector2(8.95001f, 5.02701f),
                    new Vector2(7.031f, 2.1191f), new Vector2(2.93001f, 2.12901f)
                },
                new[]
                {
                    new Vector2(-4.0909f, 5.07192f), new Vector2(-8.0412f, 5.0202f),
                    new Vector2(-5.94001f, 8.0201f)
                },
                new[]
                {
                    new Vector2(-7.013f, -1.080001f), new Vector2(-4.1301f, -7.91001f),
                    new Vector2(-7.1001f, -13.95001f), new Vector2(-10.007f, -7.91230f)
                },
                new[]
                {
                    new Vector2(1.8812f, -7.8601f), new Vector2(9.951001f, -7.8001f),
                    new Vector2(10.1001f, -10.5001f), new Vector2(8.1601f, -11.85001f),
                    new Vector2(6.034f, -12.00009f)
                }
            };
            // Center coordiantes for free/intr polygons
            Vector2[][] polTrans =
            {
                new[] {new Vector2(140.234f, 2345.76f), new Vector2(0.0345f, 0.1065f)},
                new[] {new Vector2(-220.0f, 330.0f), new Vector2(0.0044f, 0.00765f)},
                new[] {new Vector2(-2222.0f, -1222.0f), new Vector2(0.10001f, 0.9089f)},
                new[] {new Vector2(544.0f, -998.0f), new Vector2(0.5544f, 0.77f)}
            };
            // Turns for free/intr polygons
            float[] freeTurn = {37.304f, 124.303f, 322.022f, 266.6666f};
            float[] intrTurn = {6.6001f, 6.6f, 32.546f, 4.012f};

            //Create circles and check collisions
            for (var obj = 0; obj < circles.Length; obj++)
            {
                //Work vals
                CollisionResult result;

                // Circle objects: circles[obj][0] - coords, circles[obj][1][0] - radius, circles[obj][1][1] - turn
                CircleShape circleShape = new CircleShape(Vector2.Zero, circles[obj][1].X);
                Collision.Transform circleTransform =
                    new Collision.Transform(circles[obj][0], circles[obj][1].Y * Mathf.Deg2Rad);

                //Create free polygons objects: freePol[obj] - coord, freeTurn[obj] - turn, polTrans[obj][0] - centr
                PolygonShape freePolShape = new PolygonShape(freePol[obj]);
                Collision.Transform freePolTransform =
                    new Collision.Transform(polTrans[obj][0], freeTurn[obj] * Mathf.Deg2Rad);

                //Create free polygons objects: intrPol[obj] - coor, freeTurn[obj] - turn, polTrans[obj][1] - centr
                PolygonShape intrPolShape = new PolygonShape(intrPol[obj]);
                Collision.Transform intrPolTransform =
                    new Collision.Transform(polTrans[obj][1], intrTurn[obj] * Mathf.Deg2Rad);

                // Make free test
                var freeResult =
                    circleShape.IntersectsWith(circleTransform, freePolShape, freePolTransform, out result);
                // Make Intersection test
                var intrResult =
                    circleShape.IntersectsWith(circleTransform, intrPolShape, intrPolTransform, out result);
                // Generate error messages
                string freeErr = String.Format("Not interssept for Circle to Polygon test({0}) failed", obj);
                string intrErr = String.Format("Interssept for Circle to Polygon test({0}) failed", obj);
                // Assert results
                Assert.That(freeResult == false, freeErr);
                Assert.That(intrResult, intrErr);
            }
        }

        // Circle to Complex test
        [Test]
        public void TestCircleComplexCollision()
        {
            // true results
            bool[] trueResults = {false, true, true, true};
            // Circles params: center coordiantes x,y ; radius, turn
            Vector2[][] circles =
            {
                new[] {new Vector2(-5.10756f, 25.02873f), new Vector2(3.234f, 34.098f)},
                new[] {new Vector2(4.14134f, 25.21903f), new Vector2(3.00123f, 324.081f)},
                new[] {new Vector2(10.11697f, 15.05666f), new Vector2(1.892340f, 7.00098f)},
                new[] {new Vector2(20.05097f, 18.25381f), new Vector2(4.1101f, 104.3398f)}
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
            //Create Rectangle
            RectangleShape rectShape = new RectangleShape(rect[0], rect[1].X, rect[1].Y);
            //Create Polygon
            PolygonShape polShape = new PolygonShape(polPoints);
            //Create Complex obj
            IShape[] shapes = {rectShape, polShape};
            ComplexShape compShape = new ComplexShape(shapes);
            Collision.Transform compTranform =
                new Collision.Transform(new Vector2(5.0456f, 8.1234543f), 23.023f * Mathf.Deg2Rad);

            //Create circles and check collisions
            for (var obj = 0; obj < trueResults.Length; obj++)
            {
                CollisionResult result;
                CircleShape circleShape = new CircleShape(Vector2.Zero, circles[obj][1].X);
                Collision.Transform circleTransform =
                    new Collision.Transform(circles[obj][0], circles[obj][1].Y * Mathf.Deg2Rad);
                var testResult = compShape.IntersectsWith(compTranform, circleShape, circleTransform, out result);
                string err = String.Format("Point to Complex test({0}) failed.", obj);
                Assert.That(trueResults[obj] == testResult, err);
            }
        }
    }
}
using System;
using FlatPhysics.Collision;
using NUnit.Framework;
using System.Numerics;

namespace FlatPhysics.NUnit
{
    // Circles class
    public class CirclesData
    {
        public readonly Vector2 Center;
        public readonly float Radius;
        public readonly Vector2 Trans;
        public readonly float Rotate;

        public CirclesData(float x, float y, float radius, float tx, float ty, float turn)
        {
            Center = new Vector2(x, y);
            Radius = radius;
            Trans = new Vector2(tx, ty);
            Rotate = turn;
        }
    }

    // Rects class
    public class RectanglesData
    {
        public readonly Vector2 Center;
        public readonly float Width;
        public readonly float Height;
        public readonly Vector2 Trans;
        public readonly float Rotate;

        public RectanglesData(float x, float y, float width, float height, float tx, float ty, float turn)
        {
            Center = new Vector2(x, y);
            Width = width;
            Height = height;
            Trans = new Vector2(tx, ty);
            Rotate = turn;
        }
    }

    // Polygons class
    public class PolygonsData
    {
        public readonly Vector2[] Points;
        public readonly Vector2 Trans;
        public readonly float Rotate;

        public PolygonsData(float tx, float ty, float turn, params float[] points)
        {
            Trans = new Vector2(tx, ty);
            Rotate = turn;
            Points = new Vector2[points.Length / 2];
            for (int i = 0; i < Points.Length; i++)
            {
                Points[i] = new Vector2(points[i * 2], points[i * 2 + 1]);
            }
        }
    }

    //Compex shapes class
    public class ComplexesData
    {
        public readonly Vector2[][] PolPoints;
        public readonly Vector2 Trans;
        public readonly float Rotate;

        public ComplexesData(float tx, float ty, float turn, params float[][] polygons)
        {
            Trans = new Vector2(tx, ty);
            Rotate = turn;
            PolPoints = new Vector2[polygons.Length][];

            for (int polygon = 0; polygon < polygons.Length; polygon++)
            {
                float[] points = polygons[polygon];
                PolPoints[polygon] = new Vector2[points.Length / 2];

                for (int dot = 0; dot < points.Length / 2; dot++)
                {
                    PolPoints[polygon][dot] = new Vector2(points[dot * 2], points[dot * 2 + 1]);
                }
            }
        }
    }

    // Test results class
    public class TestResults
    {
        public readonly int FirstShape;
        public readonly int SecondShape;
        public readonly bool Result;

        public TestResults(int firstId, int secondId, bool result)
        {
            FirstShape = firstId;
            SecondShape = secondId;
            Result = result;
        }
    }



    [TestFixture]
    public class TestCollisionsDynamic
    {
        // Circle to Circle test
        [Test]
        public void TestCircleCircleCollision()
        {
            // Import data
            var circle = Circles.Circle;
            var results = CircleCircleTest.CircleCircleResults;

            for (int obj = 0; obj < results.Length; obj++)
            {
                CollisionResult result;
                //Create circle objects
                CircleShape firstCircle = new CircleShape(circle[results[obj].FirstShape].Center,
                    circle[results[obj].FirstShape].Radius);
                CircleShape secondCircle = new CircleShape(circle[results[obj].SecondShape].Center,
                    circle[results[obj].SecondShape].Radius);
                // Transform circles
                Collision.Transform firstCircleTransform =
                    new Collision.Transform(circle[results[obj].FirstShape].Trans,
                        circle[results[obj].FirstShape].Rotate);
                Collision.Transform secondCircleTransform =
                    new Collision.Transform(circle[results[obj].SecondShape].Trans,
                        circle[results[obj].SecondShape].Rotate);
                var testResult = firstCircle.IntersectsWith(firstCircleTransform, secondCircle,
                    secondCircleTransform, out result);
                string err = String.Format("Circle to Circle test({0}) failed; Circle1_ID: {1}, Circle2_ID: {2}",
                    obj, results[obj].FirstShape, results[obj].SecondShape);
                Assert.That(results[obj].Result == testResult, err);
            }
        }

        // Circle to rectangle test
        [Test]
        public void TestCircleRectangleCollision()
        {
            // Import data
            var circle = Circles.Circle;
            var rect = Rects.Rect;
            var results = CircleRectTest.CircleRectResults;

            for (int obj = 0; obj < results.Length; obj++)
            {
                CollisionResult result;
                //Create objects
                CircleShape circleShape = new CircleShape(circle[results[obj].FirstShape].Center,
                    circle[results[obj].FirstShape].Radius);
                Collision.Transform circleTransform =
                    new Collision.Transform(circle[results[obj].FirstShape].Trans,
                        circle[results[obj].FirstShape].Rotate);

                RectangleShape rectShape =
                    new RectangleShape(rect[results[obj].SecondShape].Center, rect[results[obj].SecondShape].Width,
                        rect[results[obj].SecondShape].Height);

                Collision.Transform rectTransform =
                    new Collision.Transform(rect[results[obj].SecondShape].Trans,
                        rect[results[obj].SecondShape].Rotate);

                var testResult = circleShape.IntersectsWith(circleTransform, rectShape,
                    rectTransform, out result);
                string err = String.Format(
                    "Circle to Rectangle test({0}) failed; Circle_ID: {1}, Rectangle_ID: {2}",
                    obj, results[obj].FirstShape, results[obj].SecondShape);
                Assert.That(results[obj].Result == testResult, err);
            }
        }

        // Circle to Polygon test
        [Test]
        public void TestCirclePolygonCollision()
        {
            // Import data
            var circle = Circles.Circle;
            var poly = Polygons.Poly;
            var results = CirclePolyTest.CirclePolyResults;

            for (int obj = 0; obj < results.Length; obj++)
            {
                CollisionResult result;
                //Create objects
                CircleShape circleShape = new CircleShape(circle[results[obj].FirstShape].Center,
                    circle[results[obj].FirstShape].Radius);
                Collision.Transform circleTransform =
                    new Collision.Transform(circle[results[obj].FirstShape].Trans,
                        circle[results[obj].FirstShape].Rotate);

                PolygonShape polShape = new PolygonShape(poly[results[obj].SecondShape].Points);
                Collision.Transform polTransform =
                    new Collision.Transform(poly[results[obj].SecondShape].Trans,
                        poly[results[obj].SecondShape].Rotate);

                var testResult = circleShape.IntersectsWith(circleTransform, polShape,
                    polTransform, out result);
                string err = String.Format(
                    "Circle to Polygon test({0}) failed; Circle_ID: {1}, Polygon_ID: {2}",
                    obj, results[obj].FirstShape, results[obj].SecondShape);
                Assert.That(results[obj].Result == testResult, err);
            }
        }


        // Rectangle to rectangle test
        [Test]
        public void TestRectangleRectangleCollision()
        {
            // Import data
            var rect = Rects.Rect;
            var results = RectRectTest.RectRectResults;

            for (int obj = 0; obj < results.Length; obj++)
            {
                CollisionResult result;
                //Create rectangle objects
                RectangleShape firstRect =
                    new RectangleShape(rect[results[obj].FirstShape].Center, rect[results[obj].FirstShape].Width,
                        rect[results[obj].FirstShape].Height);
                RectangleShape secondRect =
                    new RectangleShape(rect[results[obj].SecondShape].Center, rect[results[obj].SecondShape].Width,
                        rect[results[obj].SecondShape].Height);
                // Transform Rectangles
                Collision.Transform firstRectTransform =
                    new Collision.Transform(rect[results[obj].FirstShape].Trans,
                        rect[results[obj].FirstShape].Rotate);
                Collision.Transform secondRectTransform =
                    new Collision.Transform(rect[results[obj].SecondShape].Trans,
                        rect[results[obj].SecondShape].Rotate);
                var testResult = firstRect.IntersectsWith(firstRectTransform, secondRect,
                    secondRectTransform, out result);
                string err = String.Format(
                    "Rectangle to Rectangle test({0}) failed; Rectangle({1}), Rectangle({2})",
                    obj, results[obj].FirstShape, results[obj].SecondShape);
                Assert.That(results[obj].Result == testResult, err);
            }
        }

        // Polygon to Polygon test
        [Test]
        public void TestPolygonPolygonCollision()
        {
            // Import data
            var poly = Polygons.Poly;
            var results = PolyPolyTest.PolyPolyResults;

            for (int obj = 0; obj < results.Length; obj++)
            {
                CollisionResult result;
                // Create polygon objects
                PolygonShape firstPol = new PolygonShape(poly[results[obj].FirstShape].Points);
                PolygonShape secondPol = new PolygonShape(poly[results[obj].SecondShape].Points);

                // Transform polygons
                Collision.Transform firstPolTransform =
                    new Collision.Transform(poly[results[obj].FirstShape].Trans,
                        poly[results[obj].FirstShape].Rotate);
                Collision.Transform secondPolTransform =
                    new Collision.Transform(poly[results[obj].SecondShape].Trans,
                        poly[results[obj].SecondShape].Rotate);
                var testResult = firstPol.IntersectsWith(firstPolTransform, secondPol,
                    secondPolTransform, out result);
                string err = String.Format("Polygon to Polygon test({0}) failed; Polygon({1}), Polygon({2})",
                    obj, results[obj].FirstShape, results[obj].SecondShape);
                Assert.That(results[obj].Result == testResult, err);
            }
        }


        // Rectangle to Polygon test
        [Test]
        public void TestRectanglePolygonCollision()
        {
            // Import data
            var rect = Rects.Rect;
            var poly = Polygons.Poly;
            var results = RectPolyTest.RectPolyResults;

            for (int obj = 0; obj < results.Length; obj++)
            {
                CollisionResult result;
                //Create objects
                RectangleShape rectShape =
                    new RectangleShape(rect[results[obj].FirstShape].Center, rect[results[obj].FirstShape].Width,
                        rect[results[obj].FirstShape].Height);
                Collision.Transform rectTransform =
                    new Collision.Transform(rect[results[obj].FirstShape].Trans,
                        rect[results[obj].FirstShape].Rotate);

                PolygonShape polShape = new PolygonShape(poly[results[obj].SecondShape].Points);
                Collision.Transform polTransform =
                    new Collision.Transform(poly[results[obj].SecondShape].Trans,
                        poly[results[obj].SecondShape].Rotate);


                var testResult = rectShape.IntersectsWith(rectTransform, polShape,
                    polTransform, out result);
                string err = String.Format(
                    "Rectangle to Polygon test({0}) failed; Rectangle_ID: {1}, Polygon_ID: {2}",
                    obj, results[obj].FirstShape, results[obj].SecondShape);
                Assert.That(results[obj].Result == testResult, err);
            }
        }


        // Complex to Complex test
        [Test]
        public void TestComplexComplexCollision()
        {
            // Import data
            var comp = Complexes.Comp;
            var results = CompCompTest.CompCompResults;

            for (int obj = 0; obj < results.Length; obj++)
            {
                IShape[] firstComplex = new IShape[comp[results[obj].FirstShape].PolPoints.Length];
                IShape[] secondComplex = new IShape[comp[results[obj].SecondShape].PolPoints.Length];

                for (int pol = 0; pol < comp[results[obj].FirstShape].PolPoints.Length; pol++)
                {
                    firstComplex[pol] = new PolygonShape(comp[results[obj].FirstShape].PolPoints[pol]);
                }

                for (int pol = 0; pol < comp[results[obj].SecondShape].PolPoints.Length; pol++)
                {
                    secondComplex[pol] = new PolygonShape(comp[results[obj].SecondShape].PolPoints[pol]);
                }
                // Create complex objects
                ComplexShape firstComp = new ComplexShape(firstComplex);
                ComplexShape secondComp = new ComplexShape(secondComplex);
                // Transform complex objects
                Collision.Transform firstCompTranform =
                    new Collision.Transform(comp[results[obj].FirstShape].Trans,
                        comp[results[obj].FirstShape].Rotate);
                Collision.Transform secondCompTranform =
                    new Collision.Transform(comp[results[obj].SecondShape].Trans,
                        comp[results[obj].SecondShape].Rotate);


                CollisionResult result;
                var testResult = firstComp.IntersectsWith(firstCompTranform, secondComp, secondCompTranform,
                    out result);
                string err = String.Format(
                    "Complex to Compex test({0}) failed; Complex({1}), Complex({2}) {3} {4}",
                    obj, results[obj].FirstShape, results[obj].SecondShape, comp[results[obj].FirstShape].Trans,
                    comp[results[obj].SecondShape].Rotate);

                Assert.That(results[obj].Result == testResult, err);
            }
        }

        // Complex to Circle test
        [Test]
        public void TestComplexCircleCollision()
        {
            // Import data
            var comp = Complexes.Comp;
            var circle = Circles.Circle;
            var results = CompCircleTest.CompCircleResults;

            for (int obj = 0; obj < results.Length; obj++)
            {
                IShape[] polygons = new IShape[comp[results[obj].FirstShape].PolPoints.Length];

                for (int pol = 0; pol < comp[results[obj].FirstShape].PolPoints.Length; pol++)
                {
                    polygons[pol] = new PolygonShape(comp[results[obj].FirstShape].PolPoints[pol]);
                }

                // Create complex objects
                ComplexShape compShape = new ComplexShape(polygons);
                // Transform complex objects
                Collision.Transform firstCompTranform =
                    new Collision.Transform(comp[results[obj].FirstShape].Trans,
                        comp[results[obj].FirstShape].Rotate);
                // Create Circle object
                CircleShape circleShape = new CircleShape(circle[results[obj].SecondShape].Center,
                    circle[results[obj].SecondShape].Radius);
                // Transform Circle objext
                Collision.Transform circleShapeTransform =
                    new Collision.Transform(circle[results[obj].SecondShape].Trans,
                        circle[results[obj].SecondShape].Rotate);

                CollisionResult result;
                var testResult = compShape.IntersectsWith(firstCompTranform, circleShape, circleShapeTransform,
                    out result);
                string err = String.Format("Complex to Circle test({0}) failed; Complex({1}), Circle({2}), res:{3}",
                    obj, results[obj].FirstShape, results[obj].SecondShape, result);
                Assert.That(results[obj].Result == testResult, err);
            }
        }

        // Complex to Rectangle test
        [Test]
        public void TestComplexRectangleCollision()
        {
            // Import data
            var comp = Complexes.Comp;
            var rect = Rects.Rect;
            var results = CompRectTest.CompRectResults;

            for (int obj = 0; obj < results.Length; obj++)
            {
                IShape[] polygons = new IShape[comp[results[obj].FirstShape].PolPoints.Length];
                for (int pol = 0; pol < comp[results[obj].FirstShape].PolPoints.Length; pol++)
                {
                    polygons[pol] = new PolygonShape(comp[results[obj].FirstShape].PolPoints[pol]);
                }

                // Create complex objects
                ComplexShape compShape = new ComplexShape(polygons);
                // Transform complex objects
                Collision.Transform firstCompTranform =
                    new Collision.Transform(comp[results[obj].FirstShape].Trans,
                        comp[results[obj].FirstShape].Rotate);

                // Create Rectangle object
                RectangleShape rectShape =
                    new RectangleShape(rect[results[obj].SecondShape].Center, rect[results[obj].SecondShape].Width,
                        rect[results[obj].SecondShape].Height);
                // Transform Rectangle object
                Collision.Transform rectTransform =
                    new Collision.Transform(rect[results[obj].SecondShape].Trans,
                        rect[results[obj].SecondShape].Rotate);


                CollisionResult result;
                var testResult = compShape.IntersectsWith(firstCompTranform, rectShape, rectTransform,
                    out result);
                string err = String.Format("Complex to Rectangle test({0}) failed; Complex({1}), Rectangle({2})",
                    obj, results[obj].FirstShape, results[obj].SecondShape);
                Assert.That(results[obj].Result == testResult, err);
            }
        }

        // Complex to Polygon test
        [Test]
        public void TestComplexPolygonCollision()
        {
            // Import data
            var comp = Complexes.Comp;
            var poly = Polygons.Poly;
            var results = CompPolyTest.CompPolyResults;

            for (int obj = 0; obj < results.Length; obj++)
            {
                IShape[] polygons = new IShape[comp[results[obj].FirstShape].PolPoints.Length];

                for (int pol = 0; pol < comp[results[obj].FirstShape].PolPoints.Length; pol++)
                {
                    polygons[pol] = new PolygonShape(comp[results[obj].FirstShape].PolPoints[pol]);
                }

                // Create complex objects
                ComplexShape compShape = new ComplexShape(polygons);
                // Transform complex objects
                Collision.Transform firstCompTranform =
                    new Collision.Transform(comp[results[obj].FirstShape].Trans,
                        comp[results[obj].FirstShape].Rotate);

                // Create polygon objects
                PolygonShape polShape = new PolygonShape(poly[results[obj].SecondShape].Points);
                // Transform polygon object
                Collision.Transform polTransform =
                    new Collision.Transform(poly[results[obj].SecondShape].Trans,
                        poly[results[obj].SecondShape].Rotate);


                CollisionResult result;
                var testResult = compShape.IntersectsWith(firstCompTranform, polShape, polTransform,
                    out result);
                string err = String.Format("Complex to Polygon test({0}) failed; Complex({1}), Polygon({2})",
                    obj, results[obj].FirstShape, results[obj].SecondShape);
                Assert.That(results[obj].Result == testResult, err);
            }
        }
    }
}
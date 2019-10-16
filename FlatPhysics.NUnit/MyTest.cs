using System;
using FlatPhysics.Collision;
using NUnit.Framework;
using System.Numerics;


namespace FlatPhysics.NUnit
{
    // MyTest results class
    public class MyResults
    {
        public readonly int Shape;
        public readonly bool FirstResult;
        public readonly bool SecondResult;

        public MyResults(int shape, bool firstResult, bool secondResult)
        {
            Shape = shape;
            FirstResult = firstResult;
            SecondResult = secondResult;
        }
    }

    public static class MyCompCirce
    {
        public static readonly MyResults[] MyCompCirceResults =
        {
            new MyResults(0, false, false),
            new MyResults(1, false, false),
            new MyResults(2, false, false),
            new MyResults(3, false, false),
            new MyResults(4, false, false),
            new MyResults(5, false, false),
            new MyResults(6, false, false),
            new MyResults(7, false, false),
            new MyResults(8, false, false),
            new MyResults(9, false, false),
            new MyResults(10, false, false),
            new MyResults(11, false, false),
            new MyResults(12, false, false),
            new MyResults(13, false, false),
            new MyResults(14, false, false),
            new MyResults(15, false, false),
            new MyResults(16, false, false),
            new MyResults(17, false, false),
            new MyResults(18, false, true),
            new MyResults(19, false, true),
            new MyResults(20, false, true),
            new MyResults(21, false, true),
            new MyResults(22, false, true),
            new MyResults(23, true, true),
            new MyResults(24, true, true),
            new MyResults(25, true, true),
            new MyResults(26, true, true),
            new MyResults(27, true, true),
            new MyResults(28, true, true),
            new MyResults(29, true, true),
            new MyResults(30, false, false),
            new MyResults(31, false, false),
            new MyResults(32, false, false),
            new MyResults(33, false, false),
            new MyResults(34, false, false),
            new MyResults(35, false, false),
            new MyResults(36, false, false),
            new MyResults(37, false, false),
            new MyResults(38, false, false),
            new MyResults(39, false, false),
            new MyResults(40, false, false),
            new MyResults(41, false, false),
            new MyResults(42, false, false),
            new MyResults(43, false, false),
            new MyResults(44, false, false)

        };
    }

    [TestFixture]
    public class ManualTest
    {
        [Test]
        public void MyTest()
        {

            // first shape Polygons points
            Vector2 trans1 = new Vector2(4700f, 2307f);
            float turn1 = 0.672428f;

            Vector2[] fp1 =
            {
                new Vector2(350.5f, -115f), new Vector2(226.5f, -120f), new Vector2(125.5f, -119f),
                new Vector2(-73.5f, -102f), new Vector2(-353.5f, -18f), new Vector2(-259.5f, 121f),
                new Vector2(277.5f, 73f), new Vector2(351.5f, 6f)
            };

            Vector2[] fp2 =
            {
                new Vector2(346.5f, 54f), new Vector2(351.5f, 6f), new Vector2(277.5f, 73f)
            };

            Vector2[] fp3 =
            {
                new Vector2(-328.5f, 114f), new Vector2(-259.5f, 121f), new Vector2(-353.5f, -18f)
            };

            Vector2[] fp4 =
            {
                new Vector2(-353.5f, -44f), new Vector2(-353.5f, -18f), new Vector2(-73.5f, -102f),
                new Vector2(-225.5f, -76f)
            };

            // second shape Polygons points
            Vector2 trans2 = new Vector2(3604f, 1915f);
            float turn2 = 3.365334f;

            Vector2[] sp1 =
            {
                new Vector2(921f, 37f), new Vector2(878f, -125f), new Vector2(771f, -126f),
                new Vector2(33f, -56f), new Vector2(-526f, 101f), new Vector2(51f, 127f),
                new Vector2(510f, 101f), new Vector2(748.6665f, 69.6665f)
            };

            Vector2[] sp2 =
            {
                new Vector2(-920f, 40f), new Vector2(-746f, 73.6665f), new Vector2(-526f, 101f),
                new Vector2(33f, -56f), new Vector2(-826f, -127f), new Vector2(-900f, -79f)
            };

            Vector2[] sp3 =
            {
                new Vector2(304.6665f, 119f), new Vector2(510f, 101f), new Vector2(51f, 127f)
            };

            Vector2[] sp4 =
            {
                new Vector2(-526f, 101f), new Vector2(-289f, 119f), new Vector2(51f, 127f)
            };


            //Create first polygons
            PolygonShape fpol1 = new PolygonShape(fp1);
            PolygonShape fpol2 = new PolygonShape(fp2);
            PolygonShape fpol3 = new PolygonShape(fp3);
            PolygonShape fpol4 = new PolygonShape(fp4);

            //Create second polygons
            PolygonShape spol1 = new PolygonShape(sp1);
            PolygonShape spol2 = new PolygonShape(sp2);
            PolygonShape spol3 = new PolygonShape(sp3);
            PolygonShape spol4 = new PolygonShape(sp4);


            // Create complex arrays
            IShape[] firstComplex = {fpol1, fpol2, fpol3, fpol4};
//            IShape[] secondComplex = {spol1, spol2, spol3, spol4};
            IShape[] secondComplex = {spol1, spol2};
            ComplexShape firstCompShape = new ComplexShape(firstComplex);
            Collision.Transform firstCompTranform = new Collision.Transform(trans1, turn1);

            ComplexShape secondCompShape = new ComplexShape(secondComplex);
            Collision.Transform secondCompTranform = new Collision.Transform(trans2, turn2);

            // Circles
            Vector2[] circles =
            {
                new Vector2(4200f, 1880f), new Vector2(4260f, 1910f), new Vector2(4310f, 1930f),
                new Vector2(4370f, 1960f), new Vector2(4430f, 1990f), new Vector2(4500f, 2010f),
                new Vector2(4550f, 2040f), new Vector2(4610f, 2060f), new Vector2(4660f, 2090f),
                new Vector2(4710f, 2050f), new Vector2(4650f, 2020f), new Vector2(4600f, 1980f),
                new Vector2(4540f, 1960f), new Vector2(4460f, 1940f), new Vector2(4400f, 1910f),
                new Vector2(4350f, 1880f), new Vector2(4290f, 1870f), new Vector2(4240f, 1830f),
                new Vector2(4160f, 1970f), new Vector2(4220f, 1990f), new Vector2(4280f, 2010f),
                new Vector2(4330f, 2030f), new Vector2(4390f, 2050f), new Vector2(4450f, 2080f),
                new Vector2(4470f, 2120f), new Vector2(4520f, 2130f), new Vector2(4530f, 2110f),
                new Vector2(4400f, 2180f), new Vector2(4480f, 2080f), new Vector2(4520f, 2090f),
                new Vector2(4210f, 1710f), new Vector2(4310f, 1770f), new Vector2(4410f, 1820f),
                new Vector2(4510f, 1860f), new Vector2(4600f, 1900f), new Vector2(4660f, 1920f),
                new Vector2(4750f, 1960f), new Vector2(4830f, 2000f), new Vector2(4290f, 1640f),
                new Vector2(4400f, 1700f), new Vector2(4490f, 1750f), new Vector2(4600f, 1790f),
                new Vector2(4680f, 1840f), new Vector2(4810f, 1890f), new Vector2(4900f, 1950f)
            };


            CollisionResult result;
            var testResult = firstCompShape.IntersectsWith(firstCompTranform, secondCompShape, secondCompTranform,
                out result);
//            Console.WriteLine("Test is: {0}", testResult);
            Assert.That(testResult, "Complex to Complex test failed.(Block_8 + block_3)");

            //Circles to Complexes tests
            var results = MyCompCirce.MyCompCirceResults;
            //Radius for circles
            float radius = 50.0f;


            for (int obj = 0; obj < results.Length; obj++)
            {
                CircleShape circleShape = new CircleShape(Vector2.Zero, radius);
                Collision.Transform circleTranform = new Collision.Transform(circles[obj], 0);
                //First complex text
                var firstResult = firstCompShape.IntersectsWith(firstCompTranform, circleShape, circleTranform,
                    out result);
                Assert.That(results[obj].FirstResult == firstResult, "Circle: {0} to Complex: 0 test failed.",
                    obj);
                //second complex text
                var secondResult = secondCompShape.IntersectsWith(secondCompTranform, circleShape, circleTranform,
                    out result);
                Assert.That(results[obj].SecondResult == secondResult, "Circle: {0} to Complex: 1 test failed.",
                    obj);
            }
        }

        // Circle and rectangle manual test
        [Test]
        public void ManualTestCircleRectangleCollision()
        {
            CollisionResult result;

            CircleShape circleShape = new CircleShape(Vector2.Zero, 1.2f);
            Collision.Transform circleTransform =
                new Collision.Transform(new Vector2(110.7501f, 8.56217f), 0.05750136f);

            RectangleShape rectShape =
                new RectangleShape(Vector2.Zero, 0.1f, 60f);
            Collision.Transform rectTransform =
                new Collision.Transform(new Vector2(94.20413f, 7.581524f), 0.08616392f);

            var res = circleShape.IntersectsWith(circleTransform, rectShape, rectTransform, out result);

//            Console.WriteLine(res);
            Assert.That(res == false, "Error on ManualTestCircleRectangleCollision. Must be false but got True");
        }
    }
}
using NUnit.Framework;
using System;
using FlatPhysics.Collision;
using System.Numerics;

namespace FlatPhysics.NUnit
{
    [TestFixture]
    public class AABBTest
    {
        public static bool VectorEquals(Vector2 v1, Vector2 v2)
        {
            return v1.ToString() == v2.ToString();
            //Mathf.FloatEquals(v1.X, v2.X) && Mathf.FloatEquals(v1.Y, v2.Y);
        }

        [Test]
        public void TestRectAABB()
        {
            RectangleShape rectShape = new RectangleShape(new Vector2(0, 20.0f), 29, 3);
            Collision.Transform rectTransform = new Collision.Transform(new Vector2(-1709.34f, 402.0f), 1.2456f);


            Vector2 lb;
            Vector2 ub;
            rectShape.GetAABB(rectTransform, out lb, out ub);

            Console.WriteLine("Rectangle LB: " + lb);
            Assert.That(VectorEquals(lb, new Vector2(-1734.346f, 394.1706f)), "RectangleShape.GetAABB() lower bound is incorrect");
            Console.WriteLine("Rectangle UB: " + ub);
            Assert.That(VectorEquals(ub, new Vector2(-1722.238f, 422.6092f)), "RectangleShape.GetAABB() upper bound is incorrect");
        }

        [Test]
        public void TestPolygonAABB()
        {
            PolygonShape polygonShape = new PolygonShape(new Vector2[]
            {
                new Vector2(-14.5f, 18.5f),
                new Vector2(14.5f, 18.5f),
                new Vector2(14.5f, 21.5f),
                new Vector2(-14.5f, 21.5f)
            }
                                                 );

            Collision.Transform polygonTransform = new Collision.Transform(new Vector2(-1709.34f, 402.0f), 1.2456f);


            Vector2 lb;
            Vector2 ub;
            polygonShape.GetAABB(polygonTransform, out lb, out ub);

            Console.WriteLine("Polygon LB: " + lb);
            Assert.That(VectorEquals(lb, new Vector2(-1734.346f, 394.1706f)), "PolygonShape.GetAABB() lower bound is incorrect");
            Console.WriteLine("Polygon UB: " + ub);
            Assert.That(VectorEquals(ub, new Vector2(-1722.238f, 422.6092f)), "PolygonShape.GetAABB() upper bound is incorrect");
        }

        [Test]
        public void TestCircleAABB()
        {
            CircleShape circleShape = new CircleShape(new Vector2(0, 20.0f), 14.577379f);

            Collision.Transform circleTransform = new Collision.Transform(new Vector2(-1709.34f, 402.0f), 1.2456f);

            Vector2 lb;
            Vector2 ub;
            circleShape.GetAABB(circleTransform, out lb, out ub);

            Console.WriteLine("Circle LB: " + lb);
            Assert.That(VectorEquals(lb, new Vector2(-1742.869f, 393.8125f)), "CircleShape.GetAABB() lower bound is incorrect");
            Console.WriteLine("Circle UB: " + ub);
            Assert.That(VectorEquals(ub, new Vector2(-1713.714f, 422.9673f)), "CircleShape.GetAABB() upper bound is incorrect");
        }

        [Test]
        public void TestPointAABB()
        {
            PointShape pointShape = new PointShape(new Vector2(0, 20.0f));

            Collision.Transform pointTransform = new Collision.Transform(new Vector2(-1709.34f, 402.0f), 1.2456f);

            Vector2 lb;
            Vector2 ub;
            pointShape.GetAABB(pointTransform, out lb, out ub);

            Console.WriteLine("Point LB: " + lb);
            Assert.That(VectorEquals(lb, new Vector2(-1728.292f, 408.3899f)), "PointShape.GetAABB() lower bound is incorrect");
            Console.WriteLine("Point UB: " + ub);
            Assert.That(VectorEquals(ub, new Vector2(-1728.292f, 408.3899f)), "PointShape.GetAABB() upper bound is incorrect");
        }

        [Test]
        public void TestNoneAABB()
        {
            NoneShape noneShape = new NoneShape();

            Collision.Transform noneTransform = new Collision.Transform(new Vector2(-1709.34f, 402.0f), 1.2456f);

            Vector2 lb;
            Vector2 ub;
            noneShape.GetAABB(noneTransform, out lb, out ub);

            Console.WriteLine("None LB: " + lb);
            Assert.That(lb.LengthSquared() == 0, "NoneShape.GetAABB() lower bound is incorrect");
            Console.WriteLine("None UB: " + ub);
            Assert.That(lb.LengthSquared() == 0, "NoneShape.GetAABB() upper bound is incorrect");
        }

        [Test]
        public void TestComplexAABB()
        {
            ComplexShape complexShape = new ComplexShape(new IShape[]
            {
                new CircleShape(new Vector2(0, 20.0f), 14.577379f),
                new RectangleShape(new Vector2(0, 20.0f), 49, 3)
            });

            Collision.Transform complexTransform = new Collision.Transform(new Vector2(-1709.34f, 402.0f), 1.2456f);

            Vector2 lb;
            Vector2 ub;
            complexShape.GetAABB(complexTransform, out lb, out ub);

            Console.WriteLine("Complex LB: " + lb);
            Assert.That(VectorEquals(lb, new Vector2(-1742.869f, 384.6948f)), "ComplexShape.GetAABB() lower bound is incorrect");
            Console.WriteLine("Complex UB: " + ub);
            Assert.That(VectorEquals(ub, new Vector2(-1713.714f, 432.0851f)), "ComplexShape.GetAABB() upper bound is incorrect");
        }

    }
}

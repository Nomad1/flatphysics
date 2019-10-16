using FlatPhysics.Collision;
using NUnit.Framework;
using System.Numerics;

namespace FlatPhysics.NUnit
{
    [TestFixture]
    public class CollisionTest
    {
        [Test]
        public void TestPolygonCircleCollision()
        {
            PolygonShape polyShape = new PolygonShape(new Vector2[]
            {
                new Vector2(2.291772f, 0.6106892f),
                new Vector2(2.80672f, 1.035656f),
                new Vector2(3.09072f, 1.088906f),
                new Vector2(3.436845f, 0.9380314f),
                new Vector2(3.556658f, 0.649594f),
                new Vector2(3.525595f, 0.383344f),
                new Vector2(3.290365f, 0.03150498f),
                new Vector2(2.9109f, -0.288045f)
            });
            
            Collision.Transform polyTransform = new Collision.Transform(new Vector2(10.47f, -5.44f), -1.349715f);
        
            CircleShape circleShape = new CircleShape(new Vector2(0, 0), 0.01f);
            Collision.Transform circleTransform = new Collision.Transform(new Vector2(11.51633f, -8.27487f), 5.145f);

            // circle and poly should collide
            CollisionResult result;
            Assert.That(polyShape.IntersectsWith(polyTransform, circleShape, circleTransform, out result), "PolygonCircle collision failed");
            
            // should not collide in this case
            Collision.Transform anotherCircleTransform = new Collision.Transform(new Vector2(15.51633f, -8.27487f), 5.145f);            
            Assert.That(!polyShape.IntersectsWith(polyTransform, circleShape, anotherCircleTransform, out result), "PolygonCircle collision failed");
        }


        [Test]
        public void TestCircleRectCollision()
        {
            RectangleShape rectShape = new RectangleShape(Vector2.Zero, 0.1f, 60.0f);

            Collision.Transform rectTransform = new Collision.Transform(new Vector2(94.20413f, 7.581524f), 0.08616392f);

            CircleShape circleShape = new CircleShape(new Vector2(0, 0), 1.2f);
            Collision.Transform circleTransform = new Collision.Transform(new Vector2(110.7501f, 8.56217f), 0.05750136f);

            // circle and rect should not collide
            CollisionResult result;
            Assert.That(!circleShape.IntersectsWith(circleTransform, rectShape, rectTransform, out result), "RectangleCircle collision failed");
        }
    }
}

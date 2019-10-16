using System;
using FlatPhysics.Collision;
using NUnit.Framework;
using System.Numerics;

namespace FlatPhysics.NUnit
{
    [TestFixture]
    public class RayCastTest
    {
        [Test]
        public void TestCircleRayCast()
        {
            CircleShape circleShape = new CircleShape(new Vector2(-3.0f, 6.0f), 4.15f);

            Collision.Transform circleTransform = new Collision.Transform(new Vector2(-1.05f, 0), 0.0f);

            Vector2 from = new Vector2(0, -20);
            Vector2 to = new Vector2(0, 6);


            float fraction;
            Vector2 normal;
            bool rayCast = circleShape.RayCast(circleTransform, from, to, out fraction, out normal);

            Assert.That(rayCast, "CircleShape.RayCast() result is incorrect");

            Assert.That(Mathf.FloatEquals(fraction, 0.9635197f, 0.005f), "CircleShape.RayCast() fraction is incorrect: " + fraction);

            Vector2 point = from + (to - from) * fraction;

            Console.WriteLine("Connection point is at fraction {0}, point {1}, normal {2}", fraction, point, normal);
        }
    }
}

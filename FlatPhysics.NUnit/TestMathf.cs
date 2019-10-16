using System;
using NUnit.Framework;
using System.Numerics;

namespace FlatPhysics.NUnit
{
    [TestFixture]
    public static class TestMathf
    {
        //Mathf.Sqrt() test
        [Test]
        public static void TestSqrt()
        {
            float[] fdata = { -1.0f, 0, 4.00f, 2347.93f, 287654.2344f, 99999.99f, 100.000f };
            foreach (float number in fdata)
            {
                float true_result = (float)Math.Sqrt(number);
                float test_result = Mathf.Sqrt(number);
                string err = String.Format("Sqrt test for value {0} failed. Must be: {1}, but got: {2}",
                    number, true_result, test_result);
                Assert.AreEqual(true_result, test_result, 0.05f, err);
            }
        }

        //Mathf.InvSqrt()
        [Test]
        public static void TestInvSqrt()
        {
            float[] fdata = { -1.0f, 0, 4.00f, 2347.93f, 987654.2344f, 99999.99f, 100.000f };
            foreach (float number in fdata)
            {
                float true_result = (float)(1.0 / Math.Sqrt(number));
                float test_result = Mathf.InvSqrt(number);
                string err = String.Format("InvSqrt test for value {0} failed. Must be: {1}, but got: {2}",
                                                                 number, true_result, test_result);
                Assert.AreEqual(true_result, test_result, 0.005f, err);
            }
        }

        //Mathf.Sin()
        [Test]
        public static void TestSin()
        {
            float[] fdata = { -1.0f, 0, 4.00f, 2347.93f, 987654.2344f, 99999.99f, 100.000f };
            foreach (float number in fdata)
            {
                float true_result = (float)System.Math.Sin(number);
                float test_result = Mathf.Sin(number);
                string err = String.Format("Sin test for value {0} failed. Must be: {1}, but got: {2}",
                                                         number, true_result, test_result);
                Assert.AreEqual(true_result, test_result, 0.05f, err);
            }
        }

        //Mathf.Cos()
        [Test]
        public static void TestCos()
        {
            float[] fdata = { -1.0f, 0, 4.00f, 2347.93f, 987654.2344f, 99999.99f, 100.000f };
            foreach (float number in fdata)
            {
                float true_result = (float)System.Math.Cos(number);
                float test_result = Mathf.Cos(number);
                string err = String.Format("Cos test for value {0} failed. Must be: {1}, but got: {2}",
                                                            number, true_result, test_result);
                Assert.AreEqual(true_result, test_result, 0.05f, err);
            }
        }

        //Mathf.Atan2()
        [Test]
        public static void TestAtan2()
        {
            Vector2[] data = new Vector2[]
            {
                //Y: X
                new Vector2(2.291772f, 0.6106892f),
                new Vector2(2.80672f, 1.035656f),
                new Vector2(3.09072f, 1.088906f),
                new Vector2(3.436845f, 0.9380314f),
                new Vector2(3.556658f, 0.649594f),
                new Vector2(3.525595f, 0.383344f),
                new Vector2(3.290365f, 0.03150498f),
                new Vector2(2.9109f, -0.288045f)

            };
            foreach (var item in data)
            {
                float true_result = (float)System.Math.Atan2(item.Y, item.X);
                float test_result = Mathf.Atan2(item.Y, item.X);
                string err = String.Format("Atan2 test for (Y={0}, X={1}) failed. Must be: {2}, but got: {3}",
                                                            item.Y, item.X, true_result, test_result);
                Assert.AreEqual(true_result, test_result, 0.005f, err);
            }

        }

        //Mathf.Exp()
        [Test]
        public static void TestExp()
        {
            float[] fdata = { -1.0f, 0, 4.00f, 2347.93f, 987654.2344f, 99999.99f, 100.000f };
            foreach (float number in fdata)
            {
                float true_result = (float)System.Math.Exp(number);
                float test_result = Mathf.Exp(number);
                string err = String.Format("Exp test for value {0} failed. Must be: {1}, but got: {2}",
                                                            number, true_result, test_result);
                Assert.AreEqual(true_result, test_result, 0.005f, err);
            }
        }

        //Mathf.Log()
        [Test]
        public static void TestLog()
        {
            float[] fdata = { -1.0f, 0, 4.00f, 2347.93f, 987654.2344f, 99999.99f, 100.000f };
            foreach (float number in fdata)
            {
                float true_result = (float)System.Math.Log(number);
                float test_result = Mathf.Log(number);
                string err = String.Format("Log test for value {0} failed. Must be: {1}, but got: {2}",
                                                         number, true_result, test_result);
                Assert.AreEqual(true_result, test_result, 0.005f, err);
            }
        }

        //Mathf.POw()
        [Test]
        public static void TestPow()
        {
            Vector2[] data = new Vector2[]
            {
                new Vector2(2.291772f, 0),
                new Vector2(0, 1.035656f),
                new Vector2(3.09072f, 1.088906f),
                new Vector2(3.436845f, 0.9380314f),
                new Vector2(3.290365f, 0.03150498f),
                new Vector2(2.9109f, -0.288045f)

            };
            foreach (var item in data)
            {
                float true_result = (float)System.Math.Pow(item.X, item.Y);
                float test_result = Mathf.Pow(item.X, item.Y);
                string err = String.Format("Pow test for (X={0}, Y={1}) failed. Must be: {2}, but got: {3}",
                                                            item.X, item.Y, true_result, test_result);
                Assert.AreEqual(true_result, test_result, 0.005f, err);
            }
        }
    }
}


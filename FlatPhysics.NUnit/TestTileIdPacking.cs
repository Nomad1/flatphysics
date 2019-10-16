#if false // not used anymore
using NUnit.Framework;
using System;
using FlatPhysics.Map;
using RunMobile.Utility;

namespace FlatPhysics.NUnit
{
    [TestFixture]
    public class TestTileIdPacking
    {
        [Test]
        public void TileIdPacking()
        {
            int[][] pairs =
            {
                new int [] { -123, 45},
                new int [] { 0, 0},
                new int [] { 2311140, -3140},
                new int [] { 65535, 65535},
                new int [] { -65535, -65535},
                new int [] { -1, 1},
                new int [] { 1, -1},
                new int [] { -1, -1},
                new int [] { 1, 1},
            };

            foreach (int[] pair in pairs)
            {
                ulong safeId = BaseMapInstance<IGuidable>.SafeGetTileId(pair[0], pair[1]);
                ulong id = BaseMapInstance<IGuidable>.GetTileId(pair[0], pair[1]);

                Console.WriteLine("Safe value is {0}, normal value is {1} for ({2};{3})", safeId, id, pair[0], pair[1]);
                Assert.AreEqual(safeId, id, "Tile id is not valid for ({0};{1})", pair[0], pair[1]); 
            }
        }
    }
}
#endif
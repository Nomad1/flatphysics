using System.Runtime.InteropServices;

namespace FlatPhysics.Map
{
    /// <summary>
    /// This helper class allows merging three 16-bit values to 64 bit ID without bitshifts and casts
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct IdUnion
    {
        [FieldOffset(0)]
        private readonly short X;
        [FieldOffset(2)]
        private readonly short Y;
        [FieldOffset(4)]
        private readonly short Z;
        [FieldOffset(0)]
        public readonly ulong Id;

        public IdUnion(int x, int y, int z)
            : this()
        {
            X = (short)x;
            Y = (short)y;
            Z = (short)z;
        }
    }
}


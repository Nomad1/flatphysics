#if RUNSERVER
using RunServer.Common;
#else
using System.Numerics;
#endif

namespace FlatPhysics.Collision
{
    /// <summary>
    /// Collision result structure. Contains contact points, normals, other needed data
    /// </summary>
    public struct CollisionResult
    {
        public readonly bool HaveCollision;

        public readonly Vector2[] Points;
        public readonly Vector2[] Normals;

        public CollisionResult(Vector2 point, Vector2 normal)
        {
            Points = new Vector2[] { point };
            Normals = new Vector2[] { normal };
            HaveCollision = true;
        }

        public CollisionResult(Vector2[] points, Vector2[] normals)
        {
            Points = points;
            Normals = normals;
            HaveCollision = true;
        }

        public void Invert()
        {
            for (int i = 0; i < Normals.Length; i++)
                Normals[i] = -Normals[i];
        }
    }
}

#if RUNSERVER
using RunServer.Common;
using RunServer.Common.Collections;
#else
using System.Numerics;
#endif

namespace FlatPhysics.Collision
{
    public class ComplexShape : BaseShape
    {
        private readonly IShape[] m_shapes;
        
        public IShape [] Shapes
        {
            get { return m_shapes; }
        }
        
        public ComplexShape(IShape[] shapes)
            : base (ShapeType.Complex)
        {
            m_shapes = shapes;

            float maxRadius = 0;

            foreach (IShape shape in shapes)
            {
                if (shape.BoundingRadius > maxRadius)
                    maxRadius = shape.BoundingRadius;
            }

            BoundingRadius = maxRadius;
        }

        public override void GetAABB(Transform ownTransform, out Vector2 min, out Vector2 max)
        {
            float minx = float.MaxValue;
            float miny = float.MaxValue;
            float maxx = float.MinValue;
            float maxy = float.MinValue;
            
            for (int i = 0; i < m_shapes.Length; i++)
            {
                m_shapes[i].GetAABB(ownTransform, out min, out max);
                
                if (min.X < minx)
                    minx = min.X;
                
                if (min.Y < miny)
                    miny = min.Y;
                
                if (max.X > maxx)
                    maxx = max.X;
                
                if (max.Y > maxy)
                    maxy = max.Y;
            }
            
            min = new Vector2(minx, miny);
            max = new Vector2(maxx, maxy);
        }
        
        public override bool RayCast(Transform ownTransform, Vector2 from, Vector2 to, out float fraction, out Vector2 normal)
        {
            fraction = float.MaxValue;
            normal = Vector2.Zero;
            bool found = false;

            foreach (IShape ownShape in m_shapes)
            {
                float shapeFraction;
                Vector2 shapeNormal;
                if (ownShape.RayCast(ownTransform, from, to, out shapeFraction, out shapeNormal))
                {
                    if (shapeFraction < fraction)
                    {
                        fraction = shapeFraction;
                        normal = shapeNormal;
                        found = true;
                    }
                }
            }
            
            if (!found) // no meaningful results, set fraction to 0 for backwards compatibility
            {
                fraction = 0;
                normal = Vector2.Zero;
            }
            
            return found;
        }
    }
}

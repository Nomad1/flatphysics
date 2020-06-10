#define RECT_AS_POLY
#if RUNSERVER
using RunServer.Common;
#else
using System.Numerics;
#endif

#if USE_MATHF
using Math = FlatPhysics.Mathf;
#else
using Math = System.Math;
#endif

using System.Collections.Generic;

namespace FlatPhysics.Collision
{
    public static class CollisionHelper
    {
        #region Collisions
        //This part knows nothing about shapes and works only with raw data

        internal static bool CollidePoints(Vector2 one, Transform oneTransform, Vector2 another, Transform anotherTransform, out CollisionResult result)
        {
            Vector2 onePoint = oneTransform.Mul(one);
            Vector2 anotherPoint = anotherTransform.Mul(another);

            Vector2 dir = onePoint - anotherPoint;
            float distanceSqrd = dir.LengthSquared();

            if (distanceSqrd > float.Epsilon)
            {
                result = default(CollisionResult);
                return false;
            }

#if !USE_MATHF
            dir /= (float)Math.Sqrt(distanceSqrd); // that function requires define and additional check
#else
            dir *= Math.InvSqrt(distanceSqrd);
#endif
            result = new CollisionResult(onePoint, dir);

            return true;
        }

        internal static bool CollideCircleAndPoint(Vector2 anotherCenter, float anotherRadius, Transform anotherTransform, Vector2 one, Transform oneTransform, out CollisionResult result)
        {
            Vector2 onePoint = oneTransform.Mul(one);
            Vector2 anotherPoint = anotherTransform.Mul(anotherCenter);

            Vector2 dir = anotherPoint - onePoint;
            float distanceSqrd = dir.LengthSquared();

            if (distanceSqrd > anotherRadius * anotherRadius)
            {
                result = default(CollisionResult);
                return false;
            }

            result = new CollisionResult(onePoint, dir); // we need to shift the point to the full collision distance

            return true;
        }

        internal static bool CollideCircles(Vector2 oneCenter, float oneRadius, Transform oneTransform, Vector2 anotherCenter, float anotherRadius, Transform anotherTransform, out CollisionResult result)
        {
            Vector2 onePoint = oneTransform.Mul(oneCenter);
            Vector2 anotherPoint = anotherTransform.Mul(anotherCenter);

            float radius = oneRadius + anotherRadius;

            Vector2 dir = anotherPoint - onePoint;
            float distanceSqrd = dir.LengthSquared();

            if (distanceSqrd > radius * radius)
            {
                result = default(CollisionResult);
                return false;
            }

            float distance = (float)Math.Sqrt(distanceSqrd);

            dir /= distance;

            float collisionLength = (oneRadius + anotherRadius - distance);

            Vector2 point = onePoint + dir * (oneRadius + collisionLength / 2); // center of the collision
            result = new CollisionResult(point, dir * collisionLength); // we need to shift one of circles to collision length distance

            return true;
        }

        internal static bool CollideRectangleAndPoint(Vector2 anotherCenter, float anotherWidth, float anotherHeight, Transform anotherTransform, Vector2 one, Transform oneTransform, out CollisionResult result)
        {
            Vector2 worldPoint = oneTransform.Mul(one);
            Vector2 anotherPoint = (anotherTransform.MulT(worldPoint) - anotherCenter) * 2; // note the * 2 here, that is done to use width/height and not width*0.5

            if (anotherPoint.X < -anotherWidth || anotherPoint.X > anotherWidth || anotherPoint.Y < -anotherHeight || anotherPoint.Y > anotherHeight)
            {
                result = default(CollisionResult);
                return false;
            }

            float[] distances =
            {
                Math.Abs(-anotherWidth - worldPoint.X), // left
                Math.Abs(anotherHeight - worldPoint.Y), // top
                Math.Abs(anotherWidth - worldPoint.X), // right
                Math.Abs(-anotherHeight - worldPoint.Y), // bottom
            };

            int index = -1;
            float dist = float.MaxValue;

            for (int i = 0; i < distances.Length; i++)
            {
                if (distances[i] < dist)
                {
                    dist = distances[i];
                    index = i;
                }
            }

            Vector2 normal = anotherTransform.MulR(RectangleShape.RectangleNormals[index]) * dist;

            result = new CollisionResult(worldPoint, normal);

            return true;
        }

        internal static bool CollideRectangleAndCircle(Vector2 anotherCenter, float anotherWidth, float anotherHeight, Transform anotherTransform, Vector2 oneCenter, float oneRadius, Transform oneTransform, out CollisionResult result)
        {
            Vector2 anotherPoint = anotherTransform.MulT(oneTransform.Mul(oneCenter)) - anotherCenter;

            result = default(CollisionResult);

            // resulting X and Y are invalid in some rare cases, that why we need to rewrite this method or use RECT_AS_POLY

            float x;
            {
                Vector2 axisX = new Vector2(1, 0);
                float minA = -anotherWidth / 2;
                float maxA = anotherWidth / 2;

                float pointA = Vector2.Dot(anotherPoint, axisX);
                float minB = pointA - oneRadius;
                float maxB = pointA + oneRadius;

                if (minA > maxB || maxA < minB)
                    return false;

                x = (Math.Max(minA, minB) + Math.Min(maxA, maxB)) / 2;
            }
            float y;
            {
                Vector2 axisY = new Vector2(0, 1);
                float minA = -anotherHeight / 2;
                float maxA = anotherHeight / 2;

                float pointA = Vector2.Dot(anotherPoint, axisY);
                float minB = pointA - oneRadius;
                float maxB = pointA + oneRadius;

                if (minA > maxB || maxA < minB)
                    return false;

                y = (Math.Max(minA, minB) + Math.Min(maxA, maxB)) / 2;

            }

            if (Vector2.DistanceSquared(anotherPoint, new Vector2(x, y)) > oneRadius * oneRadius)
                return false;

            float left = Math.Abs(-anotherWidth / 2 - x); // left
            float top = Math.Abs(anotherHeight / 2 - y); // top
            float right = Math.Abs(anotherWidth / 2 - x); // right
            float bottom = Math.Abs(-anotherHeight / 2 - y); // bottom

            int index = 0;
            float dist = left;

            if (top < dist)
            {
                dist = top;
                index = 1;
            }

            if (right < dist)
            {
                dist = right;
                index = 2;
            }

            if (bottom < dist)
            {
                dist = bottom;
                index = 3;
            }

            //for (int i = 0; i < distances.Length; i++)
            //{
            //    if (distances[i] < dist)
            //    {
            //        dist = distances[i];
            //        index = i;
            //    }
            //}

            Vector2 normal = anotherTransform.MulR(RectangleShape.RectangleNormals[index]) * dist;

            result = new CollisionResult(anotherTransform.Mul(new Vector2(x, y) + anotherCenter), normal);

            return true;
        }

        internal static bool CollidePolygonAndPoint(Vector2[] one, Vector2[] oneNormals, Transform oneTransform, Vector2 another, Transform anotherTransform, out CollisionResult result)
        {
            Vector2 worldPoint = anotherTransform.Mul(another);
            Vector2 anotherPoint = oneTransform.MulT(worldPoint);

            int index = -1;
            float dist = float.MaxValue;

            // Loop through all the edges
            for (int i = 0; i < oneNormals.Length; i++)
            {
                Vector2 normal = oneNormals[i];

                float pointA = Vector2.Dot(anotherPoint, normal);

                // Find the projection of the polygon on the current axis
                float minA = 0;
                float maxA = 0;
                ProjectPolygon(normal, one, out minA, out maxA);

                if (pointA < minA || pointA > maxA)
                {
                    result = default(CollisionResult);
                    return false;
                }

                float distance = Vector2.Dot(one[i] - anotherPoint, normal);
                if (distance < dist)
                {
                    dist = distance;
                    index = i;
                }
            }

            result = new CollisionResult(worldPoint, oneTransform.MulR(oneNormals[index]) * dist);

            return true;
        }

        internal static bool CollidePolygonAndCircle(Vector2[] one, Vector2[] oneNormals, Transform oneTransform, Vector2 anotherCenter, float anotherRadius, Transform anotherTransform, out CollisionResult result)
        {
            result = default(CollisionResult);

            Vector2 worldCenter = anotherTransform.Mul(anotherCenter);
            Vector2 anotherPoint = oneTransform.MulT(worldCenter);

#if OLD
            int index = -1;
            float dist = float.MaxValue;
            
            // Loop through all the edges
            for (int i = 0; i < oneNormals.Length; i++)
            {
                Vector2 normal = oneNormals[i];

                float pointA = Vector2.Dot(anotherPoint, normal);

                // Find the projection of the polygon on the current axis
                float minA = 0;
                float maxA = 0;
                ProjectPolygon(normal, one, out minA, out maxA);

                if (pointA + anotherRadius < minA || pointA - anotherRadius > maxA)
                    return false;

                float distance = Vector2.Dot(one[i] - anotherPoint, normal);

                if (-distance > anotherRadius)
                    return false;

                if (distance < dist)
                {
                    dist = distance;
                    index = i;
                }
            }

            float x;
            {
                Vector2 axisX = new Vector2(1, 0);
                float minA = 0;
                float maxA = 0;
                ProjectPolygon(axisX, one, out minA, out maxA);
                    
                float pointA = Vector2.Dot(anotherPoint, axisX);
                float minB = pointA - anotherRadius;
                float maxB = pointA + anotherRadius;
                    
                x = (Math.Max(minA, minB) + Math.Min(maxA, maxB)) / 2;
            }
            float y;
            {
                Vector2 axisY = new Vector2(0, 1);
                float minA = 0;
                float maxA = 0;
                ProjectPolygon(axisY, one, out minA, out maxA);

                float pointA = Vector2.Dot(anotherPoint, axisY);
                float minB = pointA - anotherRadius;
                float maxB = pointA + anotherRadius;

                y = (Math.Max(minA, minB) + Math.Min(maxA, maxB)) / 2;
            }
            
            Vector2 resultPoint = oneTransform.Mul(new Vector2(x, y));
            Vector2 resultNormal = oneTransform.MulR(oneNormals[index]);
            
            result = new CollisionResult(resultPoint, resultNormal);

            return true;
#elif true

            int index = -1;
            float dist = float.MaxValue;

            // Loop through all the edges
            for (int i = 0; i < oneNormals.Length; i++)
            {
                Vector2 normal = oneNormals[i];

                float pointA = Vector2.Dot(anotherPoint, normal);

                // Find the projection of the polygon on the current axis
                float minA = 0;
                float maxA = 0;
                ProjectPolygon(normal, one, out minA, out maxA);

                if (pointA + anotherRadius < minA || pointA - anotherRadius > maxA)
                    return false;

                float distance = Vector2.Dot(one[i] - anotherPoint, normal);

                if (-distance > anotherRadius)
                    return false;

                if (distance < dist)
                {
                    dist = distance;
                    index = i;
                }
            }

            // Vertices that subtend the incident face.

            int vertIndex1 = index;
            int vertIndex2 = (vertIndex1 + 1) % one.Length;
            Vector2 v1 = one[vertIndex1];
            Vector2 v2 = one[vertIndex2];

            // If the center is inside the polygon ...
            if (-dist < float.Epsilon)
            {
                Vector2 localNormal = oneNormals[index];
                Vector2 localPoint = (v1 + v2) / 2;

                Vector2 resultPoint = oneTransform.Mul(localPoint);
                Vector2 resultNormal = oneTransform.MulR(localNormal);

                result = new CollisionResult(resultPoint, resultNormal);

                return true;
            }

            // Compute barycentric coordinates
            float u1 = Vector2.Dot(anotherPoint - v1, v2 - v1);
            float u2 = Vector2.Dot(anotherPoint - v2, v1 - v2);

            if (u1 <= 0.0f)
            {
                float r = Vector2.DistanceSquared(anotherPoint, v1);
                if (r > anotherRadius * anotherRadius)
                {
                    return false;
                }

                Vector2 localNormal = (anotherPoint - v1) / r;

                Vector2 resultPoint = oneTransform.Mul(v1);
                Vector2 resultNormal = oneTransform.MulR(localNormal);// * Math.Sqrt(r);

                result = new CollisionResult(resultPoint, resultNormal);
            }
            else if (u2 <= 0.0f)
            {
                float r = Vector2.DistanceSquared(anotherPoint, v2);
                if (r > anotherRadius * anotherRadius)
                {
                    return false;
                }

                Vector2 localNormal = (anotherPoint - v2) / r;

                Vector2 resultPoint = oneTransform.Mul(v2);
                Vector2 resultNormal = oneTransform.MulR(localNormal);// * Math.Sqrt(r);

                result = new CollisionResult(resultPoint, resultNormal);
            }
            else
            {
                Vector2 faceCenter =
                    (v1 * u1 + v2 * u2) / (u1 + u2);

                //if (Vector2.DistanceSquared(anotherPoint, faceCenter) > anotherRadius * anotherRadius)
                //return false;

                float s = Vector2.Dot(oneNormals[vertIndex1], anotherPoint - faceCenter);

                if (s > anotherRadius)
                {
                    return false;
                }

                Vector2 localNormal = oneNormals[vertIndex1];

                Vector2 resultPoint = oneTransform.Mul(faceCenter);
                Vector2 resultNormal = oneTransform.MulR(localNormal);// * s;

                result = new CollisionResult(resultPoint, resultNormal);
            }

            return true;
#else
            // Compute circle position in the frame of the polygon.

            // Find the min separating edge.
            int index = 0;
            float separation = float.MinValue;
            float radius = anotherRadius + Mathf.Epsilon;
            int vertexCount = one.Length;

            for (int i = 0; i < vertexCount; ++i)
            {
                float distance = Vector2.Dot(oneNormals[i], anotherPoint - one[i]);

                if (distance > radius)
                {
                     //Early out.
                    return false;
                }

                if (distance > separation)
                {
                    separation = distance;
                    index = i;
                }
            }

            int vertIndex1 = index;
            int vertIndex2 = (vertIndex1 + 1) % one.Length;
            Vector2 v1 = one[vertIndex1];
            Vector2 v2 = one[vertIndex2];

            // If the center is inside the polygon ...
            if (separation < Mathf.Epsilon)
            {
                Vector2 localNormal = oneNormals[index];
                Vector2 localPoint = (v1 + v2) / 2;

                Vector2 resultPoint = oneTransform.Mul(localPoint);
                Vector2 resultNormal = oneTransform.MulR(localNormal);

                result = new CollisionResult(resultPoint, resultNormal);

                return true;
            }

            float u1 = Vector2.Dot(anotherPoint - v1, v2 - v1);
            float u2 = Vector2.Dot(anotherPoint - v2, v1 - v2);

            if (u1 <= 0.0f)
            {
                float r = Vector2.DistanceSquared(anotherPoint, v1);
                if (r > anotherRadius * anotherRadius)
                {
                    return false;
                }

                Vector2 localNormal = anotherPoint - v1;
                localNormal.Normalize();

                Vector2 resultPoint = oneTransform.Mul(v1);
                Vector2 resultNormal = oneTransform.MulR(localNormal);

                result = new CollisionResult(resultPoint, resultNormal);
            }
            else if (u2 <= 0.0f)
            {
                float r = Vector2.DistanceSquared(anotherPoint, v2);
                if (r > anotherRadius * anotherRadius)
                {
                    return false;
                }

                Vector2 localNormal = anotherPoint - v2;
                localNormal.Normalize();

                Vector2 resultPoint = oneTransform.Mul(v2);
                Vector2 resultNormal = oneTransform.MulR(localNormal);

                result = new CollisionResult(resultPoint, resultNormal);
            }
            else
            {
                Vector2 faceCenter = 
                    //(v1 + v2) / 2;
                    (v1 * u1 + v2 * u2) / (u1 + u2);

                if (Vector2.DistanceSquared(anotherPoint, faceCenter) > anotherRadius * anotherRadius)
                    return false;

                float s = Vector2.Dot(oneNormals[vertIndex1], anotherPoint - faceCenter);

                if (s > anotherRadius)
                {
                    return false;
                }

                Vector2 localNormal = oneNormals[vertIndex1];

                Vector2 resultPoint = oneTransform.Mul(faceCenter);
                Vector2 resultNormal = oneTransform.MulR(localNormal);

                result = new CollisionResult(resultPoint, resultNormal);
            }

            return true;
#endif
        }

        private static Vector2[] s_cachePoints = null;
        private static Vector2[] s_cacheNormals = null;

        internal static bool CollidePolygons(Vector2[] one, Vector2[] oneNormals, Transform oneTransform, Vector2[] nanotherPoints, Vector2[] nanotherNormals, Transform anotherTransform, out CollisionResult result)
        {
            result = default(CollisionResult);

            if (s_cachePoints == null || nanotherPoints.Length != s_cachePoints.Length)
            {
                s_cachePoints = new Vector2[nanotherPoints.Length];
                s_cacheNormals = new Vector2[nanotherNormals.Length];
            }

            Vector2[] anotherPoints = s_cachePoints;
            Vector2[] anotherNormals = s_cacheNormals;

            for (int i = 0; i < anotherPoints.Length; i++)
            {
                anotherPoints[i] = oneTransform.MulT(anotherTransform.Mul(nanotherPoints[i]));
                anotherNormals[i] = oneTransform.MulTR(anotherTransform.MulR(nanotherNormals[i]));
            }

            float minDistance = float.MaxValue;
            Vector2 minNormal = Vector2.Zero;

            // Loop through all the edges of both polygons
            for (int i = 0; i < oneNormals.Length; i++)
            {
                Vector2 normal = oneNormals[i];

                // Find the projection of the polygon on the current axis
                float minA;
                float maxA;
                ProjectPolygon(normal, one, out minA, out maxA);

                float minB;
                float maxB;
                ProjectPolygon(normal, anotherPoints, out minB, out maxB);

                if (minA > maxB || maxA < minB)
                    return false;

                float distance = Math.Max(minA, minB) - Math.Min(maxA, maxB);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    minNormal = normal;
                }
            }

            for (int i = 0; i < anotherNormals.Length; i++)
            {
                Vector2 normal = anotherNormals[i];

                // Find the projection of the polygon on the current axis
                float minA;
                float maxA;
                ProjectPolygon(normal, one, out minA, out maxA);

                float minB;
                float maxB;
                ProjectPolygon(normal, anotherPoints, out minB, out maxB);

                if (minA > maxB || maxA < minB)
                    return false;

                float distance = Math.Max(minA, minB) - Math.Min(maxA, maxB);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    minNormal = normal;
                }
            }

            float x;
            {
                Vector2 axisX = new Vector2(1, 0);
                float minA;
                float maxA;
                ProjectPolygon(axisX, one, out minA, out maxA);

                float minB;
                float maxB;
                ProjectPolygon(axisX, anotherPoints, out minB, out maxB);

                x = (Math.Max(minA, minB) + Math.Min(maxA, maxB)) / 2;
            }
            float y;
            {
                Vector2 axisY = new Vector2(0, 1);
                float minA;
                float maxA;
                ProjectPolygon(axisY, one, out minA, out maxA);

                float minB;
                float maxB;
                ProjectPolygon(axisY, anotherPoints, out minB, out maxB);

                y = (Math.Max(minA, minB) + Math.Min(maxA, maxB)) / 2;
            }

            // TODO: correct polygon-polygon collision result
            result = new CollisionResult(oneTransform.Mul(new Vector2(x, y)), oneTransform.MulR(minNormal));

            return true;
        }

        // Calculate the projection of a polygon on an axis
        // and returns it as a [min, max] interval
        private static void ProjectPolygon(Vector2 axis, Vector2[] polygonPoints, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;

            for (int i = 0; i < polygonPoints.Length; i++)
            {
                // To project a point on an axis use the dot product
                float dotProduct = Vector2.Dot(polygonPoints[i], axis);
                if (dotProduct < min)
                    min = dotProduct;

                if (dotProduct > max)
                    max = dotProduct;
            }
        }

#endregion

        // This functions take any pair of shapes and performs raw operations

        public static bool CheckIntersection(IShape one, Transform oneTransform, IShape another, Transform anotherTransform, out CollisionResult result)
        {
            // here goes the trick: we need to check most complex case first, so swap the shapes
            if ((int)one.ShapeType < (int)another.ShapeType)
            {
                bool cresult = CheckIntersection(another, anotherTransform, one, oneTransform, out result);

                if (cresult)
                    result.Invert();

                return cresult;
            }

            switch (one.ShapeType)
            {
                // top level shape needs to be checked against all other kinds of shapes
                case ShapeType.Complex:
                    {
                        List<CollisionResult> collisionList = new List<CollisionResult>(((ComplexShape)one).Shapes.Length);
                        int count = 0;

                        foreach (IShape ownShape in ((ComplexShape)one).Shapes)
                        {
                            if (CheckIntersection(ownShape, oneTransform, another, anotherTransform, out result))
                            {
                                collisionList.Add(result);
                                count += result.Points.Length;
                            }
                        }

                        if (count > 0)
                        {
                            List<Vector2> collisionPoints = new List<Vector2>(count);
                            List<Vector2> collisionNormals = new List<Vector2>(count);

                            foreach (CollisionResult collision in collisionList)
                            {
                                collisionPoints.AddRange(collision.Points);
                                collisionNormals.AddRange(collision.Normals);
                            }

                            result = new CollisionResult(collisionPoints.ToArray(), collisionNormals.ToArray());

                            return true;
                        }
                    }
                    break;

                // second level don't need to be checked against top level, so minus one check here
                case ShapeType.Polygon:
                    switch (another.ShapeType)
                    {
                        case ShapeType.Point:
                            return CollidePolygonAndPoint(((PolygonShape)one).Points, ((PolygonShape)one).Normals, oneTransform, ((PointShape)another).Position, anotherTransform, out result);
                        case ShapeType.Circle:
                            return CollidePolygonAndCircle(((PolygonShape)one).Points, ((PolygonShape)one).Normals, oneTransform, ((CircleShape)another).Center, ((CircleShape)another).Radius, anotherTransform, out result);
                        case ShapeType.Rectangle:
                            return CollidePolygons(((PolygonShape)one).Points, ((PolygonShape)one).Normals, oneTransform, ((RectangleShape)another).Points, ((RectangleShape)another).Normals, anotherTransform, out result);
                        case ShapeType.Polygon:
                            return CollidePolygons(((PolygonShape)one).Points, ((PolygonShape)one).Normals, oneTransform, ((PolygonShape)another).Points, ((PolygonShape)another).Normals, anotherTransform, out result);
                    }
                    break;

                // minus one check here
                case ShapeType.Rectangle:
                    switch (another.ShapeType)
                    {
                        case ShapeType.Point:
                            return CollideRectangleAndPoint(((RectangleShape)one).Center, ((RectangleShape)one).Width, ((RectangleShape)one).Height, oneTransform, ((PointShape)another).Position, anotherTransform, out result);
                        case ShapeType.Circle:
                            // rectangle can be represented as polygon with the same result, but pure rect-circle calculation is faster                    
#if RECT_AS_POLY
                            return CollidePolygonAndCircle(((RectangleShape)one).Points, ((RectangleShape)one).Normals, oneTransform, ((CircleShape)another).Center, ((CircleShape)another).Radius, anotherTransform, out result);
#else
                        return CollideRectangleAndCircle(((RectangleShape)one).Center, ((RectangleShape)one).Width, ((RectangleShape)one).Height, oneTransform, ((CircleShape)another).Center, ((CircleShape)another).Radius, anotherTransform, out result);
#endif
                        case ShapeType.Rectangle:
                            return CollidePolygons(((RectangleShape)one).Points, ((RectangleShape)one).Normals, oneTransform, ((RectangleShape)another).Points, ((RectangleShape)another).Normals, anotherTransform, out result);
                    }
                    break;

                // minus one check here
                case ShapeType.Circle:
                    switch (another.ShapeType)
                    {
                        case ShapeType.Point:
                            return CollideCircleAndPoint(((CircleShape)one).Center, ((CircleShape)one).Radius, oneTransform, ((PointShape)another).Position, anotherTransform, out result);
                        case ShapeType.Circle:
                            return CollideCircles(((CircleShape)one).Center, ((CircleShape)one).Radius, oneTransform, ((CircleShape)another).Center, ((CircleShape)another).Radius, anotherTransform, out result);
                    }
                    break;

                // Only one check left here - all other cases will be guaranteed checked above. I.e. Point/Polygon will become Polygon/Point
                case ShapeType.Point:
                    {
                        if (another.ShapeType == ShapeType.Point)
                            return CollidePoints(((PointShape)one).Position, oneTransform, ((PointShape)another).Position, anotherTransform, out result);
                    }
                    break;
            }

            result = default(CollisionResult);
            return false;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HFFlib
{
    public class Utils
    {
        /// <summary>
        /// </summary>
        /// <returns>the amount by which rectangle rect needs to be pushed to 
        /// no longer intersect the circle</returns>
        public static Vector2 Pushout(Circle circle, Rectangle rect)
        {

            Vector2 center = circle.Center;
            Vector2[] arr = new Vector2[] { rect.TopLeft, rect.TopRight, 
                rect.BottomRight, rect.BottomLeft };

            if((center.X < arr[0].X || center.X > arr[2].X) && 
                (center.Y < arr[0].Y || center.Y > arr[2].Y))
            {
                float[] distances = new float[] 
                { 
                    (center - arr[0]).LengthSquared(),
                    (center - arr[1]).LengthSquared(),
                    (center - arr[2]).LengthSquared(),
                    (center - arr[3]).LengthSquared()
                };
                int closestIndex = 0;
                for(int i = 1; i <= 3; i++)
                {
                    if(distances[closestIndex] > distances[i])
                    {
                        closestIndex = i;
                    }
                }
                Vector2 corner = arr[closestIndex];
                double atan2 = Math.Atan2(corner.Y - center.Y, corner.X - center.X);

                return new Vector2((float)(center.X + circle.Radius * Math.Cos(atan2)),
                    (float)(center.Y + circle.Radius * Math.Sin(atan2))) - corner;
            }
            else
            {
                return FlattenAlongLesserDimension(Pushout(circle.Bounds, rect));
            }
        }

        /// <summary> 
        /// </summary>
        /// <param name="rect1"></param>
        /// <param name="rect2"></param>
        /// <returns>the amount by which rectangle rect2 must be pushed out to 
        /// no longer intersect rectangle rect1</returns>
        public static Vector2 Pushout(Rectangle rect1, Rectangle rect2)
        {
            Vector2[] arr1 = new Vector2[] { rect1.TopLeft, rect1.TopRight,
                rect1.BottomRight, rect1.BottomLeft };
            Vector2[] arr2 = new Vector2[] { rect2.TopLeft, rect2.TopRight,
                rect2.BottomRight, rect2.BottomLeft };

            Vector2 center1 = rect1.Center;
            Vector2 center2 = rect2.Center;

            return new(center2.X < center1.X ? arr1[0].X - arr2[2].X : arr1[2].X - arr2[0].X,
                center2.Y < center1.Y ? arr1[0].Y - arr2[2].Y : arr1[2].Y - arr2[0].Y);
        }

        /// <summary> 
        /// </summary>
        /// <param name="tri"></param>
        /// <param name="circle"></param>
        /// <returns>the amount by which the circle must be pushed out to no 
        /// longer intersect triangle tri</returns>
        public static Vector2 Pushout(Triangle tri, Circle circle)
        {
            //tri info
            LineSegment hypoteneuse = tri.Hypoteneuse;
            float slope = hypoteneuse.Slope;
            float yIntercept = hypoteneuse.YIntercept;
            Vector2 pointA = hypoteneuse.PointA;
            Vector2 pointB = hypoteneuse.PointB;
            float r2 = circle.Radius * circle.Radius;
            float slopeReciprocal = 1 / slope;
            int emptyQuadrant = tri.EmptyQuadrant;
            Rectangle triBounds = tri.Bounds;
            Vector2 triBottomRight = triBounds.BottomRight;
            
            //circle info
            Vector2 center = circle.Center;
            Rectangle circleBounds = circle.Bounds;
            Vector2 circleBottomRight = circleBounds.BottomRight;

            Vector2 pushHypot;
            Vector2 pushCorner;
            Vector2 pushRect;

            //pushRect
            pushRect = -Pushout(circle, triBounds);

            Vector2 linePoint;
            Vector2 circlePoint;

            //pushHypot
            switch (emptyQuadrant)
            {
                case 1:
                    circlePoint = new(circle.X - (float)Math.Sqrt(r2 /
                (1 / (slope * slope) + 1)), circle.Y + (float)Math.Sqrt(r2 /
                (1 / (slopeReciprocal * slopeReciprocal) + 1)));
                    linePoint = new(circlePoint.X, slope * circlePoint.X + yIntercept);

                    pushHypot = new(0, linePoint.Y - circlePoint.Y);
                    break;

                case 2:
                    circlePoint = new(circle.X + (float)Math.Sqrt(r2 /
                (1 / (slope * slope) + 1)), circle.Y + (float)Math.Sqrt(r2 /
                (1 / (slopeReciprocal * slopeReciprocal) + 1)));
                    linePoint = new(circlePoint.X, slope * circlePoint.X + yIntercept);

                    pushHypot = new(0, linePoint.Y - circlePoint.Y);
                    break;

                case 3:
                    circlePoint = new(circle.X + (float)Math.Sqrt(r2 /
                (1 / (slope * slope) + 1)), circle.Y - (float)Math.Sqrt(r2 /
                (1 / (slopeReciprocal * slopeReciprocal) + 1)));
                    
                    linePoint = new((circlePoint.Y - yIntercept) * slopeReciprocal, circlePoint.Y);

                    pushHypot = new(linePoint.X - circlePoint.X, 0);
                    break;

                case 4:
                    circlePoint = new(circle.X - (float)Math.Sqrt(r2 /
                (1 / (slope * slope) + 1)), circle.Y - (float)Math.Sqrt(r2 /
                (1 / (slopeReciprocal * slopeReciprocal) + 1)));
                    linePoint = new((circlePoint.Y - yIntercept) * slopeReciprocal, circlePoint.Y);

                    pushHypot = new(linePoint.X - circlePoint.X, 0);
                    break;
                default:
                    throw new Exception("tri.EmptyQuadrant returned " + emptyQuadrant);
            }

            //case linePoint in Tri bounds
            if((emptyQuadrant == 1 || emptyQuadrant == 2) && triBounds.Contains(linePoint) ||
                (emptyQuadrant == 3 || emptyQuadrant == 4) && 
                triBounds.X < circlePoint.X && circlePoint.X < triBottomRight.X)
            {
                return pushHypot.LengthSquared() < pushRect.LengthSquared() ? pushHypot : pushRect;
            }
            //case linePoint out of Tri bounds
            else
            {
                //pushCorner
                Vector2 closestPoint = (pointA - center).LengthSquared() <
                    (pointB - center).LengthSquared() ? pointA : pointB;
                double atan2 = Math.Atan2(closestPoint.Y - center.Y, closestPoint.X - center.X);
                switch (emptyQuadrant)
                {
                    case 1:
                        if(closestPoint == pointA)
                        {
                            if (center.X > closestPoint.X)
                            {
                                //push up
                                pushCorner = new(0, closestPoint.Y -
                                    ((float)Math.Sqrt(r2 - Math.Pow(closestPoint.X - center.X, 2)) + center.Y));
                            }
                            else
                            {
                                //no pushdown allowed
                                pushCorner = closestPoint - new Vector2(
                                    (float)(center.X + circle.Radius * Math.Cos(atan2)),
                                    (float)(center.Y + circle.Radius * Math.Abs(Math.Sin(atan2))));
                            }
                        }
                        else
                        {
                            if (center.Y < closestPoint.Y)
                            {
                                //push up
                                pushCorner = new(0, closestPoint.Y -
                                    ((float)Math.Sqrt(r2 - Math.Pow(closestPoint.X - center.X, 2)) + center.Y));
                            }
                            else
                            {
                                //no pushleft allowed
                                pushCorner = closestPoint - new Vector2((float)
                                    (center.X + circle.Radius * -Math.Abs(Math.Cos(atan2))),
                                    (float)(center.Y + circle.Radius * Math.Sin(atan2)));
                            }
                        }
                        break;
                    case 2:
                        if (closestPoint == pointA)
                        {
                            if (center.Y > closestPoint.Y)
                            {
                                //push up
                                pushCorner = new(0, closestPoint.Y -
                                    ((float)Math.Sqrt(r2 - Math.Pow(closestPoint.X - center.X, 2)) + center.Y));
                            }
                            else
                            {
                                //no pushright allowed
                                pushCorner = closestPoint - new Vector2((float)
                                    (center.X + circle.Radius * Math.Abs(Math.Cos(atan2))),
                                    (float)(center.Y + circle.Radius * Math.Sin(atan2)));
                            }
                        }
                        else
                        {
                            if (center.X < closestPoint.X)
                            {
                                //push up
                                pushCorner = new(0, closestPoint.Y -
                                    ((float)Math.Sqrt(r2 - Math.Pow(closestPoint.X - center.X, 2)) + center.Y));
                            }
                            else
                            {
                                //no pushdown allowed
                                pushCorner = closestPoint - new Vector2(
                                    (float)(center.X + circle.Radius * Math.Cos(atan2)),
                                    (float)(center.Y + circle.Radius * Math.Abs(Math.Sin(atan2))));
                            }
                        }
                        break;
                    case 3:
                        if (closestPoint == pointA)
                        {
                            if (center.Y > closestPoint.Y)
                            {
                                //push left
                                pushCorner = new(closestPoint.X -
                                    ((float)Math.Sqrt(r2 - Math.Pow(closestPoint.Y - center.Y, 2)) + center.X), 0);
                            }
                            else
                            {
                                //no pushright allowed
                                pushCorner = closestPoint - new Vector2(
                                    (float)(center.X + circle.Radius * Math.Abs(Math.Cos(atan2))),
                                    (float)(center.Y + circle.Radius * Math.Sin(atan2)));
                            }
                        }
                        else
                        {
                            if (center.X < closestPoint.X)
                            {
                                //push left
                                pushCorner = new(closestPoint.X -
                                    ((float)Math.Sqrt(r2 - Math.Pow(closestPoint.Y - center.Y, 2)) + center.X), 0);
                            }
                            else
                            {
                                //no pushup allowed
                                pushCorner = closestPoint - new Vector2(
                                    (float)(center.X + circle.Radius * Math.Cos(atan2)),
                                    (float)(center.Y + circle.Radius * -Math.Abs(Math.Sin(atan2))));
                            }
                        }
                        break;
                    case 4:
                        if (closestPoint == pointA)
                        {
                            if (center.X > closestPoint.X)
                            {
                                //push right
                                pushCorner = new(closestPoint.X -
                                    ((float)-Math.Sqrt(r2 - Math.Pow(closestPoint.Y - center.Y, 2)) + center.X), 0);
                            }
                            else
                            {
                                //no pushup allowed
                                pushCorner = closestPoint - new Vector2(
                                    (float)(center.X + circle.Radius * Math.Cos(atan2)),
                                    (float)(center.Y + circle.Radius * -Math.Abs(Math.Sin(atan2))));
                            }
                        }
                        else
                        {
                            if (center.Y > closestPoint.Y)
                            {
                                //push right
                                pushCorner = new(closestPoint.X -
                                    ((float)-Math.Sqrt(r2 - Math.Pow(closestPoint.Y - center.Y, 2)) + center.X), 0);
                            }
                            else
                            {
                                //no pushleft allowed
                                pushCorner = closestPoint - new Vector2((float)
                                    (center.X + circle.Radius * -Math.Abs(Math.Cos(atan2))),
                                    (float)(center.Y + circle.Radius * Math.Sin(atan2)));
                            }
                        }
                        break;
                    default:
                        throw new Exception("tri.EmptyQuadrant returned " + emptyQuadrant);
                }


                return pushCorner.LengthSquared() < pushRect.LengthSquared() ? pushCorner : pushRect;
            }
        }

        /// <summary> 
        /// </summary>
        /// <param name="v"></param>
        /// <returns>A Vector2 flattened along its lesser dimension</returns>
        public static Vector2 FlattenAlongLesserDimension(Vector2 v)
        {
            return Math.Abs(v.X) < Math.Abs(v.Y) ? new(v.X, 0) : new(0, v.Y);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns>true if all items in arr are equal</returns>
        public static bool EqualValues<T>(T[] arr)
        {
            bool b = true;
            for(int i = 0; i < arr.Length - 1; i++)
            {
                b = b && arr[i].Equals(arr[i + 1]);
            }

            return false;
        }

        public static Rectangle AggregateBoundingBoxes(ICollection<IShape> shapes)
        {
            Vector2 minPoint = new(float.PositiveInfinity);
            Vector2 maxPoint = new(float.NegativeInfinity);

            foreach(IShape shape in shapes)
            {
                Rectangle bounds = shape.Bounds;
                minPoint = Vector2.Min(minPoint, bounds.TopLeft);
                maxPoint = Vector2.Max(maxPoint, bounds.BottomRight);
            }

            Vector2 dims = maxPoint - minPoint;

            return new(minPoint.X, minPoint.Y, dims.X, dims.Y);
        }

        public static Rectangle AggregateBoundingBoxes(ICollection<Hitbox> hitboxes)
        {
            Vector2 minPoint = new(float.PositiveInfinity);
            Vector2 maxPoint = new(float.NegativeInfinity);

            foreach (Hitbox shape in hitboxes)
            {
                Rectangle bounds = shape.GetBoundingBox();
                minPoint = Vector2.Min(minPoint, bounds.TopLeft);
                maxPoint = Vector2.Max(maxPoint, bounds.BottomRight);
            }

            Vector2 dims = maxPoint - minPoint;

            return new(minPoint.X, minPoint.Y, dims.X, dims.Y);
        }

        /// <summary>
        /// Push circle out of line segment at pushoutAngle. Note that ls slope can't equal pushoutAngle.
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="circle"></param>
        /// <param name="pushoutAngle"></param>
        /// <returns></returns>
        public static Vector2 Pushout(LineSegment ls, Circle circle, float pushoutAngle)
        {
            //data validation
            pushoutAngle %= 2 * (float)Math.PI;
            float slope = ls.Slope;
            
            //find pushPoint
            Vector2[] pushCandidates = PointsOfCircleAtSlope(circle, slope);
            Vector2 pushPoint;
            //push vertically
            if (slope == 0)
            {
                pushPoint = pushoutAngle < Math.PI ? pushCandidates[1] : pushCandidates[0];
            }
            //push horizontally
            else if(float.IsNaN(slope) || float.IsInfinity(slope))
            {
                pushPoint = pushoutAngle <= Math.PI / 2 || pushoutAngle >= Math.PI * 3 / 2 ?
                    new(circle.X - circle.Radius, circle.Y) :
                    new(circle.X + circle.Radius, circle.Y);
            }
            //push somewhat horizontally
            else
            {
                pushPoint = pushoutAngle < Math.PI / 2 || pushoutAngle >= Math.PI * 3 / 2 ?
                    pushCandidates[0] : pushCandidates[1];
            }

            //find pushPoint destination
            float pushoutSlope = -(float)Math.Tan(pushoutAngle);
            float b = ls.YIntercept;
            Vector2 pushPointDest;
            if (float.IsNaN(slope) || float.IsInfinity(slope))
            {
                b = pushPoint.Y - pushoutSlope * pushPoint.X;
                pushPointDest = new(ls.PointA.X, pushoutSlope + ls.PointA.X + b);
                pushPointDest.Y = pushPoint.Y;
            }
            else
            {
                pushPointDest = new((pushPoint.Y - pushoutSlope * pushPoint.X - b) / (slope - pushoutSlope));
                pushPointDest.Y = slope * pushPointDest.X + b;
            }

            //push along line at pushoutAngle
            if (ls.Bounds.Contains(pushPointDest))
            {
                return pushPointDest - pushPoint;
            }
            return Vector2.Zero;
        }

        public static Vector2[] PointsOfCircleAtSlope(Circle circle, float slope)
        {
            if(slope == 0)
            {
                return new Vector2[2]
                {
                    new(circle.X, circle.Y - circle.Radius),
                    new(circle.X, circle.Y + circle.Radius)
                };
            }
            else if(float.IsNaN(slope) || float.IsInfinity(slope))
            {
                return new Vector2[2]
                {
                    new(circle.X - circle.Radius, circle.Y),
                    new(circle.X + circle.Radius, circle.Y)
                };
            }

            float antislope = -1 / slope;
            float denom = (float)Math.Sqrt(1 + antislope * antislope);
            float dx = circle.Radius * 1 / denom;
            float dy = antislope * dx; //more efficient than antislope / denom
            return new Vector2[2] 
            { 
                new(circle.X - dx, circle.Y - dy), 
                new(circle.X + dx, circle.Y + dy) 
            };
        }

        //assumes counterclockwise
        public static bool AngleInBounds(float a1, float a2, float t)
        {
            Console.WriteLine("a1: " + a1 + ", a2: " + a2 + ", t: " + t);
            a1 = (float)((a1 + Math.PI * 2) % (Math.PI * 2));
            a2 = (float)((a2 + Math.PI * 2) % (Math.PI * 2));
            t = (float)((t + Math.PI * 2) % (Math.PI * 2));

            if (a2 < a1)
            {
                a2 += (float)Math.PI * 2;
                t += (float)Math.PI * 2;
            }

            Console.WriteLine("a1: " + a1 + ", a2: " + a2 + ", t: " + t);

            return a1 <= t && t <= a2;
        }
    }
}

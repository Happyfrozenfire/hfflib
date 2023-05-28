using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace HFFlib
{
    /// <summary>
    /// Only immutable structs should implement this interface. 
    /// IShape defines a shape with a bounding box, a center, and a clone function.
    /// 
    /// </summary>
    public interface IShape
    {
        /// <summary>
        /// Get the bounding box for this shape
        /// </summary>
        public abstract Rectangle Bounds { get; }
        /// <summary>
        /// Get the center for this shape
        /// </summary>
        public abstract Vector2 Center { get; }
        /// <summary>
        /// </summary>
        /// <param name="point"></param>
        /// <returns>true if the shape contains the point (exclusive)</returns>
        public abstract bool Contains(Vector2 point);
        /// <summary>
        /// </summary>
        /// <returns>An immutable copy of this shape as an IShape instance</returns>
        public abstract IShape CloneShape();
    }

    /// <summary>
    /// A rectangle whose top left point is its position, 
    /// and whose bottom right point is its position plus its dimensions.
    /// </summary>
    [Serializable]
    public struct Rectangle : IShape, ISerializable
    {
        /// <summary>
        /// Top left point of rectangle
        /// </summary>
        private Vector2 position;
        /// <summary>
        /// Dimensions of Rectangle
        /// </summary>
        private Vector2 dimensions;

        /// <summary>
        /// Constructor with x, y, width, and height
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width">Must be greater than 0</param>
        /// <param name="height">Must be greater than 0</param>
        public Rectangle(float x, float y, float width, float height)
        {

            this.position = new Vector2(x, y);
            this.dimensions = new Vector2(width, height);
        }

        public Rectangle(Vector2 position, Vector2 dimensions)
        {
            this.position = new(position.X, position.Y);
            this.dimensions = new(dimensions.X, dimensions.Y);
        }

        public Rectangle(SerializationInfo info, StreamingContext context)
        {
            position = new(info.GetSingle("x"), info.GetSingle("y"));
            dimensions = new(info.GetSingle("width"), info.GetSingle("height"));
        }

        /// <summary>
        /// Top left point of rectangle
        /// </summary>
        public Vector2 Position => new(position.X, position.Y);
        /// <summary>
        /// Dimensions of rectangle
        /// </summary>
        public Vector2 Dimensions => new(dimensions.X, dimensions.Y);

        /// <summary>
        /// The left side of the rectangle's x value
        /// </summary>
        public float X => position.X;
        /// <summary>
        /// The top of the rectangle's y value
        /// </summary>
        public float Y => position.Y;
        /// <summary>
        /// Width of Rectangle
        /// </summary>
        public float Width => dimensions.X;
        /// <summary>
        /// Height of Rectangle
        /// </summary>
        public float Height => dimensions.Y;

        /// <summary>
        /// Top left point of rectangle
        /// </summary>
        public Vector2 TopLeft => Position;
        /// <summary>
        /// Top right point of rectangle
        /// </summary>
        public Vector2 TopRight => new(X + Width, Y);
        /// <summary>
        /// Bottom left point of rectangle
        /// </summary>
        public Vector2 BottomLeft => new(X, Y + Height);
        /// <summary>
        /// Bottom right point of rectangle
        /// </summary>
        public Vector2 BottomRight => new(X + Width, Y + Height);

        public bool Contains(Vector2 point)
        {
            Vector2 bottomRight = BottomRight;
            return X <= point.X && point.X <= bottomRight.X &&
                Y <= point.Y && point.Y <= bottomRight.Y;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if the Rectangle intersects the specified Rectangle other</returns>
        public bool Intersects(Rectangle other)
        {
            Vector2 thisBottomRight = this.BottomRight;
            Vector2 otherBottomRight = other.BottomRight;

            //https://www.geeksforgeeks.org/find-two-rectangles-overlap/
            if (this.X >= otherBottomRight.X || thisBottomRight.X <= other.X)
            {
                return false;
            }
            if (this.Y >= otherBottomRight.Y || thisBottomRight.Y <= other.Y)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// </summary>
        /// <param name="circle"></param>
        /// <returns>true if the Rectangle intersects the specified Circle circle</returns>
        public bool Intersects(Circle circle) => circle.Intersects(this);

        public IShape CloneShape() => new Rectangle(X, Y, Width, Height);
        /// <summary>
        /// </summary>
        /// <returns>An immutable copy of this Rectangle as a Rectangle instance</returns>
        public Rectangle Clone() => (Rectangle)CloneShape();
        public Rectangle Bounds => Clone();
        public Vector2 Center => position + (dimensions / 2);

        public Vector2[] Points => new Vector2[] { TopRight, TopLeft,
            BottomLeft, BottomRight };

        /// <summary>
        /// Creates and returns a Vector2 with x and y values by which the rectangle can be pushed 
        /// out
        /// </summary>
        /// <param name="pusher"></param>
        /// <param name="pushee"></param>
        /// <returns></returns>
        public static Vector2 PushOut(Rectangle pusher, Rectangle pushee)
        {
            float x;
            float y;
            //if pushee to left
            if (pushee.X + pushee.Width / 2 < pusher.X + pusher.Width / 2)
            {
                //push pushee left
                x = -(pushee.X + pushee.Width - pusher.X);
            }
            else
            {
                //push pushee right
                x = pusher.X + pusher.Width - pushee.X;
            }
            //if pushee below
            if (pushee.Y + pushee.Height / 2 < pusher.Y + pusher.Height / 2)
            {
                //push pushee down
                y = -(pushee.Y + pushee.Height - pusher.Y);
            }
            else
            {
                //push pushee up
                y = pusher.Y + pusher.Height - pushee.Y;
            }
            //find amounts by which to push out
            return new(x, y);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("x", X);
            info.AddValue("y", Y);
            info.AddValue("width", Width);
            info.AddValue("height", Height);
        }
    }

    /// <summary>
    /// A circle with a center and a radius. 
    /// </summary>
    [Serializable]
    public struct Circle : IShape, ISerializable
    {
        /// <summary>
        /// The circle's center
        /// </summary>
        private Vector2 center;
        /// <summary>
        /// The circle's radius
        /// </summary>
        private float radius;

        /// <summary>
        /// Constructor with x, y, and radius
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius">Must be greater than 0</param>
        public Circle(float x, float y, float radius)
        {
            if (radius <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(radius),
                    "radius must be greater than 0");
            }

            this.center = new(x, y);
            this.radius = radius;
        }

        public Circle(Vector2 center, float radius)
        {
            if (radius <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(radius),
                    "radius must be greater than 0");
            }

            this.center = new(center.X, center.Y);
            this.radius = radius;
        }

        public Circle(SerializationInfo info, StreamingContext context)
        {
            center = new(info.GetSingle("x"), info.GetSingle("y"));
            radius = info.GetSingle("radius");
        }

        /// <summary>
        /// The center of the circle
        /// </summary>
        public Vector2 Center => new(center.X, center.Y);
        /// <summary>
        /// The radius of the circle
        /// </summary>
        public float Radius => radius;
        /// <summary>
        /// The circle's center's x value
        /// </summary>
        public float X => center.X;
        /// <summary>
        /// The circle's center's y value
        /// </summary>
        public float Y => center.Y;

        public Rectangle Bounds => new(X - radius, Y - radius, 2 * radius, 2 * radius);

        public IShape CloneShape() => new Circle(X, Y, radius);
        public Circle Clone() => (Circle)CloneShape();

        public bool Contains(Vector2 point)
        {
            float x = point.X - center.X;
            float y = point.Y - center.Y;
            return (x * x) + (y * y) <= (radius * radius);
        }

        /// <summary>
        /// </summary>
        /// <param name="rect"></param>
        /// <returns>true if the Circle intersects the specified Rectangle rect</returns>
        public bool Intersects(Rectangle rect)
        {
            //closest point to circle
            Vector2 closestPoint = Vector2.Clamp(center, rect.TopLeft, rect.BottomRight);
            return Contains(closestPoint);
        }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if the Circle intersects the specified Circle other</returns>
        public bool Intersects(Circle other)
        {
            Vector2 posDiff = other.Center - this.Center;
            float rSum = this.Radius + other.Radius;
            return posDiff.X * posDiff.X + posDiff.Y * posDiff.Y <= rSum * rSum;
        }

        public bool Intersects(LineSegment lineSegment)
        {
            return lineSegment.Intersects(this);
        }

        public bool Intersects(Triangle tri)
        {
            return tri.Intersects(this);
        }

        public bool Intersects(Capsule capsule)
        {
            return capsule.Intersects(this);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("x", X);
            info.AddValue("y", Y);
            info.AddValue("radius", Radius);
        }
    }

    [Serializable]
    public struct Triangle : IShape, ISerializable
    {
        /// <summary>
        /// The bounds of the triangle
        /// </summary>
        private Rectangle bounds;
        /// <summary>
        /// The quadrant of the triangle that is completely empty
        /// </summary>
        private byte emptyQuadrant;

        /// <summary>
        /// Constructor with x, y, width, height, and emptyQuadrant.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width">Must be greater than 0</param>
        /// <param name="height">Must be greater tahn 0</param>
        /// <param name="emptyQuadrant">Must be between 1 and 4, inclusive</param>
        public Triangle(float x, float y, float width, float height, int emptyQuadrant)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width),
                    "width must be greater than 0");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height),
                    "height must be greater than 0");
            }

            if (emptyQuadrant < 1 || emptyQuadrant > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(emptyQuadrant),
                    "emptyQuadrant must be between 1 and 4, inclusive");
            }

            this.bounds = new(x, y, width, height);
            this.emptyQuadrant = (byte)emptyQuadrant;
        }

        public Triangle(Vector2 position, Vector2 dimensions, int emptyQuadrant)
        {
            if (dimensions.X <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(dimensions),
                    "width must be greater than 0");
            }

            if (dimensions.Y <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(dimensions),
                    "height must be greater than 0");
            }

            if (emptyQuadrant < 1 || emptyQuadrant > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(emptyQuadrant),
                    "emptyQuadrant must be between 1 and 4, inclusive");
            }

            this.bounds = new(position, dimensions);
            this.emptyQuadrant = (byte)emptyQuadrant;
        }

        public Triangle(Rectangle bounds, int emptyQuadrant)
        {
            if (bounds.Width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bounds),
                    "width must be greater than 0");
            }

            if (bounds.Height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bounds),
                    "height must be greater than 0");
            }

            if (emptyQuadrant < 1 || emptyQuadrant > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(emptyQuadrant),
                    "emptyQuadrant must be between 1 and 4, inclusive");
            }

            this.bounds = bounds;
            this.emptyQuadrant = (byte)emptyQuadrant;
        }

        public Triangle(SerializationInfo info, StreamingContext context)
        {
            bounds = (Rectangle)info.GetValue("bounds", typeof(Rectangle));
            emptyQuadrant = info.GetByte("emptyQuadrant");
        }

        public Rectangle Bounds => bounds;
        /// <summary>
        /// Which quadrant of the triangle is empty: 1, 2, 3, or 4
        /// </summary>
        public int EmptyQuadrant => emptyQuadrant;

        public float X => bounds.X;
        public float Y => bounds.Y;
        public float Width => bounds.Width;
        public float Height => bounds.Height;

        /// <summary>
        /// The slope of the hypoteneuse (dy/dx)
        /// </summary>
        public float Slope
        {
            get
            {
                switch (emptyQuadrant)
                {
                    case 1:
                    case 3:
                        Vector2 diff = bounds.BottomRight - bounds.TopLeft;
                        return diff.Y / diff.X;
                    case 2:
                    case 4:
                        diff = bounds.TopRight - bounds.BottomLeft;
                        return diff.Y / diff.X;
                }

                return float.NaN;
            }
        }

        /// <summary>
        /// The points of the triangle
        /// </summary>
        public Vector2[] Points
        {
            get
            {
                Vector2[] arr = new Vector2[3];
                int i = 0;
                if (emptyQuadrant != 1)
                {
                    arr[i] = bounds.TopRight;
                    i++;
                }
                if (emptyQuadrant != 2)
                {
                    arr[i] = bounds.TopLeft;
                    i++;
                }
                if (emptyQuadrant != 3)
                {
                    arr[i] = bounds.BottomLeft;
                    i++;
                }
                if (emptyQuadrant != 4)
                {
                    arr[i] = bounds.BottomRight;
                }

                return arr;
            }
        }

        /// <summary>
        /// The hypoteneuse of the triangle
        /// </summary>
        public LineSegment Hypoteneuse => (emptyQuadrant == 1 || emptyQuadrant == 3) ?
            new(bounds.TopLeft, bounds.BottomRight) : new(bounds.BottomLeft, bounds.TopRight);

        public Vector2 Center => bounds.Center;

        public IShape CloneShape() => new Triangle(X, Y, Width, Height, emptyQuadrant);

        /// <summary>
        /// This code is such spaghetti-ass bullshit. Seriously, why do I only need y>mx+b for QI? 
        /// If anything, I should need it for both QI and QII, but somehow, QII works like QIII and QIV. 
        /// What the actual shitbiscuits
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vector2 point)
        {
            float slope = Slope;
            //y=mx+b; slope=m
            Vector2 bottomRight = bounds.BottomRight;
            bool xWithin = X < point.X && point.X < bottomRight.X;
            //if q1 or q2: y > mx+b
            //else: y < mx + b
            bool yWithin = (emptyQuadrant == 1) ?
                (point.Y > slope * (point.X - X) + Y && point.Y < bottomRight.Y) :
                (point.Y < slope * (point.X - X) + Y && point.Y > Y);

            return xWithin && yWithin;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("bounds", bounds, typeof(Rectangle));
            info.AddValue("emptyQuadrant", emptyQuadrant);
        }

        public bool Intersects(Circle circle)
        {
            //check if circle center is inside
            Vector2 center = circle.Center;
            if (Contains(center))
            {
                return true;
            }

            //check if circle intersects any of the three line segments
            Vector2[] points = Points;
            LineSegment a = new(points[0].X, points[0].Y, points[1].X, points[1].Y);
            LineSegment b = new(points[1].X, points[1].Y, points[2].X, points[2].Y);
            LineSegment c = new(points[2].X, points[2].Y, points[0].X, points[0].Y);

            return a.DistanceTo(center) < circle.Radius ||
                b.DistanceTo(center) < circle.Radius || 
                c.DistanceTo(center) < circle.Radius;
        }
    }

    /// <summary>
    /// A line segment with two points
    /// </summary>
    [Serializable]
    public struct LineSegment : IShape, ISerializable
    {
        /// <summary>
        /// The first point
        /// </summary>
        private Vector2 pointA;
        /// <summary>
        /// The second point
        /// </summary>
        private Vector2 pointB;

        /// <summary>
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public LineSegment(float x1, float y1, float x2, float y2)
        {

            pointA = new(x1, y1);
            pointB = new(x2, y2);
        }

        public LineSegment(Vector2 p1, Vector2 p2)
        {
            pointA = new(p1.X, p1.Y);
            pointB = new(p2.X, p2.Y);
        }

        public LineSegment(SerializationInfo info, StreamingContext context)
        {
            pointA = new(info.GetSingle("x1"), info.GetSingle("y1"));
            pointB = new(info.GetSingle("x2"), info.GetSingle("y2"));
        }

        /// <summary>
        /// The first point
        /// </summary>
        public Vector2 PointA => new(pointA.X, pointA.Y);
        /// <summary>
        /// The second point
        /// </summary>
        public Vector2 PointB => new(pointB.X, pointB.Y);

        /// <summary>
        /// The slope of the line segment
        /// </summary>
        public float Slope => pointA.X == pointB.X ? float.NaN :
            (pointB.Y - pointA.Y) / (pointB.X - pointA.X);
        /// <summary>
        /// The Y-intercept of the line segment
        /// </summary>
        public float YIntercept => pointA.Y - Slope * pointA.X;

        public Rectangle Bounds
        {
            get
            {
                Vector2 position = Vector2.Min(pointA, pointB);
                Vector2 dimensions = Vector2.Max(pointA, pointB) - position;

                return new Rectangle(position.X, position.Y, dimensions.X, dimensions.Y);
            }
        }

        public Vector2 Center => (pointA + pointB) / 2;

        public IShape CloneShape() =>
            new LineSegment(pointA.X, pointA.Y, pointB.X, pointB.Y);

        /// <summary>
        /// </summary>
        /// <returns>An immutable copy of this LineSegment</returns>
        public LineSegment Clone() => (LineSegment)CloneShape();

        public bool Contains(Vector2 point)
        {
            return Slope * point.X + YIntercept == point.Y;
        }

        /// <summary>
        /// </summary>
        /// <param name="point"></param>
        /// <returns>The distance from this line segment to point point</returns>
        public float DistanceTo(Vector2 point)
        {
            //https://stackoverflow.com/a/1501725
            /*
             * float minimum_distance(vec2 v, vec2 w, vec2 p) {
            // Return minimum distance between line segment vw and point p
            const float l2 = length_squared(v, w);  // i.e. |w-v|^2 -  avoid a sqrt
            if (l2 == 0.0) return distance(p, v);   // v == w case
            // Consider the line extending the segment, parameterized as v + t (w - v).
            // We find projection of point p onto the line. 
            // It falls where t = [(p-v) . (w-v)] / |w-v|^2
            // We clamp t from [0,1] to handle points outside the segment vw.
            const float t = max(0, min(1, dot(p - v, w - v) / l2));
            const vec2 projection = v + t * (w - v);  // Projection falls on the segment
            return distance(p, projection);
            }*/

            float l2 = Vector2.DistanceSquared(pointA, pointB);
            if (l2 == 0) //pointA == pointB case
            {
                return Vector2.Distance(pointA, point);
            }

            float t = Math.Max(0, Math.Min(1, Vector2.Dot(point - pointA, pointB - pointA) / l2));
            Vector2 projection = pointA + t * (pointB - pointA);

            return Vector2.Distance(point, projection);
        }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        /// <returns>The distance from this line segment to line segment other</returns>
        public float DistanceTo(LineSegment other)
        {
            if (Intersects(other))
            {
                return 0;
            }

            float otherDisToThisA = other.DistanceTo(this.pointA);
            float otherDisToThisB = other.DistanceTo(this.pointB);
            float thisDisToOtherA = this.DistanceTo(other.pointA);
            float thisDisToOtherB = this.DistanceTo(other.pointB);

            return Math.Min(Math.Min(Math.Min(otherDisToThisA, otherDisToThisB), 
                thisDisToOtherA), thisDisToOtherB);
        }

        /// <summary>
        /// </summary>
        /// <param name="circle"></param>
        /// <returns>true if this line segment intersects Circle circle</returns>
        public bool Intersects(Circle circle)
        {
            return DistanceTo(circle.Center) < circle.Radius;
        }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if this line segment intersects LineSegment other</returns>
        public bool Intersects(LineSegment other)
        {
            float m1 = this.Slope;
            float b1 = this.YIntercept;
            float m2 = other.Slope;
            float b2 = other.YIntercept;
            float thisMin = Math.Min(pointA.X, pointB.X);
            float thisMax = Math.Max(pointA.X, pointB.X);
            float otherMin = Math.Min(other.pointA.X, other.pointB.X);
            float otherMax = Math.Max(other.pointA.X, other.pointB.X);

            //if parallel
            if (m1 == m2)
            {
                //if different parallel lines
                if (b1 != b2)
                {
                    return false;
                }
                //if same line, check for x overlap
                return otherMin < thisMax && thisMin < otherMax;
            }

            //an intersection exists. Find it and check for bounds.
            float xIntersect = (b2 - b1) / (m1 - m2);
            float maxMin = Math.Max(thisMin, otherMin);
            float minMax = Math.Min(thisMax, otherMax);
            return maxMin < xIntersect && xIntersect < minMax;
        }

        public bool Intersects(Rectangle rect)
        {
            if(rect.Contains(pointA))
            {
                return true;
            }

            Vector2 bottomRight = rect.BottomRight;

            return Intersects(new LineSegment(bottomRight.X, rect.Y, rect.X, rect.Y)) ||
                Intersects(new LineSegment(rect.X, rect.Y, rect.X, bottomRight.Y)) ||
                Intersects(new LineSegment(rect.X, bottomRight.Y, bottomRight.X, bottomRight.Y)) ||
                Intersects(new LineSegment(bottomRight.X, bottomRight.Y, bottomRight.X, rect.Y));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("x1", pointA.X);
            info.AddValue("y1", pointA.Y);
            info.AddValue("x2", pointB.X);
            info.AddValue("y2", pointB.Y);
        }
    }

    [Serializable]
    public struct Capsule : IShape, ISerializable
    {
        private LineSegment line;
        private float radius;

        public Capsule(float x1, float y1, float x2, float y2, float radius)
        {
            this.line = new(x1, y1, x2, y2);
            this.radius = radius;
        }

        public Capsule(Vector2 pointA, Vector2 pointB, float radius)
        {
            this.line = new(pointA, pointB);
            this.radius = radius;
        }

        public Capsule(LineSegment line, float radius)
        {
            this.line = line;
            this.radius = radius;
        }

        public Capsule(SerializationInfo info, StreamingContext context)
        {
            line = (LineSegment)info.GetValue("line", typeof(LineSegment));
            radius = info.GetSingle("radius");
        }

        public LineSegment Line => line;

        public float Radius => radius;

        public Rectangle Bounds
        {
            get
            {
                Rectangle bounds = line.Bounds;
                bounds = new Rectangle(bounds.X - radius, bounds.Y - radius,
                    bounds.Width + 2 * radius, bounds.Height + 2 * radius);

                return bounds;
            }
        }

        public Vector2 Center => line.Center;

        public IShape CloneShape()
        {
            Vector2 a = line.PointA;
            Vector2 b = line.PointB;
            return new Capsule(a.X, a.Y, b.X, b.Y, radius);
        }

        public Capsule Clone() => (Capsule)CloneShape();

        public bool Contains(Vector2 point)
        {
            return line.DistanceTo(point) <= radius;
        }

        public LineSegment[] GetLines()
        {
            //simplified atan into sin/cos to minimize sqrt operations
            //sin(atan) = x/sqrt(x^2+1)
            //cos(atan) = 1/sqrt(x^2+1)
            float slope = -1 / line.Slope; //perpendicular to slope
            float denom = (float)Math.Sqrt(slope * slope + 1);
            float yOff = radius * slope / denom;
            float xOff = radius * 1 / denom;

            Vector2 pointA = line.PointA;
            Vector2 pointB = line.PointB;

            LineSegment[] lines = new LineSegment[2];
            lines[0] = new(pointA.X + xOff, pointA.Y + yOff, 
                pointB.X + xOff, pointB.Y + yOff);
            lines[1] = new(pointA.X - xOff, pointA.Y - yOff,
                pointB.X - xOff, pointB.Y - yOff);

            return lines;
        }

        /// <summary> 
        /// </summary>
        /// <param name="circle"></param>
        /// <returns>true if this capsule intersects circle other</returns>
        public bool Intersects(Circle circle)
        {
            return line.DistanceTo(circle.Center) < (this.radius + circle.Radius);
        }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if this capsule intersects capsule other</returns>
        public bool Intersects(Capsule other)
        {
            return line.DistanceTo(other.Line) < (this.radius + other.radius);
        }

        /// <summary>
        /// </summary>
        /// <param name="rect"></param>
        /// <returns>true if this capsule intersects rectangle rect</returns>
        public bool Intersects(Rectangle rect)
        {
            //three intersection cases:
            //rect inside of capsule
            //capsule circle intersects rect
            //capsule line intersects rect

            //rect inside of capsule
            if(this.Contains(rect.TopLeft))
            {
                return true;
            }

            //capsule circle intersects rect
            Vector2 pointA = line.PointA;
            Vector2 pointB = line.PointB;

            Circle circleA = new(pointA.X, pointA.Y, radius);
            Circle circleB = new(pointB.X, pointB.Y, radius);
            if(circleA.Intersects(rect) || circleB.Intersects(rect))
            {
                return true;
            }

            //capsule line intersects rect
            LineSegment[] lines = GetLines();
            return lines[0].Intersects(rect) || lines[1].Intersects(rect);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("line", line, typeof(LineSegment));
            info.AddValue("radius", radius);
        }
    }

    [Serializable]
    public struct Dynrapsule : IShape, ISerializable
    {
        private LineSegment line;
        private float r1;
        private float r2;

        public Dynrapsule(float x1, float y1, float x2, float y2, float r1, float r2)
        {
            this.line = new(x1, y1, x2, y2);
            this.r1 = r1;
            this.r2 = r2;
        }

        public Dynrapsule(Vector2 pointA, Vector2 pointB, float r1, float r2)
        {
            this.line = new(pointA, pointB);
            this.r1 = r1;
            this.r2 = r2;
        }

        public Dynrapsule(LineSegment line, float r1, float r2)
        {
            this.line = line;
            this.r1 = r1;
            this.r2 = r2;
        }

        public Dynrapsule(SerializationInfo info, StreamingContext context)
        {
            line = (LineSegment)info.GetValue("line", typeof(LineSegment));
            r1 = info.GetSingle("r1");
            r2 = info.GetSingle("r2");
        }

        public LineSegment Line => line;

        public float R1 => r1;
        public float R2 => r2;

        public Rectangle Bounds
        {
            get
            {
                Vector2 pointA = line.PointA;
                Vector2 pointB = line.PointB;

                float left = Math.Min(pointA.X - r1, pointB.X - r2);
                float right = Math.Max(pointA.X + r1, pointB.X + r2);
                float top = Math.Min(pointA.Y - r1, pointB.Y - r2);
                float bottom = Math.Max(pointA.Y + r1, pointB.Y + r2);


                return new Rectangle(left, top, right - left, bottom - top);
            }
        }

        public Vector2 Center => line.Center;

        public IShape CloneShape()
        {
            Vector2 a = line.PointA;
            Vector2 b = line.PointB;
            return new Dynrapsule(a.X, a.Y, b.X, b.Y, r1, r2);
        }

        public Dynrapsule Clone() => (Dynrapsule)CloneShape();

        public bool Contains(Vector2 point)
        {
            //CASE 1: one circle entirely contains the other
            //CASE 2: !float.isNaN(slope) && Math.Abs(angle) != Math.PI / 2
            //CASE 3: !float.isNaN(slope) && Math.Abs(angle) == Math.PI / 2
            //CASE 3: isNaN(slope)

            //case 1
            Vector2 pointA = line.PointA;
            Vector2 pointB = line.PointB;
            float rDiff = Math.Abs(r2 - r1);
            float length = (float)Math.Sqrt(Math.Pow(pointB.X - pointA.X, 2) + Math.Pow(pointB.Y - pointA.Y, 2));
            if (length + r1 <= r2 || length + r2 <= r1)
            {
                return r2 >= r1 ? new Circle(pointB, r2).Contains(point) : new Circle(pointA, r1).Contains(point);
            }

            
            float slope = line.Slope;
            bool axlt = pointA.X <= pointB.X;
            bool above = !float.IsNaN(slope) ? point.Y > slope * (point.X - pointA.X) + pointA.Y : point.X > pointA.X;

            float angle = (float)((!float.IsNaN(slope) ? Math.Atan(slope) : Math.PI / 2) + ((axlt == above) ? -Math.Acos(rDiff / length) : Math.Acos(rDiff / length)));

            float xIntersect = (float)(Math.Clamp(
                Math.Abs(angle) != Math.PI / 2 ? (slope * pointA.X - Math.Tan(angle) * point.X + point.Y - pointA.Y) / (slope - Math.Tan(angle)) : point.X,
                axlt ? pointA.X : pointB.X, axlt ? pointB.X : pointA.X));
            float yIntersect = slope * (xIntersect - pointA.X) + pointA.Y;

            float progress = (xIntersect - pointA.X) / (pointB.X - pointA.X);
            float radius = r1 + progress * (r2 - r1);
            return Math.Pow(point.X - xIntersect, 2) - Math.Pow(point.Y - yIntersect, 2) <= radius * radius;

        }

        public LineSegment[] GetLines()
        {
            //CASE 1: one circle entirely contains the other
            //CASE 2: !isNaN(slope)
            //CASE 3: isNaN(slope)

            //case 1
            Vector2 pointA = this.r1 < this.r2 ? line.PointA : line.PointB;
            Vector2 pointB = this.r1 < this.r2 ? line.PointB : line.PointA;
            float r1 = this.r1 < this.r2 ? this.r1 : this.r2;
            float r2 = this.r1 < this.r2 ? this.r2 : this.r1;

            float rDiff = Math.Abs(r2 - r1);
            float length = (float)Math.Sqrt(Math.Pow(pointB.X - pointA.X, 2) + Math.Pow(pointB.Y - pointA.Y, 2));
            float slope = line.Slope;

            float atan = (float)(float.IsNaN(slope) ? (pointA.Y < pointB.Y ? -Math.PI / 2 : Math.PI / 2) : Math.Atan(slope));
            float acos = (float)Math.Acos(rDiff / length);

            float angle1 = atan - acos;
            float angle2 = atan + acos;
            float b = pointA.X < pointB.X ? 1 : -1;

            LineSegment l1 = new(pointA.X - b * r1 * (float)Math.Cos(angle1), pointA.Y - b * r1 * (float)Math.Sin(angle1),
                pointB.X - b * r2 * (float)Math.Cos(angle1), pointB.Y - b * r2 * (float)Math.Sin(angle1));
            LineSegment l2 = new(pointA.X - b * r1 * (float)Math.Cos(angle2), pointA.Y - b * r1 * (float)Math.Sin(angle2),
                pointB.X - b * r2 * (float)Math.Cos(angle2), pointB.Y - b * r2 * (float)Math.Sin(angle2));

            return new LineSegment[] { l1, l2 };
        }

        /// <summary> 
        /// </summary>
        /// <param name="circle"></param>
        /// <returns>true if this capsule intersects circle other</returns>
        public bool Intersects(Circle circle)
        {
            //CASE 1: one circle entirely contains the other
            //CASE 2: !float.isNaN(slope) && Math.Abs(angle) != Math.PI / 2
            //CASE 3: !float.isNaN(slope) && Math.Abs(angle) == Math.PI / 2
            //CASE 3: isNaN(slope)
            Vector2 point = circle.Center;

            //case 1
            Vector2 pointA = line.PointA;
            Vector2 pointB = line.PointB;
            float rDiff = Math.Abs(r2 - r1);
            float length = (float)Math.Sqrt(Math.Pow(pointB.X - pointA.X, 2) + Math.Pow(pointB.Y - pointA.Y, 2));
            if (length + r1 <= r2 || length + r2 <= r1)
            {
                return r2 >= r1 ? new Circle(pointB, r2).Intersects(circle) : new Circle(pointA, r1).Intersects(circle);
                
            }

            float slope = line.Slope;
            bool axlt = pointA.X <= pointB.X;
            bool above = !float.IsNaN(slope) ? point.Y > slope * (point.X - pointA.X) + pointA.Y : point.X > pointA.X;

            float angle = (float)((!float.IsNaN(slope) ? Math.Atan(slope) : Math.PI / 2) + ((axlt == above) ? -Math.Acos(rDiff / length) : Math.Acos(rDiff / length)));

            float xIntersect = (float)(Math.Clamp(
                Math.Abs(angle) != Math.PI / 2 ? (slope * pointA.X - Math.Tan(angle) * point.X + point.Y - pointA.Y) / (slope - Math.Tan(angle)) : point.X,
                axlt ? pointA.X : pointB.X, axlt ? pointB.X : pointA.X));
            float yIntersect = slope * (xIntersect - pointA.X) + pointA.Y;

            float progress = (xIntersect - pointA.X) / (pointB.X - pointA.X);
            float radius = r1 + progress * (r2 - r1);
            return Math.Pow(point.X - xIntersect, 2) + Math.Pow(point.Y - yIntersect, 2) <= (radius + circle.Radius) * (radius + circle.Radius);
        }

        /// <summary>
        /// </summary>
        /// <param name="capsule"></param>
        /// <returns>true if this capsule intersects capsule other</returns>
        public bool Intersects(Capsule capsule)
        {
            //cannibalizes lineseg calcs 
            Vector2 pointA = line.PointA;
            Vector2 pointB = line.PointB;
            Vector2 otherA = capsule.Line.PointA;
            Vector2 otherB = capsule.Line.PointB;

            //Lines intersect
            if(line.Intersects(capsule.Line))
            {
                return true;
            }

            //otherA == otherB case made redundant by this
            if(Intersects(new Circle(otherA, capsule.Radius)))
            {
                return true;
            }
            //if chain just in case compiler tries to run all the shit in an or statement
            if(Intersects(new Circle(otherB, capsule.Radius)))
            {
                return true;
            }
            if(capsule.Intersects(new Circle(pointA, r1)))
            {
                return true;
            }
            if(capsule.Intersects(new Circle(pointB, r2)))
            {
                return true;
            }
            return false;
        }

        public bool Intersects(Dynrapsule other)
        {
            //cannibalizes lineseg calcs 
            Vector2 pointA = line.PointA;
            Vector2 pointB = line.PointB;
            Vector2 otherA = other.Line.PointA;
            Vector2 otherB = other.Line.PointB;

            //if chain just in case compiler tries to run all in a single or statement
            if (line.Intersects(other.Line))
            {
                return true;
            }
            if (Intersects(new Circle(otherA, other.R1)))
            {
                return true;
            }
            if (Intersects(new Circle(otherB, other.R2)))
            {
                return true;
            }
            if (other.Intersects(new Circle(pointA, r1)))
            {
                return true;
            }
            if (other.Intersects(new Circle(pointB, r2)))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// </summary>
        /// <param name="rect"></param>
        /// <returns>true if this capsule intersects rectangle rect</returns>
        public bool Intersects(Rectangle rect)
        {
            //three intersection cases:
            //rect inside of capsule
            //capsule circle intersects rect
            //capsule line intersects rect

            //rect inside of capsule
            if (this.Contains(rect.TopLeft))
            {
                return true;
            }

            //capsule circle intersects rect
            Vector2 pointA = line.PointA;
            Vector2 pointB = line.PointB;

            Circle circleA = new(pointA, r1);
            Circle circleB = new(pointB, r2);
            if (circleA.Intersects(rect) || circleB.Intersects(rect))
            {
                return true;
            }

            //capsule line intersects rect
            LineSegment[] lines = GetLines();
            return lines[0].Intersects(rect) || lines[1].Intersects(rect);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("line", line, typeof(LineSegment));
            info.AddValue("r1", r1);
            info.AddValue("r2", r2);
        }
    }



}

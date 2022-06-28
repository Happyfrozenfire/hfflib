using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HFFlib
{
    /// <summary>
    /// The abstract Hitbox class. Has Intersect(Hitbox), get/set Shape, GetBoundingBox(),
    /// and serialization/deserialization methods. Note that serialization/deserializaton
    /// methods must be overridden in subclasses to account for Shape.
    /// </summary>
    [Serializable]
    public abstract class Hitbox : ISerializable
    {
        public HitboxData Data;

        protected Hitbox()
        {
            Data = new();
        }

        protected Hitbox(HitboxData data)
        {
            this.Data = data;
        }

        protected Hitbox(SerializationInfo info, StreamingContext context)
        {
            Data = (HitboxData)info.GetValue("data", typeof(HitboxData));
        }

        public abstract Rectangle GetBoundingBox();

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("data", Data, typeof(HitboxData));
        }

        public abstract bool Intersects(Hitbox other);

        public abstract IShape Shape { get; set; }
    }

    /// <summary>
    /// The rectangle hitbox. Has implementations of all functions for a rectangle.
    /// </summary>
    [Serializable]
    public class HitRect : Hitbox
    {
        public Rectangle Rect;
        public override IShape Shape
        {
            get => Rect;
            set
            {
                if (value is Rectangle rect)
                {
                    Rect = rect;
                }
                else
                {
                    throw new InvalidCastException("Cannot implicitly convert " +
                        value.GetType().ToString() + "to Rectangle");
                }
            }
        }

        public HitRect(float x, float y, float width, float height, string type) : base()
        {
            Rect = new(x, y, width, height);
            Data.Type = type;
        }

        public HitRect(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Rect = (Rectangle)info.GetValue("rect", typeof(Rectangle));
        }


        public override Rectangle GetBoundingBox()
        {
            return Rect;
        }

        public override bool Intersects(Hitbox other)
        {
            if (other.Shape is Rectangle rect) return Rect.Intersects(rect);

            if (other.Shape is Circle circle) return Rect.Intersects(circle);

            return false;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("rect", Rect, typeof(Rectangle));
        }
    }

    /// <summary>
    /// The solid interface. Has a PushOut(collision) method that returns the inverse 
    /// amount by which an intersecting collision hitbox must be pushed out to no longer
    /// intersect this solid. Also has a get Instance method for convenience.
    /// </summary>
    public interface ISolid 
    {
        public const string TYPE = "solid";
        public abstract Vector2 Diff(ICollision collision);
        public abstract Hitbox Instance { get; }
    }

    /// <summary>
    /// The collision interface. Has a PushOut(solid) method that returns the amount by
    /// which it must be pushed out to no longer intersect an intersecting solid hitbox.
    /// </summary>
    public interface ICollision
    {
        public const string TYPE = "collision";
        public abstract Vector2 Diff(ISolid solid);

        public abstract Hitbox Instance { get; }
    }

    /// <summary>
    /// An implementation for a solid rectangle hitbox.
    /// </summary>
    [Serializable]
    public class SolidRect : HitRect, ISolid
    {
        public SolidRect(float x, float y, float width, float height) : base(x, y, width, height, ISolid.TYPE)
        {

        }

        public SolidRect(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public Vector2 Diff(ICollision collision)
        {
            if (collision is CollisionRect rect) return Utils.Pushout(rect.Rect, Rect);

            return Vector2.Zero;
        }

        public Hitbox Instance => this;
    }

    /// <summary>
    /// An implementation for a collision rectangle hitbox.
    /// </summary>
    [Serializable]
    public class CollisionRect : HitRect, ICollision
    {
        public CollisionRect(float x, float y, float width, float height) : base(x, y, width, height, ICollision.TYPE)
        {

        }

        public CollisionRect(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public Vector2 Diff(ISolid solid)
        {
            if (solid is SolidRect rect) return Utils.Pushout(rect.Rect, Rect);

            return Vector2.Zero;
        }

        public Hitbox Instance => this;
    }

    /// <summary>
    /// The circle hitbox. Has implementations for all functions of a circle.
    /// </summary>
    [Serializable]
    public class HitBubble : Hitbox
    {
        public Circle Bubble;
        public override IShape Shape
        {
            get => Bubble;
            set
            {
                if (value is Circle circle)
                {
                    Bubble = circle;
                }
                else
                {
                    throw new InvalidCastException("Cannot implicitly convert " +
                        value.GetType().ToString() + "to Circle");
                }
            }
        }

        public HitBubble(float x, float y, float radius, string type) : base()
        {
            Data.Type = type;
            Bubble = new(x, y, radius);
        }

        public HitBubble(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Bubble = (Circle)info.GetValue("bubble", typeof(Circle));
        }

        public override Rectangle GetBoundingBox()
        {
            return Bubble.Bounds;
        }

        public override bool Intersects(Hitbox other)
        {
            if (other.Shape is Rectangle rect) return Bubble.Intersects(rect);
            if (other.Shape is Circle circle) return Bubble.Intersects(circle);
            if (other.Shape is LineSegment ls) return Bubble.Intersects(ls);
            if (other.Shape is Triangle tri) return Bubble.Intersects(tri);
            if (other.Shape is Capsule cap) return Bubble.Intersects(cap);

            return false;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("bubble", Bubble, typeof(Circle));
        }
    }

    /// <summary>
    /// The capsule hitbox. Has implementations for all functions of a capsule.
    /// </summary>
    [Serializable]
    public class HitCapsule : Hitbox
    {
        public Capsule Capsule;
        public override IShape Shape
        {
            get => Capsule;
            set
            {
                if (value is Capsule capsule)
                {
                    Capsule = capsule;
                }
                else
                {
                    throw new InvalidCastException("Cannot implicitly convert " +
                        value.GetType().ToString() + "to Capsule");
                }
            }
        }

        public HitCapsule(float x1, float y1, float x2, float y2, float radius, string type) : base()
        {
            Data.Type = type;
            this.Capsule = new(x1, y1, x2, y2, radius);
        }

        public HitCapsule(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.Capsule = (Capsule)info.GetValue("capsule", typeof(Capsule));
        }

        public override Rectangle GetBoundingBox()
        {
            return this.Capsule.Bounds;
        }

        public override bool Intersects(Hitbox other)
        {
            if(other.Shape is Circle bubble) return Capsule.Intersects(bubble);

            if(other.Shape is Capsule capsule) return Capsule.Intersects(capsule);

            return false;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("capsule", Capsule, typeof(Capsule));
        }
    }


    /// <summary>
    /// A container for whatever data I decide to include.
    /// </summary>
    [Serializable]
    public class HitboxData : ISerializable
    {
        public string Type;
        public Vector2 Offset;

        public HitboxData()
        {
            Type = "";
            Offset = Vector2.Zero;
        }

        public HitboxData(SerializationInfo info, StreamingContext context)
        {
            Type = info.GetString("type");
            Offset = new(info.GetSingle("xoffset"), info.GetSingle("yoffset"));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("type", Type, typeof(string));
            info.AddValue("xoffset", Offset.X);
            info.AddValue("yoffset", Offset.Y);
        }
    }
}

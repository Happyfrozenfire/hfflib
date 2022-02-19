using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HFFlib
{
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
    }

    [Serializable]
    public abstract class SolidHitbox : Hitbox
    {
        protected SolidHitbox() : base()
        {
            Data.Type = "solid";
        }

        protected SolidHitbox(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public abstract Vector2 Diff(CollisionBubble collision);
    }

    [Serializable]
    public class SolidRect : SolidHitbox
    {
        public Rectangle Rect;

        public SolidRect(float x, float y, float width, float height) : base()
        {
            Rect = new(x, y, width, height);
        }

        public SolidRect(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Rect = (Rectangle)info.GetValue("rect", typeof(Rectangle));
        }

        public override Vector2 Diff(CollisionBubble collision)
        {
            return Utils.Pushout(collision.Bubble, Rect);
        }

        public override Rectangle GetBoundingBox()
        {
            return Rect;
        }

        public override bool Intersects(Hitbox other)
        {
            if(other is CollisionBubble collision)
            {
                return Rect.Intersects(collision.Bubble);
            }

            return false;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) 
        {
            base.GetObjectData(info, context);
            info.AddValue("rect", Rect, typeof(Rectangle));
        }
    }

    [Serializable]
    public class SolidTri : SolidHitbox
    {
        public Triangle Tri;

        public SolidTri(float x, float y, float width, float height, int emptyQuadrant) : base()
        {
            Tri = new(x, y, width, height, emptyQuadrant);
        }

        public SolidTri(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Tri = (Triangle)info.GetValue("tri", typeof(Triangle));
        }

        public override Vector2 Diff(CollisionBubble collision)
        {
            return -Utils.Pushout(Tri, collision.Bubble);
        }

        public override Rectangle GetBoundingBox()
        {
            return Tri.Bounds;
        }

        public override bool Intersects(Hitbox other)
        {
            if(other is CollisionBubble bubble)
            {
                return Tri.Intersects(bubble.Bubble);
            }

            return false;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("tri", Tri, typeof(Triangle));
        }
    }

    [Serializable]
    public class CollisionBubble : Hitbox
    {
        public Circle Bubble;

        public CollisionBubble(float x, float y, float radius) : base()
        {
            Data.Type = "collision";
            Bubble = new(x, y, radius);
        }

        public CollisionBubble(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Bubble = (Circle)info.GetValue("bubble", typeof(Circle));
        }

        public Vector2 Diff(SolidHitbox other)
        {
            if(other is SolidRect rect)
            {
                return -Utils.Pushout(Bubble, rect.Rect);
            }
            else if(other is SolidTri tri)
            {
                return Utils.Pushout(tri.Tri, Bubble);
            }

            throw new ArgumentException("other is SolidHitbox but has no Diff implementation");
        }

        public override Rectangle GetBoundingBox()
        {
            return Bubble.Bounds;
        }

        public override bool Intersects(Hitbox other)
        {
            if(other is SolidRect rect)
            {
                return Bubble.Intersects(rect.Rect);
            }
            if(other is SolidTri tri)
            {
                return Bubble.Intersects(tri.Tri);
            }

            return false;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("bubble", Bubble, typeof(Circle));
        }
    }

    [Serializable]
    public class HitBubble : Hitbox
    {
        public Circle Bubble;

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
            if (other is HitBubble bubble)
            {
                return Bubble.Intersects(bubble.Bubble);
            }
            if (other is HitCapsule capsule)
            {
                return Bubble.Intersects(capsule.Capsule);
            }

            return false;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("bubble", Bubble, typeof(Circle));
        }
    }

    [Serializable]
    public class HitCapsule : Hitbox
    {
        public Capsule Capsule;

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
            if(other is HitBubble bubble)
            {
                return Capsule.Intersects(bubble.Bubble);
            }
            if(other is HitCapsule capsule)
            {
                return Capsule.Intersects(capsule.Capsule);
            }

            return false;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("capsule", Capsule, typeof(Capsule));
        }
    }



    [Serializable]
    public class HitboxData : ISerializable
    {
        public string Type;

        public HitboxData()
        {

        }

        public HitboxData(SerializationInfo info, StreamingContext context)
        {
            Type = info.GetString("type");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("type", Type, typeof(string));
        }
    }
}

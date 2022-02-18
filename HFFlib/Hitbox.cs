using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HFFlib
{
    public abstract class Hitbox
    {
        public enum Type
        {
            SOLID,
            COLLISION,
            HIT,
            OTHER
        }

        public Type type;

        protected Hitbox(Type type)
        {
            this.type = type;
        }

        public abstract Rectangle GetBoundingBox();
        public abstract bool Intersects(Hitbox other);
    }

    public abstract class SolidHitbox : Hitbox
    {
        protected SolidHitbox() : base(Type.SOLID)
        {

        }

        public abstract Vector2 Diff(CollisionBubble collision);
    }

    public class SolidRect : SolidHitbox
    {
        public Rectangle Rect;

        public SolidRect(float x, float y, float width, float height) : base()
        {
            Rect = new(x, y, width, height);
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
    }

    public class SolidTri : SolidHitbox
    {
        public Triangle Tri;

        public SolidTri(float x, float y, float width, float height, int emptyQuadrant) : base()
        {
            Tri = new(x, y, width, height, emptyQuadrant);
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
    }

    public class CollisionBubble : Hitbox
    {
        public Circle Bubble;

        public CollisionBubble(float x, float y, float radius) : base(Type.COLLISION)
        {
            Bubble = new(x, y, radius);
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
    }

    public class HitBubble : Hitbox
    {
        public Circle Bubble;

        public HitBubble(float x, float y, float radius) : base(Type.HIT)
        {
            Bubble = new(x, y, radius);
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
    }

    public class HitCapsule : Hitbox
    {
        public Capsule Capsule;

        public HitCapsule(float x1, float y1, float x2, float y2, float radius) : base(Type.HIT)
        {
            this.Capsule = new(x1, y1, x2, y2, radius);
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
    }
}

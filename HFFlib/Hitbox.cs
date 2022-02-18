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
            HURT,
            HIT,
            GRAB,
            OTHER
        }

        public Type type;

        protected Hitbox(Type type)
        {
            this.type = type;
        }

        public abstract Rectangle GetBoundingBox();
        public abstract bool Intersects(Hitbox other);
        public abstract Vector2 Diff(Hitbox other);
    }

    public class SolidRect : Hitbox
    {
        public Rectangle Rect;

        public SolidRect(float x, float y, float width, float height) : base(Type.SOLID)
        {
            Rect = new(x, y, width, height);
        }

        public override Vector2 Diff(Hitbox other)
        {
            if(other is CollisionBubble collision)
            {
                return Utils.Pushout(collision.Bubble, Rect);
            }

            return Vector2.Zero;
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

    public class SolidTri : Hitbox
    {
        public Triangle Tri;

        public SolidTri(float x, float y, float width, float height, int emptyQuadrant) : base(Type.SOLID)
        {
            Tri = new(x, y, width, height, emptyQuadrant);
        }

        public override Vector2 Diff(Hitbox other)
        {
            if(other is CollisionBubble bubble)
            {
                return -Utils.Pushout(Tri, bubble.Bubble);
            }

            return Vector2.Zero;
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

        public override Vector2 Diff(Hitbox other)
        {
            if(other is SolidRect rect)
            {
                return -Utils.Pushout(Bubble, rect.Rect);
            }
            else if(other is SolidTri tri)
            {
                return Utils.Pushout(tri.Tri, Bubble);
            }

            return Vector2.Zero;
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
}

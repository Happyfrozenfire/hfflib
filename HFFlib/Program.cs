using System;
using System.Numerics;
using Raylib_CsLo;

namespace HFFlib
{
    class Program
    {
        static Camera2D camera = new();

        static SolidRect rect = new(300, 300, 100, 100);
        static CollisionBubble circle = new(100, 100, 10);
        static SolidTri tri = new(500, 200, 100, 50, 4);
        static LineSegment line = new(200, 200, 250, 250);
        static Capsule capsule = new(300, 300, 350, 350, 40);
        static bool intersecting = false;

        static void Main(string[] args)
        {
            Raylib.InitWindow(800, 450, "HFFlib test");
            Raylib.SetTargetFPS(60);

            //init
            Init();

            while (!Raylib.WindowShouldClose())
            {
                //tick
                Tick();

                Render();
            }

            Raylib.CloseWindow();
        }

        static void Init()
        {
            
        }

        static void Tick()
        {
            bool up = Raylib.IsKeyDown(KeyboardKey.KEY_UP);
            bool down = Raylib.IsKeyDown(KeyboardKey.KEY_DOWN);
            bool left = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT);
            bool right = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT);

            Vector2 pos = circle.Bubble.Center;
            if(up)
            {
                pos.Y -= 2;
            }
            if(down)
            {
                pos.Y += 2;
            }
            if(left)
            {
                pos.X -= 2;
            }
            if(right)
            {
                pos.X += 2;
            }
            circle.Bubble = new Circle(pos.X, pos.Y, circle.Bubble.Radius);

            intersecting = false;

            if(circle.GetBoundingBox().Intersects(rect.GetBoundingBox()) && circle.Intersects(rect))
            {
                intersecting = true;
                Vector2 pushOut = circle.Diff(rect);
                circle.Bubble = new Circle(pos.X + pushOut.X, pos.Y + pushOut.Y, circle.Bubble.Radius);
            }

            if(circle.GetBoundingBox().Intersects(tri.GetBoundingBox()) && circle.Intersects(tri))
            {
                intersecting = true;
                Vector2 pushOut = circle.Diff(tri);
                circle.Bubble = new Circle(pos.X + pushOut.X, pos.Y + pushOut.Y, circle.Bubble.Radius);
            }

            intersecting = intersecting || circle.Bubble.Intersects(line) || 
                circle.Bubble.Intersects(capsule);
        }

        static void Render()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);


            //render
            Raylib.DrawRectangle((int)rect.Rect.X, (int)rect.Rect.Y, 
                (int)rect.Rect.Width, (int)rect.Rect.Height, Raylib.ORANGE);

            Vector2[] points = tri.Tri.Points;
            Raylib.DrawTriangle(points[0], points[1], points[2], Raylib.ORANGE);

            Vector2 pointA = line.PointA;
            Vector2 pointB = line.PointB;
            Raylib.DrawLine((int)pointA.X, (int)pointA.Y, (int)pointB.X, (int)pointB.Y, Raylib.ORANGE);

            pointA = capsule.Line.PointA;
            pointB = capsule.Line.PointB;
            Raylib.DrawCircle((int)pointA.X, (int)pointA.Y, capsule.Radius, Raylib.ORANGE);
            Raylib.DrawCircle((int)pointB.X, (int)pointB.Y, capsule.Radius, Raylib.ORANGE);
            LineSegment[] lines = capsule.GetLines();
            pointA = lines[0].PointA;
            pointB = lines[0].PointB;
            Raylib.DrawLine((int)pointA.X, (int)pointA.Y, (int)pointB.X, (int)pointB.Y, Raylib.ORANGE);
            pointA = lines[1].PointA;
            pointB = lines[1].PointB;
            Raylib.DrawLine((int)pointA.X, (int)pointA.Y, (int)pointB.X, (int)pointB.Y, Raylib.ORANGE);

            Vector2 center = circle.Bubble.Center;
            Raylib.DrawCircle((int)center.X, (int)center.Y, circle.Bubble.Radius, 
                intersecting ? Raylib.GREEN : Raylib.SKYBLUE);

            Raylib.DrawFPS(10, 10);
            Raylib.EndDrawing();
        }
    }
}

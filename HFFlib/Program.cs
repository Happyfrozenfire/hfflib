using System;
using System.Numerics;
using Raylib_CsLo;

namespace HFFlib
{
    class Program
    {
        static Camera2D camera = new();

        static Dynrapsule rapsule1 = new(100, 100, 120, 130, 20, 30);

        static Dynrapsule rapsule2 = new(200, 200, 700, 300, 100, 50);

        //controlling rapsule1 = true, rapsule 2 = false
        static bool controlling = true;

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

            bool w = Raylib.IsKeyDown(KeyboardKey.KEY_W);
            bool s = Raylib.IsKeyDown(KeyboardKey.KEY_S);
            bool a = Raylib.IsKeyDown(KeyboardKey.KEY_A);
            bool d = Raylib.IsKeyDown(KeyboardKey.KEY_D);

            bool z = Raylib.IsKeyDown(KeyboardKey.KEY_Z);
            bool x = Raylib.IsKeyDown(KeyboardKey.KEY_X);
            bool comma = Raylib.IsKeyDown(KeyboardKey.KEY_COMMA);
            bool period = Raylib.IsKeyDown(KeyboardKey.KEY_PERIOD);

            bool space = Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE);

            if(space)
            {
                controlling = !controlling;
            }

            if (controlling)
            {
                Vector2 posA = rapsule1.Line.PointA;
                Vector2 posB = rapsule1.Line.PointB;
                float r1 = rapsule1.R1;
                float r2 = rapsule1.R2;

                if (w)
                {
                    posA.Y -= 2;
                }
                if (s)
                {
                    posA.Y += 2;
                }
                if (a)
                {
                    posA.X -= 2;
                }
                if (d)
                {
                    posA.X += 2;
                }

                if (up)
                {
                    posB.Y -= 2;
                }
                if (down)
                {
                    posB.Y += 2;
                }
                if (left)
                {
                    posB.X -= 2;
                }
                if (right)
                {
                    posB.X += 2;
                }

                if (z)
                {
                    r1 = Math.Max(1, r1 - 1);
                }
                if (x)
                {
                    r1 += 1;
                }

                if (comma)
                {
                    r2 = Math.Max(1, r2 - 1);
                }
                if (period)
                {
                    r2 += 1;
                }

                rapsule1 = new(posA, posB, r1, r2);
            }
            else
            {
                Vector2 posA = rapsule2.Line.PointA;
                Vector2 posB = rapsule2.Line.PointB;
                float r1 = rapsule2.R1;
                float r2 = rapsule2.R2;

                if (w)
                {
                    posA.Y -= 2;
                }
                if (s)
                {
                    posA.Y += 2;
                }
                if (a)
                {
                    posA.X -= 2;
                }
                if (d)
                {
                    posA.X += 2;
                }

                if (up)
                {
                    posB.Y -= 2;
                }
                if (down)
                {
                    posB.Y += 2;
                }
                if (left)
                {
                    posB.X -= 2;
                }
                if (right)
                {
                    posB.X += 2;
                }

                if (z)
                {
                    r1 = Math.Max(1, r1 - 1);
                }
                if (x)
                {
                    r1 += 1;
                }

                if (comma)
                {
                    r2 = Math.Max(1, r2 - 1);
                }
                if (period)
                {
                    r2 += 1;
                }

                rapsule2 = new(posA, posB, r1, r2);
            }

            intersecting = false;

            if(rapsule1.Bounds.Intersects(rapsule2.Bounds))
            {
                intersecting = rapsule1.Intersects(rapsule2);
            }
        }

        static void Render()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            Vector2 p1a = rapsule1.Line.PointA;
            Vector2 p1b = rapsule1.Line.PointB;
            Vector2 p2a = rapsule2.Line.PointA;
            Vector2 p2b = rapsule2.Line.PointB;
            LineSegment[] lines1 = rapsule1.GetLines();
            LineSegment[] lines2 = rapsule2.GetLines();
            Rectangle bounds1 = rapsule1.Bounds;
            Rectangle bounds2 = rapsule2.Bounds;
            LineSegment middle1 = rapsule1.Line;
            LineSegment middle2 = rapsule2.Line;

            Color c1 = intersecting ? Raylib.ORANGE : Raylib.RED;
            Color c2 = intersecting ? Raylib.SKYBLUE : Raylib.VIOLET;

            Raylib.DrawCircleLines((int)Math.Round(p1a.X), (int)Math.Round(p1a.Y), rapsule1.R1, c1);
            Raylib.DrawCircleLines((int)Math.Round(p1b.X), (int)Math.Round(p1b.Y), rapsule1.R2, c1);
            foreach(LineSegment line in lines1)
            {
                Vector2 a = line.PointA;
                Vector2 b = line.PointB;
                Raylib.DrawLine((int)Math.Round(a.X), (int)Math.Round(a.Y), (int)Math.Round(b.X), (int)Math.Round(b.Y), c1);
            }
            /*Raylib.DrawRectangleLines((int)Math.Round(bounds1.TopLeft.X), (int)Math.Round(bounds1.TopLeft.Y), 
                (int)Math.Round(bounds1.Width), (int)Math.Round(bounds1.Height), c1);
            Raylib.DrawLine((int)Math.Round(middle1.PointA.X), (int)Math.Round(middle1.PointA.Y), 
                (int)Math.Round(middle1.PointB.X), (int)Math.Round(middle1.PointB.Y), c1);*/

            Raylib.DrawCircleLines((int)Math.Round(p2a.X), (int)Math.Round(p2a.Y), rapsule2.R1, c2);
            Raylib.DrawCircleLines((int)Math.Round(p2b.X), (int)Math.Round(p2b.Y), rapsule2.R2, c2);
            foreach (LineSegment line in lines2)
            {
                Vector2 a = line.PointA;
                Vector2 b = line.PointB;
                Raylib.DrawLine((int)Math.Round(a.X), (int)Math.Round(a.Y), (int)Math.Round(b.X), (int)Math.Round(b.Y), c2);
            }
            /*Raylib.DrawRectangleLines((int)bounds2.TopLeft.X, (int)bounds2.TopLeft.Y, (int)bounds2.Width, (int)bounds2.Height, c2);
            Raylib.DrawLine((int)Math.Round(middle2.PointA.X), (int)Math.Round(middle2.PointA.Y),
                (int)Math.Round(middle2.PointB.X), (int)Math.Round(middle2.PointB.Y), c2);*/

            Raylib.DrawFPS(10, 10); 
            Raylib.EndDrawing();
        }
    }
}

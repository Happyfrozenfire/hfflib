using System;
using System.Numerics;
using Raylib_CsLo;

namespace HFFlib
{
    class Program
    {
        static Camera2D camera = new();

        static CollisionRect collision = new(100, 100, 20, 30);

        static SolidRect solid = new(200, 200, 500, 300);
        /*
        static SolidLineSeg ls1 = new(200, 300, 600, 300, (float)Math.PI / 2);
        static SolidLineSeg ls2 = new(600, 300, 500, 400, 0);
        static SolidLineSeg ls3 = new(500, 400, 400, 450, 11 * (float)Math.PI / 6);
        static SolidLineSeg ls4 = new(400, 450, 300, 400, 7 * (float)Math.PI / 6);
        static SolidLineSeg ls5 = new(300, 400, 200, 300, (float)Math.PI);
        */
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

            Vector2 pos = collision.Rect.Position;
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
            collision.Rect = new Rectangle(pos.X, pos.Y, collision.Rect.Width, collision.Rect.Height);

            intersecting = false;

            if(collision.GetBoundingBox().Intersects(solid.GetBoundingBox()))
            {
                if(collision.Intersects(solid))
                {
                    intersecting = true;
                    Vector2 pushOut = collision.Diff(solid);
                    pushOut = Math.Abs(pushOut.Y) <= Math.Abs(pushOut.X) ? new(0, pushOut.Y) : new(pushOut.X, 0);
                    collision.Rect = new Rectangle(pos.X + pushOut.X, pos.Y + pushOut.Y,
                        collision.Rect.Width, collision.Rect.Height);
                }
            }
        }

        static void Render()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            Raylib.DrawRectangle((int)collision.Rect.X, (int)collision.Rect.Y, 
                (int)collision.Rect.Width, (int)collision.Rect.Height, 
                intersecting ? Raylib.GREEN : Raylib.SKYBLUE);

            Raylib.DrawRectangle((int)solid.Rect.X, (int)solid.Rect.Y,
                (int)solid.Rect.Width, (int)solid.Rect.Height, Raylib.ORANGE);

            Raylib.DrawFPS(10, 10);
            Raylib.EndDrawing();
        }
    }
}

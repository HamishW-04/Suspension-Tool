//using System.Numerics;
using Raylib_cs;
using System.Data;
using System.Numerics;

namespace Core_App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Defining Window
            Raylib.InitWindow(1280, 720, "Display");
            Raylib.SetTargetFPS(60);

            //Define Scene
            Scene scene = new Scene();

            //Camera
            Camera3D camera = new Camera3D();
            camera.Projection = CameraProjection.Perspective;
            camera.Position = new Vector3(-4.0f, 4.0f, -4.0f);
            camera.Target = new Vector3(0, 0, 0);
            camera.Up = new Vector3(0, 1, 0);
            camera.FovY = 45f;

            //Main Loop
            long prevTicks = DateTime.Now.Ticks;
            while (!Raylib.WindowShouldClose())
            {
                //Calculating Delta Time
                float deltaTime = (DateTime.Now.Ticks - prevTicks) / 10000000.0f;
                prevTicks = DateTime.Now.Ticks;

                scene.CallUpdate(new UpdateEventArgs() { DeltaTime = deltaTime }); // Calling Update

                //Drawing
                {
                    Raylib.BeginDrawing();

                    Raylib.ClearBackground(Color.White);

                    //3D Begin
                    {
                        Raylib.BeginMode3D(camera);

                        Raylib.DrawGrid(100, 0.5f);
                        scene.CallRender3D(EventArgs.Empty);

                        Raylib.EndMode3D();
                    }

                    scene.CallRender2D(EventArgs.Empty);

                    Raylib.EndDrawing();
                }
            }
        }
    }
}

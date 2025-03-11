//using System.Numerics;
using Raylib_cs;
using System.Data;
using System.Numerics;
using ImGuiNET;

namespace Core_App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //// Setting Up Raylib
            //Defining Graphic Window
            Raylib.InitWindow(1280, 720, "Display");
            Raylib.SetTargetFPS(60);

            //Define Scene
            Scene scene = new Scene();

            //Camera
            Camera3D camera = new Camera3D();
            float movementSpeed = 2f;
            camera.Projection = CameraProjection.Perspective;
            camera.Position = new Vector3(-4.0f, 4.0f, -4.0f);
            camera.Target = Vector3.Zero;
            camera.Up = new Vector3(0, 1, 0);
            camera.FovY = 45f;

            //// Scene Contnents
            Point origin = new Point(scene, Vector3.Zero, true);
            Axis xAxis = new Axis(scene, -Vector3.UnitX, Color.Blue);
            Axis yAxis = new Axis(scene, Vector3.UnitY, Color.Green);
            Axis zAxis = new Axis(scene, Vector3.UnitZ, Color.Red);

            //Front
            Point contactPoint = new Point(scene, origin.GetTransform(), new Vector3(0,0,3), true);
            Wheel w = new Wheel(scene, contactPoint, 0.5f, 0.2f);
            Point h1 = new Point(scene, contactPoint.GetTransform(), new Vector3(0f, 0.6f, -1f), false);
            ControlArm arm = new ControlArm(scene,h1,w.GetCentrePoint());


            //// Setting up IMGUI
            GUI gui = new GUI(h1);
            gui.Start().Wait();

            //// Main Loop
            long prevTicks = DateTime.Now.Ticks;
            while (!Raylib.WindowShouldClose())
            {
                //Calculating Delta Time
                float deltaTime = (DateTime.Now.Ticks - prevTicks) / 10000000.0f;
                prevTicks = DateTime.Now.Ticks;

                scene.CallUpdate(new UpdateEventArgs() { DeltaTime = deltaTime }); // Calling Update



                //Camera Control
               
                
                if(Raylib.IsMouseButtonDown(MouseButton.Middle)) Raylib.UpdateCamera(ref camera, CameraMode.ThirdPerson);
                else
                {
                    Vector2 moveInput = Vector2.Zero;
                    if (Raylib.IsKeyDown(KeyboardKey.W)) moveInput.X = 1;
                    else if (Raylib.IsKeyDown(KeyboardKey.S)) moveInput.X = -1;

                    if (Raylib.IsKeyDown(KeyboardKey.D)) moveInput.Y = 1;
                    else if (Raylib.IsKeyDown(KeyboardKey.A)) moveInput.Y = -1;

                    if (moveInput != Vector2.Zero)
                    {
                        Raylib.CameraMoveForward(ref camera, moveInput.X * movementSpeed * deltaTime, true);
                        Raylib.CameraMoveRight(ref camera, moveInput.Y * movementSpeed * deltaTime, true);
                    }
                }
                
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

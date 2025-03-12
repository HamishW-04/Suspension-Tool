//using System.Numerics;
using Raylib_cs;
using System.Data;
using System.Numerics;
using ImGuiNET;
using Core_App.src;

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

            //Suspension
            Suspension suspension = new Suspension(
                500f,
                new Vector3(850f, 625f, 0f),
                new Vector3(665f, 244f, 0f),
                new Vector3(380f, 710f, 0f),
                new Vector3(260f, 240f, 0f),
                260f);

            //Points
            Point contactPoint = new Point(scene, origin.GetTransform(), suspension.ContactPoint, true);
            
            Point upperHP = new Point(scene, contactPoint.GetTransform(), suspension.UpperHardPoint / 1000f, true);
            Point lowerHP = new Point(scene, contactPoint.GetTransform(), suspension.LowerHardPoint / 1000f, true);
            Point kpTop = new Point(scene, contactPoint.GetTransform(), suspension.KingpinTop / 1000f, true);
            Point kpBottom = new Point(scene, contactPoint.GetTransform(), suspension.KingpinBottom / 1000f, true);

            //Objects
            Wheel w = new Wheel(scene, contactPoint, suspension.WheelCentre/1000f, suspension.WheelRadius/1000f, suspension.WheelWidth/1000f, suspension.Camber);

            Line upperCA = new Line(scene, upperHP, kpTop);
            Line lowerCA = new Line(scene, lowerHP, kpBottom);
            Line kingpin = new Line(scene, kpTop, kpBottom);
            Line stubAxel = new Line(scene, w.m_CentrePoint, new Point (scene, contactPoint.GetTransform(),suspension.StubStartPosition/1000f, true));


            //// Setting up IMGUI
            InputGUI gui = new InputGUI(suspension);
            Thread overlayGui = new Thread(gui.Start().Wait);
            overlayGui.Start();

            //// Main Loop
            long prevTicks = DateTime.Now.Ticks;
            while (!Raylib.WindowShouldClose())
            {
                //Calculating Delta Time
                float deltaTime = (DateTime.Now.Ticks - prevTicks) / 10000000.0f;
                prevTicks = DateTime.Now.Ticks;

                scene.CallUpdate(new UpdateEventArgs() { DeltaTime = deltaTime }); // Calling Update

                UpdateGeometry();

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

            void UpdateGeometry()
            {
                w.SetCamber(suspension.Camber);
                w.SetCentre(suspension.WheelCentre / 1000f);

                upperHP.GetTransform().SetLocalPosition(suspension.UpperHardPoint / 1000);
                lowerHP.GetTransform().SetLocalPosition(suspension.LowerHardPoint / 1000);
                kpTop.GetTransform().SetLocalPosition(suspension.KingpinTop / 1000);
                kpBottom.GetTransform().SetLocalPosition(suspension.KingpinBottom / 1000) ;
            }

        }
    }
}

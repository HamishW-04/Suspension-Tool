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

            //StartingProperties
            CarProperties carProp = new CarProperties(1f,3f,0.5f, 500f, 260f);
            SuspensionProperties suspProp = new SuspensionProperties(
                new Vector3(850f, 625f, 250f),
                new Vector3(850f, 625f, -250f),
                new Vector3(665f, 244f, 260f),
                new Vector3(665f, 244f, -260f),
                new Vector3(380f, 710f, 0f),
                new Vector3(260f, 240f, 0f),
                260f,
                new Vector3(900f, 800f, 0f),
                200f
                );

            //Suspension
            Suspension suspension = new Suspension(carProp, suspProp);

            //Points
            Point contactPoint = new Point(scene, origin.GetTransform(), suspension.ContactPoint, true);
            
            Point upperHrdPntA = new Point(scene, contactPoint.GetTransform(), suspProp.UpperContArmA / 1000f, true);
            Point upperHrdPntB = new Point(scene, contactPoint.GetTransform(), suspProp.UpperContArmB / 1000f, true);
            Point lowerHrdPntA = new Point(scene, contactPoint.GetTransform(), suspProp.LowerContArmA / 1000f, true);
            Point lowerHrdPntB = new Point(scene, contactPoint.GetTransform(), suspProp.LowerContArmB / 1000f, true);

            Point kpTop = new Point(scene, contactPoint.GetTransform(), suspProp.KingPinTop / 1000f, true);
            Point kpBottom = new Point(scene, contactPoint.GetTransform(), suspProp.KingPinBottom / 1000f, true);

            Point stubStart = new Point(scene, contactPoint.GetTransform(), suspension.StubStartPosition/1000f, true);

            Point upperContArmAvg = new Point(scene, contactPoint.GetTransform(), suspension.UpperHrdPntAvg/1000f, true);
            Point lowerContArmAvg = new Point(scene, contactPoint.GetTransform(), suspension.LowerHrdPntAvg/1000f, true);

            Point instancePoint = new Point(scene, contactPoint.GetTransform(), suspension.InstancePoint / 1000f, false);
            Point rollPoint = new Point(scene, origin.GetTransform(), suspension.RollCentre / 1000f, false);

            Point springTop = new Point(scene, contactPoint.GetTransform(), suspension.SpringHardPoint / 1000f, true);
            Point springBottom = new Point(scene, contactPoint.GetTransform(), suspension.SpringStartPos / 1000f, true);

            //Objects
            Wheel w = new Wheel(scene, contactPoint, suspension.WheelCentre/1000f, suspension.WheelRadius/1000f, suspension.WheelWidth/1000f, suspension.Camber);

            Line upperCntArmA = new Line(scene, upperHrdPntA, kpTop);
            Line upperCntArmB = new Line(scene, upperHrdPntB, kpTop);
            Line lowerCntArmA = new Line(scene, lowerHrdPntA, kpBottom);
            Line lowerCntArmB = new Line(scene, lowerHrdPntB, kpBottom);
            Line kingpin = new Line(scene, kpTop, kpBottom);
            Line stubAxel = new Line(scene, w.m_CentrePoint, stubStart);

            Line instanceLine1 = new Line(scene, upperContArmAvg, instancePoint);
            instanceLine1.IsSolid = false;

            Line instanceLine2 = new Line(scene, lowerContArmAvg, instancePoint);
            instanceLine2.IsSolid = false;

            Line rollLine = new Line(scene, instancePoint, contactPoint);
            rollLine.IsSolid = false;

            Spring spring = new Spring(scene, springBottom, springTop);

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

                upperHrdPntA.GetTransform().SetLocalPosition(suspension.UpperHardPointA / 1000f);
                upperHrdPntB.GetTransform().SetLocalPosition(suspension.UpperHardPointB / 1000f);
                lowerHrdPntA.GetTransform().SetLocalPosition(suspension.LowerHardPointA / 1000f);
                lowerHrdPntB.GetTransform().SetLocalPosition(suspension.LowerHardPointB / 1000f);

                kpTop.GetTransform().SetLocalPosition(suspension.KingpinTop / 1000f);
                kpBottom.GetTransform().SetLocalPosition(suspension.KingpinBottom / 1000f);

                stubStart.GetTransform().SetLocalPosition(suspension.StubStartPosition/1000f);

                upperContArmAvg.GetTransform().SetLocalPosition(suspension.UpperHrdPntAvg / 1000f);
                lowerContArmAvg.GetTransform().SetLocalPosition(suspension.LowerHrdPntAvg / 1000f);

                instancePoint.GetTransform().SetLocalPosition(suspension.InstancePoint / 1000f);

                springTop.GetTransform().SetLocalPosition(suspension.SpringHardPoint / 1000f);
                springBottom.GetTransform().SetLocalPosition(suspension.SpringStartPos / 1000f);
            }

        }
    }
}

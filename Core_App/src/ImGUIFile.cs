using ImGuiNET;
using ClickableTransparentOverlay;
using Core_App.src;

namespace Core_App
{
    class InputGUI : Overlay
    {
        //Properties
        private Suspension s;

        public InputGUI(Suspension n_Suspension)
        {
            s = n_Suspension;
        }


        protected override void Render()
        {
            bool modified = false;

            ImGui.Begin("Input Window");

            ImGui.SeparatorText("Car Properties Input");
            if (ImGui.InputFloat("Front Track (m)", ref s.Track)) modified = true;
            if (ImGui.InputFloat("Spring Stiffness (N/m)", ref s.SpringStiffness)) modified = true;
            if (ImGui.InputFloat("Spring Natural Length (mm)", ref s.SpringNaturalLength)) modified = true;

            ImGui.SeparatorText("Front Suspension Inputs (mm)");
            if(ImGui.InputFloat3("Upper Hard Point Position A", ref s.UpperHardPointA)) modified = true;
            if(ImGui.InputFloat3("Upper Hard Point Position B", ref s.UpperHardPointB)) modified = true;
            if (ImGui.InputFloat3("Lower Hard Point Position A", ref s.LowerHardPointA)) modified = true;
            if (ImGui.InputFloat3("Lower Hard Point Position B", ref s.LowerHardPointB)) modified = true;
            if (ImGui.InputFloat3("King-pin Top Position", ref s.KingpinTop)) modified = true;
            if(ImGui.InputFloat3("King-pin Bottom Position", ref s.KingpinBottom)) modified = true;
            if(ImGui.InputFloat("Stub Axel Start Position (Length From Bottom)", ref s.StubStartLength)) modified = true;
            if (ImGui.InputFloat3("Spring Hard Point Position", ref s.SpringHardPoint)) modified = true;
            if (ImGui.InputFloat("Spring Start Position (Length From Start of Arm)", ref s.SpringStartLength)) modified = true;

            ImGui.SeparatorText("Front Suspension Outputs");
            ImGui.BeginTable("Output Table", 2);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text("Wheel Centre (mm):");
            ImGui.TableNextColumn();
            ImGui.Text(s.WheelCentre.ToString());

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text("Camber (deg):");
            ImGui.TableNextColumn();
            ImGui.Text(s.Camber.ToString());

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text("King-pin Inclination (deg):");
            ImGui.TableNextColumn();
            ImGui.Text(s.KingPinInclination.ToString());

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text("Scrub Radius (mm):");
            ImGui.TableNextColumn();
            ImGui.Text(s.ScrubRadius.ToString());

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text("Instance Point (mm): ");
            ImGui.TableNextColumn();
            ImGui.Text(s.InstancePoint.ToString());

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text("Roll Centre (mm):");
            ImGui.TableNextColumn();
            ImGui.Text(s.RollCentre.ToString());

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text("MotionRatio:");
            ImGui.TableNextColumn();
            ImGui.Text(s.MotionRatio.ToString());

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.Text("Wheel Rate (N/m):");
            ImGui.TableNextColumn();
            ImGui.Text(s.WheelRate.ToString());

            ImGui.EndTable();

            if (modified) s.Calculate();
        }
    }
}
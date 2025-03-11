using ImGuiNET;
using ClickableTransparentOverlay;

namespace Core_App
{
    class GUI : Overlay
    {
        //Properties
        private Point h1;

        public GUI(Point H1)
        {
            h1 = H1;
        }

        protected override void Render()
        {
            ImGui.Begin("Input Window");
            ImGui.SeparatorText("Front Suspension Inputs");
            ImGui.InputFloat3("H1 Position", ref h1.GetTransform().m_Position);
        }
    }
}
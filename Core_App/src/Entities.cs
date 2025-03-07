using Raylib_cs;
using System.Numerics;

namespace Core_App
{
    class Point : SceneEntity
    {
        //Attributes
        private const float m_radius = 0.2f;
        private bool m_hidden = false;

        //Constructor
        public Point(Scene scene, Transform Parent, Vector3 LocalPosition, bool Hidden) : base(scene)
        {
            m_Transform.SetParent(Parent);
            m_Transform.SetLocalPosition(LocalPosition);
            m_hidden = Hidden;
        }
        public Point(Scene scene, Vector3 GlobalPosition, bool Hidden) : base(scene)
        {
            m_Transform.SetLocalPosition(GlobalPosition); //No parent so global = local position
            m_hidden = Hidden;
        }

        //Methods
        public override void Render(object sender, EventArgs e)
        {
            if (!m_hidden) Raylib.DrawSphere(m_Transform.GetGlobalPosition(), m_radius, Color.Red);
        }

    }

    class Line : SceneEntity
    {
        //Attributes
        private Vector3 m_start;
        private Vector3 m_end;
        private float m_thickness = 0.02f;

        //Constructor
        public Line(Scene scene) : base(scene)
        {

        }

        //Methods
        public override void Render(object sender, EventArgs e)
        {
            Raylib.DrawCylinderEx(m_start, m_end, m_thickness, m_thickness, 3, Color.Black);
            //Raylib.DrawLine3D(m_start, m_end, Color.Red);
        }

    }

}
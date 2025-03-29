using Raylib_cs;
using System.Net;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using Vortice.Direct3D11;

namespace Core_App
{
    class Point : SceneEntity
    {
        //Attributes
        private const float m_radius = 0.1f;
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

    class Axis : SceneEntity
    {
        //Attributes
        private Vector3 m_Direction;
        private Color m_Colour;
        private const float m_Length = 1f;

        public Point StartPoint { get; }
        public Point EndPoint { get; }

        //Constructor
        public Axis(Scene scene, Vector3 Direction, Color Colour) : base(scene)
        {
            m_Colour = Colour;
            m_Direction = Direction;
        }

        //Methods
        public override void Render(object sender, EventArgs e)
        {
            Vector3 endPos = m_Direction * m_Length;
            //Raylib.DrawLine3D(Vector3.Zero, endPos, m_Colour);
            Raylib.DrawCylinderEx(Vector3.Zero, endPos, 0.05f, 0f, 3, m_Colour);
        }

    }

    class Line : SceneEntity
    {
        //Attributes
        private Vector3 m_start { get { return StartPoint.GetTransform().GetGlobalPosition(); } }
        private Vector3 m_end { get { return EndPoint.GetTransform().GetGlobalPosition(); } }
        private float m_thickness = 0.02f;

        public bool IsSolid { set => m_solid = value; get => m_solid; }
        private bool m_solid = true;

        public Point StartPoint { get; }
        public Point EndPoint { get; }

        //Constructor
        public Line(Scene scene, Point Start, Point End ) : base(scene)
        {
            StartPoint = Start;
            EndPoint = End;
        }


        //Methods
        public override void Render(object sender, EventArgs e)
        {
            if (m_solid) Raylib.DrawCylinderEx(m_start, m_end, m_thickness, m_thickness, 3, Color.Black);
            else Raylib.DrawLine3D(m_start, m_end, Color.Blue);
        }

    }


    class Knuckle
    {
        private Scene m_scene;
        
        public Point m_Top;
        public Point m_Bottom;
        private Point m_StubStart;

        private float ratio = 0.5f;

        private Line kpLine;

        public Knuckle(Scene scene, Transform Parent,Point Top, Point Bottom)
        {
            m_scene = scene;
            m_Top = Top;
            m_Bottom = Bottom;

            m_StubStart = new Point(scene,Parent, Vector3.Zero, false );

            Calculate();

            kpLine = new Line(scene, m_Top,m_Bottom);
        }

        public void Calculate()
        {
            //StubStart
            Vector3 pos1 = m_Top.GetTransform().GetLocalPosition();
            Vector3 pos2 = m_Bottom.GetTransform().GetLocalPosition();
            Vector3 dir = Vector3.Normalize(pos1 - pos2);
            Vector3 midpoint = pos2 + (dir * (Vector3.Distance(pos1, pos2) * ratio));
            m_StubStart.GetTransform().SetLocalPosition(midpoint);
        }
    }

    class ControlArm
    {
        private Scene m_scene;

        private Point m_Hardpoint;
        public Point m_End { get; }
        private Line m_beam;

        public ControlArm(Scene scene, Point Hardpoint, Point End)
        {
            m_scene = scene;

            m_Hardpoint = Hardpoint;
            m_End = End;
            m_beam = new Line(scene, m_Hardpoint, m_End);
        }
    }

    class Wheel : SceneEntity
    {
        //Attributes
        private Point m_ContactPoint;
        public Point m_CentrePoint { get; }
        private float m_Radius;
        private float m_Width;

        private float m_Camber;

        //Constructor
        public Wheel(Scene scene, Point ContactPoint, Vector3 CentrePosition, float Radius, float Width, float Camber) : base(scene)
        {
            //Assign
            m_ContactPoint = ContactPoint;
            m_Radius = Radius;
            m_Width = Width;
            m_Camber = 0;

            //Find Center assuming 0deg camber
            Vector3 a = new Vector3(0, MathF.Cos(m_Camber), MathF.Sin(m_Camber));
            Vector3 b = new Vector3(0, -MathF.Sin(m_Camber), MathF.Cos(m_Camber));
            Vector3 up = Vector3.UnitY.Y * a + Vector3.UnitY.Z * b;
            up = Vector3.Normalize(up);
            m_CentrePoint = new Point(scene, m_ContactPoint.GetTransform(), CentrePosition, true);
        }

        //Fields
        public Point GetCentrePoint() { return m_CentrePoint; }

        public void SetCamber(float Camber) { m_Camber = Camber; }

        public void SetCentre(Vector3 Centre) { m_CentrePoint.GetTransform().SetLocalPosition(Centre); }

        //Methods
        public override void Render(object sender, EventArgs e)
        {

            //Calculations
            Vector3 centre = m_CentrePoint.GetTransform().GetGlobalPosition();
            Vector3 contact = m_ContactPoint.GetTransform().GetGlobalPosition();

            Vector3 dir = Vector3.Normalize(centre-contact);
     
            Vector3 cross = Vector3.Normalize(Vector3.Cross(dir, Vector3.UnitZ));
            Vector3 end1 = centre + (cross*m_Width);
            Vector3 end2 = centre - (cross*m_Width);

            //Draw Body
            Raylib.DrawCylinderEx(centre, end1, m_Radius, m_Radius, 16, Color.Gray);
            Raylib.DrawCylinderEx(centre, end2, m_Radius, m_Radius, 16, Color.Gray);
            //Draw Outline
            Raylib.DrawCylinderWiresEx(centre, end1, m_Radius, m_Radius, 16, Color.DarkGray);
            Raylib.DrawCylinderWiresEx(centre, end2, m_Radius, m_Radius, 16, Color.DarkGray);
        }

    }

    class Spring : SceneEntity
    {
        //Attributes
        private Vector3 m_start { get { return m_StartPoint.GetTransform().GetGlobalPosition(); } }
        private Vector3 m_end { get { return m_EndPoint.GetTransform().GetGlobalPosition(); } }
        private float m_thickness = 0.05f;
        private Point m_StartPoint;
        private Point m_EndPoint;

        //Constructor
        public Spring(Scene scene, Point Start, Point End) : base(scene)
        {
            m_StartPoint = Start;
            m_EndPoint = End;
        }

        //Methods
        public override void Render(object sender, EventArgs e)
        {
            Raylib.DrawCylinderEx(m_start, m_end, m_thickness, m_thickness, 16, Color.Lime);
        }
    }
}
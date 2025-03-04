//using System.Numerics;
using Raylib_cs;
using System.Numerics;

namespace Core_App
{

    class SceneEntity
    {
        //Attributs
        private protected Transform m_Transform;
        private Scene m_Scene;

        public SceneEntity(Scene scene)
        {
            m_Scene = scene;
            m_Transform = new Transform();
        }
    }

    class Line:SceneEntity
    {
        //Attributes
        private Vector3 m_start;
        private Vector3 m_end;

        //Constructor
        public Line(Scene scene) : base(scene)
        {
            
        }

        //Fields

    }

    class Scene
    {

    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Transform Origin = new Transform();
            Transform P = new Transform(Origin, new Vector3(1,5,2));


            Console.WriteLine(P.GetGlobalPosition());
        }
    }
}

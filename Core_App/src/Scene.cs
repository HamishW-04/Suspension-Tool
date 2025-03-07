namespace Core_App
{
    class Scene
    {
        //Event Handlers
        public event UpdateEventHandler Update;
        public event EventHandler Render3D;
        public event EventHandler Render2D;

        //Event Calls
        public virtual void CallUpdate(UpdateEventArgs e) { Update?.Invoke(this, e); }
        public virtual void CallRender3D(EventArgs e) { Render3D?.Invoke(this, e); }
        public virtual void CallRender2D(EventArgs e) { Render2D?.Invoke(this, e); }
    }

    abstract class SceneEntity
    {
        //Attributs
        private protected Transform m_Transform;
        private Scene m_Scene;

        public SceneEntity(Scene scene)
        {
            m_Scene = scene;
            m_Transform = new Transform();
            m_Scene.Render3D += Render;
        }

        public abstract void Render(object sender, EventArgs e);
    }

    public class UpdateEventArgs : EventArgs
    {
        public float DeltaTime { get; set; }
    }

    public delegate void UpdateEventHandler(object sender, UpdateEventArgs e);
}


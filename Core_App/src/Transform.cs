using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core_App
{
    class Transform
    {
        //Attributes
        private Transform? m_Parent;
        private Vector3 m_Position;

        public Transform()
        {
            m_Position = Vector3.Zero;
        }

        public Transform(Transform Parent, Vector3 LocalPosition)
        {
            m_Parent = Parent;
            m_Position = LocalPosition;
        }

        //Properties
        public Vector3 GetGlobalPosition()
        {
            if (m_Parent != null) return m_Parent.GetGlobalPosition() + m_Position;
            else return m_Position;
        }
        public Vector3 GetLocalPosition() { return m_Position; }
        public void SetParent(Transform Parent) { m_Parent = Parent; }
        public void SetLocalPosition(Vector3 NewPosition)
        {
            m_Position = NewPosition;
        }

        //Methods
        public void Translate(Vector3 Translation)
        {
            m_Position += Translation;
        }
    }
}

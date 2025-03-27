using System.Numerics;

namespace Core_App
{
    struct CarProperties
    {
        //// Properties
        //Vehicle Body
        public float WheelBase { get => m_WheelBase; } //m
        public float Track { get => m_Track; } //m
        public float WeightDistribution
        {
            get => m_WeightDistribution;
            set => m_WeightDistribution = (value < 0.1f || value > 0.9f) ? 0.5f : value;
        }
        public float FrontaxelDistance { get { return (1 - m_WeightDistribution) * m_WheelBase; } }
        public float RearaxelDistance { get { return  m_WeightDistribution * m_WheelBase; } }

        //Tyre
        public float WheelRadius { get => m_WheelRadius; }
        public float WheelWidth { get => m_WheelWidth; }

        //// Fields
        private float m_WheelBase;
        private float m_Track;
        private float m_WeightDistribution;
        private float m_WheelRadius;
        private float m_WheelWidth;

        public CarProperties(float n_WheelBase, float n_Track, float n_WeightDistribution, float n_WheelRadius, float n_WheelWidth)
        {
            m_WheelBase = n_WheelBase;
            m_Track = n_Track;
            m_WheelRadius = n_WheelRadius;
            m_WheelWidth = n_WheelWidth;
            m_WeightDistribution = 0.5f;
            WeightDistribution = n_WeightDistribution;
        }
    }

    struct SuspensionProperties
    {
        // Properties
        public Vector3 UpperContArmA { get => m_UpperContArmA; }
        public Vector3 UpperContArmB { get => m_UpperContArmB; }
        public Vector3 LowerContArmA { get => m_LowerContArmA; }
        public Vector3 LowerContArmB { get => m_LowerContArmB; }
        public Vector3 KingPinTop { get => m_KingPinTop; }
        public Vector3 KingPinBottom { get => m_KingPinBottom; }
        public float StubStartLength { get => m_StubStartLength; }

        //Fields
        private Vector3 m_UpperContArmA, m_UpperContArmB; // mm local
        private Vector3 m_LowerContArmA, m_LowerContArmB; // mm local
        private Vector3 m_KingPinTop, m_KingPinBottom; // mm local
        private float m_StubStartLength; // mm local

        public SuspensionProperties(Vector3 n_UpperContArmA, Vector3 n_UpperContArmB, Vector3 n_LowerContArmA, Vector3 n_LowerContArmB, Vector3 n_KingPinTop, Vector3 n_KingPinBottom, float n_StubStartLength )
        {
            m_UpperContArmA = n_UpperContArmA;
            m_UpperContArmB = n_UpperContArmB;
            m_LowerContArmA = n_LowerContArmA;
            m_LowerContArmB = n_LowerContArmB;
            m_KingPinTop = n_KingPinTop;
            m_KingPinBottom = n_KingPinBottom;
            m_StubStartLength = n_StubStartLength;
        }
    }
}
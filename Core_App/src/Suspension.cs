using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core_App.src
{
    class Suspension
    {
        ////  Input Fields
        //Wheel
        public float WheelRadius; //mm
        public float WheelWidth; //mm
        public float HalfTrack; //m
        //Positions
        public Vector3 ContactPoint { get { return new Vector3(-HalfTrack,0,0); } } //m
        public Vector3 UpperHardPoint; //mm
        public Vector3 LowerHardPoint; //mm
        public Vector3 KingpinTop; //mm
        public Vector3 KingpinBottom; //mm
        public float StubStartLength; //mm
        public Vector3 StubStartPosition { get { return KingpinBottom + Vector3.Normalize(KingpinTop - KingpinBottom) * StubStartLength; } } //mm

        //Calculated Inputs
        public Vector3 WheelCentre; //mm Local
        public Vector3 GlobalWheelCentre { get { return WheelCentre + ContactPoint * 1000f; } } // mm Global

        ////  Output Fields
        public float Camber; //deg

        //Constructor
        public Suspension(float n_WheelRadius, Vector3 n_UpperHardPoint, Vector3 n_LowerHardPoint, Vector3 n_KingpinTop, Vector3 n_KingpinBottom, float n_StubStartLength)
        {
            WheelRadius = n_WheelRadius;
            WheelWidth = 200f;
            HalfTrack = 1f;

            UpperHardPoint = n_UpperHardPoint;
            LowerHardPoint = n_LowerHardPoint; 
            KingpinTop = n_KingpinTop;
            KingpinBottom = n_KingpinBottom;
            StubStartLength = n_StubStartLength;

            WheelCentre = CalculateWheelCentre();
            Camber = CalculateCamber();
        }

        //Methods
        public void Calculate()
        {
            WheelCentre = CalculateWheelCentre();
            Camber = CalculateCamber();
        }

        private Vector3 CalculateWheelCentre()
        {
            Vector3 solution = Vector3.Zero;

            //Vector3 stubStart = KingpinBottom + Vector3.Normalize(KingpinTop - KingpinBottom) * StubStartLength;
            float radius2 = Mag(StubStartPosition)/2f;
            Vector3 c2Centre = Vector3.Normalize(StubStartPosition) * radius2;

            float c = (MathF.Pow(WheelRadius,2) - MathF.Pow(radius2, 2) + MathF.Pow(c2Centre.X, 2) + MathF.Pow(c2Centre.Y, 2)) / (2 * c2Centre.Y);
            float quadA = 1 + (MathF.Pow(c2Centre.X, 2) / MathF.Pow(c2Centre.Y, 2));
            float quadB = (-2 * c2Centre.X * c) / c2Centre.Y;
            float quadC = MathF.Pow(c,2) - MathF.Pow(WheelRadius,2);

            Vector2 xSolutions = SolveQuadratic(quadA, quadB, quadC);

            Console.WriteLine(xSolutions);
            if (MathF.Abs(xSolutions.X) < MathF.Abs(xSolutions.Y)) solution.X = xSolutions.X;
            else solution.X = xSolutions.Y;

            solution.Y =MathF.Sqrt( WheelRadius * WheelRadius - MathF.Sqrt(solution.X));

            return solution;
        }

        private float CalculateCamber()
        {
            float dot = Vector3.Dot(WheelCentre, Vector3.UnitY);
            float angle = MathF.Acos(dot/(Mag(WheelCentre)));
            angle *= 180 / MathF.PI;
            return angle;
        }

        private float Mag(Vector3 v)
        {
            return MathF.Sqrt(MathF.Pow(v.X,2) + MathF.Pow(v.Y,2) + MathF.Pow(v.Z,2));
        }

        private Vector2 SolveQuadratic(float a, float b, float c)
        {
            float d = MathF.Pow(b, 2) - 4 * a * c;
            if (d > 0)
            {
                float plus = (-b + MathF.Sqrt(d)) / (2 * a);
                float minus = (-b - MathF.Sqrt(d)) / (2 * a);
                return new Vector2(plus, minus);
            }else if (d == 0)
            {
                float root = (-b + MathF.Sqrt(d)) / (2 * a);
                return new Vector2(root, root);
            }
            else
            {
                Console.WriteLine("!! Warning: Tried to solve quadratic with no real Roots !!");
                return Vector2.Zero;
            }
        }
    }
}

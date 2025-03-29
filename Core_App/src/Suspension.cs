using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
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
        public Vector3 ContactPoint { get { return new Vector3(-HalfTrack,0,0); } } //m global
        public Vector3 UpperHardPointA, UpperHardPointB, UpperHrdPntAvg; //mm
        public Vector3 LowerHardPointA, LowerHardPointB, LowerHrdPntAvg;//mm
        public Vector3 KingpinTop; //mm
        public Vector3 KingpinBottom; //mm
        public float StubStartLength; //mm
        public Vector3 SpringHardPoint; //mm
        public Vector3 SpringStartPos; //mm
        public float SpringStartLength; //mm
        public Vector3 StubStartPosition { get { return KingpinBottom + Vector3.Normalize(KingpinTop - KingpinBottom) * StubStartLength; } } //mm

        //Spring
        public float SpringStiffness = 1000f;
        public float SpringNaturalLength = 400f;

        //Calculated Fields
        public Vector3 WheelCentre; //mm Local
        public Vector3 GlobalWheelCentre { get { return WheelCentre + ContactPoint * 1000f; } } // mm Global
        public Vector3 SpringEndPosition;

        ////  Output Fields
        public float Camber; //deg
        public float KingPinInclination; //deg
        public Vector3 InstancePoint; // mm local
        public Vector3 RollCentre; // mm global
        public float MotionRatio;
        public float WheelRate;

        //Constructor
        public Suspension(CarProperties car, SuspensionProperties sus)
        {
            WheelRadius = car.WheelRadius;
            WheelWidth = car.WheelWidth;
            HalfTrack = car.Track/2f;

            UpperHardPointA = sus.UpperContArmA;
            UpperHardPointB = sus.UpperContArmB;
            LowerHardPointA = sus.LowerContArmA;
            LowerHardPointB = sus.LowerContArmB;

            KingpinTop = sus.KingPinTop;
            KingpinBottom = sus.KingPinBottom;
            StubStartLength = sus.StubStartLength;

            SpringHardPoint = sus.SpringHrdPnt;
            SpringStartLength = sus.SpringStartLength;

            Calculate();
        }

        //Methods
        public void Calculate()
        {
            CalculateCAMidPoints();
            WheelCentre = CalculateWheelCentre();
            Camber = CalculateCamber();
            CalculateInstacneCentre();
            CalculateRollCentre();
            SpringStartPos = CalculateSpringStartPos();
            CalculateMotionRatio();
            CalculateWheelRate();
        }

        private Vector3 CalculateSpringStartPos()
        {
            Vector3 dir = Vector3.Normalize( KingpinBottom - LowerHrdPntAvg);
            return LowerHrdPntAvg + (dir * SpringStartLength);
        }

        private void CalculateMotionRatio()
        {

            float a = SpringStartLength;
            float b = Mag(LowerHrdPntAvg - KingpinBottom);
            float c = InstancePoint.X - KingpinBottom.X;
            float d = (InstancePoint.X - ContactPoint.X) - ContactPoint.X;

            float mrVertical = (a / b) * (c/d);

            float t = MathF.Acos(Vector3.Dot(Vector3.Normalize(SpringHardPoint - SpringStartPos), Vector3.UnitX)); //inclination of spring
            float cos = MathF.Sin(t);
            MotionRatio = mrVertical * cos;
        }

        private void CalculateWheelRate()
        {
            WheelRate = MotionRatio * MotionRatio * SpringStiffness;
        }

        private void CalculateInstacneCentre()
        {
            Vector3 d1 = Vector3.Normalize(UpperHrdPntAvg - KingpinTop);
            Vector3 p1 = UpperHrdPntAvg;
            Vector3 d2 = Vector3.Normalize(LowerHrdPntAvg - KingpinBottom);
            Vector3 p2 = LowerHrdPntAvg;
            float s = 0f;

            float det = d1.X * d2.Y - d1.Y * d2.X;
            if (det == 0) return;

            Vector3 diff = p2 - p1;

            s = (diff.X * d2.Y - diff.Y * d2.X)/det;

            InstancePoint = ((s * d1) + p1);

        }

        private void CalculateRollCentre()
        {
            Vector3 dir = Vector3.Normalize(1000f* ContactPoint - (InstancePoint - ContactPoint * 1000f));
            float s = -InstancePoint.X/dir.X;

            RollCentre = InstancePoint + s * dir;
        }

        private void CalculateCAMidPoints()
        {
            Vector3 upperDir = UpperHardPointA - UpperHardPointB;
            Vector3 upperMidPnt = UpperHardPointB+ ((Mag(upperDir) / 2f) * Vector3.Normalize(upperDir));
            UpperHrdPntAvg = new Vector3(upperMidPnt.X, upperMidPnt.Y, 0f);

            Vector3 lowerDir = LowerHardPointA - LowerHardPointB;
            Vector3 lowerMidPnt = LowerHardPointB + ((Mag(lowerDir) / 2f) * Vector3.Normalize(lowerDir));
            LowerHrdPntAvg = new Vector3(lowerMidPnt.X, lowerMidPnt.Y, 0f);
        }

        private Vector3 CalculateWheelCentre()
        {
            Vector3 solution = Vector3.Zero;

            float radius2 = Mag(StubStartPosition)/2f;
            Vector3 c2Centre = Vector3.Normalize(StubStartPosition) * radius2;

            float c = (MathF.Pow(WheelRadius,2) - MathF.Pow(radius2, 2) + MathF.Pow(c2Centre.X, 2) + MathF.Pow(c2Centre.Y, 2)) / (2 * c2Centre.Y);
            float quadA = 1 + (MathF.Pow(c2Centre.X, 2) / MathF.Pow(c2Centre.Y, 2));
            float quadB = (-2 * c2Centre.X * c) / c2Centre.Y;
            float quadC = MathF.Pow(c,2) - MathF.Pow(WheelRadius,2);

            Vector2 xSolutions = SolveQuadratic(quadA, quadB, quadC);

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

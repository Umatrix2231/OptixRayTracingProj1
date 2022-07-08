using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using NumSharp;
namespace OptixProject1
{

    [Serializable]
    public class triangle : vec3s
    {

        public float luminosity = 0;
        public bool Seleted = false;
        public float FastRanderFlag = 0;
        public vec3s Color = new vec3s();
        public vec3s P1 = new vec3s();
        public vec3s P2 = new vec3s();
        public vec3s P3 = new vec3s();
        public vec3s N = new vec3s();
        public vec3s VT1 = new vec3s();
        public vec3s VT2 = new vec3s();
        public vec3s VT3 = new vec3s();
        public float Refr = 0;
        public float Trans = 0;
        public float Refl = 0;
        public float MASK = 0;
        public float FOG = 0;
        public float DiffsePassAngle = 0;
        public float DiffsePassChance = 0;
        public byte DiffsePassReturn = 0;
        public void init()
        { 
            N = (P1 - P2).cross(P3 - P2).Norm();
             

            if (Color == new vec3s()) Color = new vec3s(240, 240, 240);
            Color = Color / 255f;

        }

        public vec3s TriangleMin()
        {
            float Xmin = Math.Min(Math.Min(P1.x, P2.x), P3.x);
            float Ymin = Math.Min(Math.Min(P1.x, P2.x), P3.x);
            float Zmin = Math.Min(Math.Min(P1.x, P2.x), P3.x);
            return new vec3s(Xmin, Ymin, Zmin);
        }
        public vec3s TriangleMax()
        {
            float Xmin = Math.Max(Math.Max(P1.x, P2.x), P3.x);
            float Ymin = Math.Max(Math.Max(P1.x, P2.x), P3.x);
            float Zmin = Math.Max(Math.Max(P1.x, P2.x), P3.x);
            return new vec3s(Xmin, Ymin, Zmin);
        }




    }
}

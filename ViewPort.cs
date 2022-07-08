using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptixProject1
{
    [Serializable]
    public class ViewPort : vec3s
    {
        public vec3s Point = new vec3s();
        public float fovW = 90, fovH = 90;
        public float u, v, w;
        public int Wid, Hei;
        public float MissColor1;
        public float MissColor2;
        public float MissColor3;
        public float Missluminosity;

        public ViewPort()
        {
        }
        public ViewPort(vec3s _point, float _u, float _v, float _w)
        {
            Point = _point.Clone();
            u = _u;
            v = _v;
            w = _w;

        }
        public ViewPort Clone()
        {
            return new ViewPort(this.Point, this.u, this.v, this.w);
        }
        public bool ChangedValue(ViewPort VP1)
        {
            if (VP1.u == u && VP1.v == v && VP1.w == w && VP1.Point.x == Point.x && VP1.Point.y == Point.y && VP1.Point.z == Point.z) return false;
            else return true;
        }
        public vec3s LookAt_RayV()
        {
            (vec3s StartPoint, vec3s WV, vec3s HV) = Math2.ScreenGay(this);
            return StartPoint + WV * 0.5f + HV * 0.5f - this.Point;
        }
        public DataEx.Camera_Param_Cuda GetCameraParams()
        {

            (vec3s StartPoint, vec3s WV, vec3s HV) = Math2.ScreenGay(this);

            DataEx.Camera_Param_Cuda Temp = new DataEx.Camera_Param_Cuda();
            Temp.x = Point.x;
            Temp.y = Point.y;
            Temp.z = Point.z;

            vec3s LookAt = StartPoint + WV * 0.5f + HV * 0.5f;
            vec3s LookAtLeft = StartPoint + WV * 0.1f + HV * 0.5f;
            vec3s Ups = new vec3s(0, 0, 1);


            Temp.lookx = LookAt.x;
            Temp.looky = LookAt.y;
            Temp.lookz = LookAt.z;

            Temp.upx = Ups.x;
            Temp.upy = Ups.y;
            Temp.upz = Ups.z;

            Temp.FovW = fovW;
            Temp.FovH = fovH;

            return Temp;
        }
    }
}

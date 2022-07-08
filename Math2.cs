using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using NumSharp;
using System.Threading;
namespace OptixProject1
{
    public  class Math2
    {
        public static void InitTri(List<Model> models, ViewPort camera)
        {
            for (int j = 0; j < models.Count; j++)
                for (int i = 0; i < models[j].tri_list.Count; i++)
                    if (models[j].tri_list[i].Color == new vec3s(1, 1, 1))
                    {
                        camera.Point.x = models[j].tri_list[i].P1.x;
                        camera.Point.y = models[j].tri_list[i].P1.y;
                        camera.Point.z = models[j].tri_list[i].P1.z;
                        models[j].tri_list.RemoveAt(i--); 
                        Console.WriteLine("Set Camera");
                        camera.Point.Print();
                    }
            for (int j = 0; j < models.Count; j++)
            {
                for (int i = 0; i < models[j].tri_list.Count; i++)
                {

                    models[j].tri_list[i].init();
                }
            }
        }
    

        public static object locker = new object();


        public static (vec3s StartPoint, vec3s WV, vec3s HV) ScreenGay(ViewPort camera)
        {



            float Yangle1 = camera.v + camera.fovH / 2f;
            float Yangle2 = camera.v - camera.fovH / 2f;
            float Zangle1 = camera.u + camera.fovW / 2f;
            float Xangle1 = camera.w + 0;

            NDArray Point1 = np.array(new float[] { 1, 0, 0, 1 });

            NDArray Xmatrix1 = np.array(new float[4, 4] {
                { 1, 0, 0, 0 },
                { 0, (float)Math.Cos(Math.PI / 180f * Xangle1), (float)Math.Sin(Math.PI / 180f * Xangle1), 0 },
                { 0, -(float)Math.Sin(Math.PI / 180f * Xangle1), (float)Math.Cos(Math.PI / 180f * Xangle1), 0 },
                { 0, 0, 0, 1 } });
            NDArray Ymatrix1 = np.array(new float[4, 4] {
                { (float)Math.Cos(Math.PI / 180f * Yangle1), 0,- (float)Math.Sin(Math.PI / 180f *Yangle1), 0 },
                { 0, 1, 0, 0 },
                { (float)Math.Sin(Math.PI / 180f * Yangle1), 0, (float)Math.Cos(Math.PI / 180f * Yangle1), 0 },
                { 0, 0, 0, 1 } });
            NDArray Zmatrix1 = np.array(new float[4, 4] {
                { (float)Math.Cos(Math.PI / 180f * Zangle1), (float)Math.Sin(Math.PI / 180f * Zangle1), 0, 0 },
                { -(float)Math.Sin(Math.PI / 180f * Zangle1), (float)Math.Cos(Math.PI / 180f *Zangle1), 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 } });

            NDArray Ymatrix2 = np.array(new float[4, 4] {
                { (float)Math.Cos(Math.PI / 180f * Yangle2), 0,- (float)Math.Sin(Math.PI / 180f *Yangle2), 0 },
                { 0, 1, 0, 0 },
                { (float)Math.Sin(Math.PI / 180f * Yangle2), 0, (float)Math.Cos(Math.PI / 180f * Yangle2), 0 },
                { 0, 0, 0, 1 } });

            NDArray Translation = np.array(new float[4, 4] {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 } });




            var R1 = np.matmul(Ymatrix1, Point1);
            R1 = np.matmul(Xmatrix1, R1);
            R1 = np.matmul(Zmatrix1, R1);

            var R2 = np.matmul(Ymatrix2, Point1);
            R2 = np.matmul(Xmatrix1, R2);
            R2 = np.matmul(Zmatrix1, R2);



            var FT1 = (float[,])(R1.ToMuliDimArray<float>());
            var FT2 = (float[,])(R2.ToMuliDimArray<float>());

            vec3s RayV1 = new vec3s(FT1[0, 0], FT1[1, 0], FT1[2, 0]);
            vec3s RayV3 = new vec3s(FT2[0, 0], FT2[1, 0], FT2[2, 0]); 

            vec3s WV = (RayV1).cross(RayV3).Norm();//横面矢量
            vec3s HV = (RayV3 - RayV1).Norm();//竖面矢量  

            float TDdist = RayV1.Dist(RayV3); 

            float Wdist = TDdist * (camera.fovW / camera.fovH);// WAline * 2f;



            vec3s StartPoint = camera.Point + RayV1 + (WV * Wdist / 2f);

            WV *= -Wdist;
            HV *= TDdist;

            return (StartPoint, WV, HV);
        }



        public static vec3s UVRay(float U,float V)
        {
            float DX, DY, DZ;
            float X = 1f, Y = 0f, Z = 0;
             
            DZ = (float)Math.Sin(Math.PI / 180f * V) * X;
            DX = (float)Math.Cos(Math.PI / 180f * V) * X;

             
            DY = -(float)Math.Sin(Math.PI / 180f * U) * DX;
            DZ = (float)Math.Cos(Math.PI / 180f * U) * DX;

            return new vec3s(DX, DY, DZ);
        }
        public static float[] GetM12_Fast(float R_x, float R_y, float R_z, float T_x, float T_y, float T_z, float K_x = 1, float K_y = 1, float K_z = 1)
        {

            float DEG_TO_RAD = (float)Math.PI / 180f;
            R_x = R_x * DEG_TO_RAD;
            R_y = R_y * DEG_TO_RAD;
            R_z = R_z * DEG_TO_RAD;
            float SinRx = (float)Math.Sin(R_x);
            float SinRy = (float)Math.Sin(R_y);
            float SinRz = (float)Math.Sin(R_z);
            float CosRx = (float)Math.Cos(R_x);
            float CosRy = (float)Math.Cos(R_y);
            float CosRz = (float)Math.Cos(R_z);

            float[] m = new float[12];

            m[0] = K_x * (SinRx * SinRy * SinRz + CosRy * CosRz);
            m[1] = K_y * SinRz * CosRx;
            m[2] = K_z * (SinRx * SinRz * CosRy - SinRy * CosRz);
            m[3] = T_x;
            m[4] = K_x * (SinRx * SinRy * CosRz - SinRz * CosRy);
            m[5] = K_y * CosRx * CosRz;
            m[6] = K_z * (SinRx * CosRy * CosRz + SinRy * SinRz);
            m[7] = T_y;
            m[8] = K_x * SinRy * CosRx;
            m[9] = (-K_y) * SinRx;
            m[10] = K_z * CosRx * CosRy;
            m[11] = T_z;

            return m;
        }
        public static (vec3s StartPoint, vec3s WV, vec3s HV) ScreenRay(ViewPort camera)
        {
             


            float Yangle1 = camera.v + camera.fovH / 2f;
            float Yangle2 = camera.v - camera.fovH / 2f;
            float Zangle1 = camera.u + camera.fovW / 2f; 
            float Xangle1 = camera.w + 0;
            
            NDArray Point1 = np.array(new float[] { 1, 0, 0, 1 });

            NDArray Xmatrix1 = np.array(new float[4, 4] {
                { 1, 0, 0, 0 },
                { 0, (float)Math.Cos(Math.PI / 180f * Xangle1), (float)Math.Sin(Math.PI / 180f * Xangle1), 0 },
                { 0, -(float)Math.Sin(Math.PI / 180f * Xangle1), (float)Math.Cos(Math.PI / 180f * Xangle1), 0 },
                { 0, 0, 0, 1 } });
            NDArray Ymatrix1 = np.array(new float[4, 4] {
                { (float)Math.Cos(Math.PI / 180f * Yangle1), 0,- (float)Math.Sin(Math.PI / 180f *Yangle1), 0 },
                { 0, 1, 0, 0 },
                { (float)Math.Sin(Math.PI / 180f * Yangle1), 0, (float)Math.Cos(Math.PI / 180f * Yangle1), 0 },
                { 0, 0, 0, 1 } });
            NDArray Zmatrix1 = np.array(new float[4, 4] {
                { (float)Math.Cos(Math.PI / 180f * Zangle1), (float)Math.Sin(Math.PI / 180f * Zangle1), 0, 0 },
                { -(float)Math.Sin(Math.PI / 180f * Zangle1), (float)Math.Cos(Math.PI / 180f *Zangle1), 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 } });

            NDArray Ymatrix2 = np.array(new float[4, 4] {
                { (float)Math.Cos(Math.PI / 180f * Yangle2), 0,- (float)Math.Sin(Math.PI / 180f *Yangle2), 0 },
                { 0, 1, 0, 0 },
                { (float)Math.Sin(Math.PI / 180f * Yangle2), 0, (float)Math.Cos(Math.PI / 180f * Yangle2), 0 },
                { 0, 0, 0, 1 } });




            var R1 = np.matmul(Ymatrix1, Point1); 
            R1 = np.matmul(Xmatrix1, R1);
            R1 = np.matmul(Zmatrix1, R1); 

            var R2 = np.matmul(Ymatrix2, Point1); 
            R2 = np.matmul(Xmatrix1, R2);
            R2 = np.matmul(Zmatrix1, R2); 



            var FT1 = (float[,])(R1.ToMuliDimArray<float>());
            var FT2 = (float[,])(R2.ToMuliDimArray<float>());

            vec3s RayV1 = new vec3s(FT1[0,0], FT1[1,0], FT1[2,0]);
            vec3s RayV3 = new vec3s(FT2[0,0], FT2[1,0], FT2[2,0]);
             

            vec3s WV = (RayV1).cross(RayV3).Norm(); 
            vec3s HV = (RayV3 - RayV1).Norm();   

            float TDdist = RayV1.Dist(RayV3);
            float Wdist = TDdist * (camera.fovW / camera.fovH);

            vec3s StartPoint = camera.Point + RayV1 + (WV * Wdist / 2f);

            WV *=- Wdist;
            HV *= TDdist;

            return (StartPoint, WV, HV);
        }
            
      
    }
}

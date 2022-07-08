using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Threading;
using NumSharp; 
using System.Runtime.InteropServices; 
namespace OptixProject1
{
    internal class InitSence
    {

        public static int SenceRoom(List<Model> models, ViewPort camera, float wid, float hei,ListView Lv)
        {
            models.AddRange(GetModels.OBJMODEL(@"objs/room1.obj"));
            models.Last().tri_list.ForEach(s =>
            {
                if (s.Color == new vec3s(0x95, 0xDB, 0xFE))
                {
                    s.luminosity = 1000f;
                }

                if (s.Color == new vec3s(0xF4, 0xF9, 0xFC))
                {
                    s.Refl = 0.45f;
                }
                if (s.Color == new vec3s(0xE7, 0xC9, 0x8A))
                {
                    s.Refr = 1.5f;
                    s.Trans = 0.95f;
                }
                if (s.Color == new vec3s(0xE8, 0xC7, 0x7F))
                {
                    s.Refr = 1.5f;
                    s.Trans = 0.95f;
                }
                if (s.Color == new vec3s(0xC6, 0x88, 0x92))
                {
                    s.Refl = 0.25f;
                }
                if (s.Color == new vec3s(0xFE, 0x95, 0x9C) || s.Color == new vec3s(0x95, 0xAF, 0xFE))
                {
                    s.DiffsePassChance = 0.7f;
                    s.DiffsePassAngle = 0.7f;
                }

            });
            models.AddRange(GetModels.OBJMODEL(@"objs/酒杯.obj"));
            models.Last().tri_list.ForEach(s =>
            {
                s.Color = new vec3s(252, 252, 252);
                s.Refr = 1.5f;
                s.Trans = 0.95f;
            });


            models.AddRange(GetModels.STLMODEL(@"objs/球.stl"));
            models.Last().tri_list.ForEach(s =>
            {
                s.Color = new vec3s(252, 152, 52);
                s.Refr = 1.5f;
                s.Trans = 0.97f;
            });
            models.AddRange(GetModels.STLMODEL(@"objs/球.stl"));
            models.Last().tri_list.ForEach(s =>
            {
                s.Color = new vec3s(252, 252, 252);
                s.Refl = 0.9f;
            });


            models.AddRange(GetModels.OBJMODELVT(@"skybox/earth.obj", @"skybox/0027.exr", ComputeSrcLight: true, SkyBox: true));
            models.Last().tri_list.ForEach(s =>
            {
                s.P1 *= 100000;
                s.P2 *= 100000;
                s.P3 *= 100000;
            });
            models.Last().Rotation.x = -90;


            //models.AddRange(GetModels.CircleLight(MaxRadios: 1000000, 45, 45, luminosity: 7000, Angle: 2, new vec3s(153, 172, 252)));





            camera.Point.x = -100;
            camera.Point.y = -100;
            camera.Point.z = 20;
            camera.Wid = (int)wid;
            camera.Hei = (int)hei;
            camera.fovW = 90;
            camera.fovH = hei / wid * camera.fovW;
            camera.u = 0;
            camera.v = 0;

            var RayV = Math2.UVRay(45, 45).Norm();
            DataEx.SkyLight SL = new DataEx.SkyLight();
            SL.MissColor1 = 253f / 255f;
            SL.MissColor2 = 232f / 255f;
            SL.MissColor3 = 222f / 255f;
            SL.Missluminosity = 150f;
            SL.LightNx = RayV.x;
            SL.LightNy = RayV.y;
            SL.LightNz = RayV.z;

            Math2.InitTri(models, camera);
            int TriCount = models.Sum(s => s.tri_list.Count());

            CudaOperation.InitCuda(camera);

            CudaOperation.OptixInit(camera.Wid, camera.Hei, models, SL);


             

            

            return TriCount;
        }

        public static int SenceNubian(List<Model> models, ViewPort camera, float wid, float hei)
        {


            models.AddRange(GetModels.OBJMODELVT(@"植物/bflow1.obj", @"植物/bflow1.jpg"));
            models.AddRange(GetModels.OBJMODELVT(@"植物/grass1.obj", @"植物/grass1_0.jpg"));
            models.AddRange(GetModels.OBJMODELVT(@"植物/treevt3.obj", @"植物/treevt3.jpg"));
            models.AddRange(GetModels.OBJMODELVT(@"植物/treevt4.obj", @"植物/treevt4.jpg"));

            models.AddRange(GetModels.OBJMODELVT(@"建筑/nubian1.obj", @"建筑/nubian1.jpg"));
            models.Last().tri_list.ForEach(s =>
            {
                s.Refl = 0.45f;
            });

            models.AddRange(GetModels.OBJMODELVT(@"skybox/earth.obj", @"skybox/0027.exr", ComputeSrcLight: true, SkyBox: true));
            models.Last().tri_list.ForEach(s =>
            {
                s.P1 *= 100000;
                s.P2 *= 100000;
                s.P3 *= 100000;
            });
            models.Last().Rotation.x = -90;


            models.AddRange(GetModels.OBJMODELVT(@"机械/ship1_2.obj", @"机械/ship1.png"));
            models.Last().tri_list.ForEach(s =>
            {
                s.Refl = 0.65f;
            });

            //models.AddRange(GetModels.OBJMODEL(@"skybox.obj"));
            models.AddRange(GetModels.OBJMODEL(@"建筑/phouse.obj"));
            models.Last().tri_list.ForEach(s =>
            {
                s.Color = new vec3s(222, 222, 222);
            });
            models.AddRange(GetModels.OBJMODELVT(@"地形/destg.obj", "地形/destg.jpg"));
            models.AddRange(GetModels.OBJMODEL(@"地形/ocean.obj"));
            models.Last().tri_list.ForEach(s =>
            {
                s.Color = new vec3s(252, 202, 150);
                s.Refr = 1.33f;
                s.Trans = 0.97f;
            });
            models.AddRange(GetModels.OBJMODELVT(@"角色/091_Aya.obj", @"角色/091_Aya.jpg")); models.Last().Rotation.x = -90;
            models.AddRange(GetModels.OBJMODELVT(@"角色\00208_Quint009.OBJ", @"角色\00208_Quint009_Diffuse.JPG")); models.Last().Rotation.x = -90;
            models.AddRange(GetModels.OBJMODELVT(@"角色\00219_Paul008.OBJ", @"角色\00219_Paul008_Diffuse.JPG")); models.Last().Rotation.x = -90;
            models.AddRange(GetModels.OBJMODELVT(@"角色\00218_Jon005.OBJ", @"角色\00218_Jon005_Diffuse.jpg")); models.Last().Rotation.x = -90;
            models.AddRange(GetModels.OBJMODELVT(@"角色\00246_Zeef006.OBJ", @"角色\00246_Zeef006_Diffuse.jpg")); models.Last().Rotation.x = -90;
            models.AddRange(GetModels.OBJMODELVT(@"角色\00256_Daud008.OBJ", @"角色\00256_Daud008_Diffuse.jpg")); models.Last().Rotation.x = -90;

            models.AddRange(GetModels.OBJMODEL("sofa11.obj"));



            //  models.AddRange(GetModels.OBJMODELVT("wood1.obj", "white.jpg", "black.jpg", "black.jpg"));
            // models.AddRange(GetModels.OBJMODELVT("wood2.obj", "db1.jpg", "black.jpg", "black.jpg"));


            //models.AddRange(GetModels.CircleLight(MaxRadios: 1000000, 45, 45, luminosity: 7000, Angle: 2, new vec3s(213, 232, 252)));




            camera.Point.x = -100;
            camera.Point.y = -100;
            camera.Point.z = 20;
            camera.Wid = (int)wid;
            camera.Hei = (int)hei;
            camera.fovW = 90;
            camera.fovH = hei / wid * camera.fovW;
            camera.u = 0;
            camera.v = 0;

            var RayV = Math2.UVRay(45, 45).Norm();
            DataEx.SkyLight SL = new DataEx.SkyLight();
            SL.MissColor1 = 253f / 255f;
            SL.MissColor2 = 232f / 255f;
            SL.MissColor3 = 222f / 255f;
            SL.Missluminosity = 150f;
            SL.LightNx = RayV.x;
            SL.LightNy = RayV.y;
            SL.LightNz = RayV.z;

            Math2.InitTri(models, camera);
            int TriCount = models.Sum(s => s.tri_list.Count());

            CudaOperation.InitCuda(camera);

            CudaOperation.OptixInit(camera.Wid, camera.Hei, models, SL);
            return TriCount;
        }
        public static int 花台场景(List<Model> models, ViewPort camera, float wid, float hei)
        {
            models.AddRange(GetModels.OBJMODELVT(@"skybox/earth.obj", @"skybox/0027.exr", ComputeSrcLight: true, SkyBox: true));
            models.Last().tri_list.ForEach(s =>
            {
                s.P1 *= 10000000;
                s.P2 *= 10000000;
                s.P3 *= 10000000;
            });
            models.Last().Rotation.x = -90;


            models.AddRange(GetModels.OBJMODEL(@"建筑/nd1.obj"));
            models.Last().tri_list.ForEach(s =>
            {
                if (s.Color == new vec3s(0xFF, 0x8C, 0x8C))
                {
                    s.Refr = 1.5f;
                    s.Trans = 0.95f;
                }
                else if (s.Color == new vec3s(0xD1, 0xE3, 0xFF))
                {
                    s.luminosity = 1000;
                }
                else if (s.Color == new vec3s(0x80, 0x80, 0x80))
                {
                    s.Refl = 0.25f;
                }
                else if (s.Color == new vec3s(0x66, 0xCC, 0xFF))
                {
                    s.Refl = 0.65f;
                }
                else if (s.Color == new vec3s(0xFD, 0xFC, 0xFF))
                {
                    s.Refl = 0.45f;
                }
                else
                {
                    s.DiffsePassChance = 0.13f;
                    s.DiffsePassAngle = 0.5f;
                    s.DiffsePassReturn = 1;
                }

            });
            models.AddRange(GetModels.OBJMODELVT(@"建筑/雕像1.obj", @"建筑/雕像1.jpg"));
            models.Last().tri_list.ForEach(s =>
            {
                s.DiffsePassChance = 0.33f;
                s.DiffsePassAngle = 0.5f;
                s.DiffsePassReturn = 1;
            });

            models.AddRange(GetModels.OBJMODEL(@"建筑/花台1.obj"));
            models.Last().tri_list.ForEach(s =>
            {
                if (s.Color == new vec3s(0xE8, 0xDD, 0xC7))
                {
                    s.Refr = 1.33f;
                    s.Trans = 0.95f;
                }
                if (s.Color == new vec3s(0xB7, 0xD0, 0xFF))
                {
                    s.luminosity = 1000f;
                }
                if (s.Color == new vec3s(0xF4, 0xEB, 0xDB))
                {
                    s.Refr = 0.15f;
                }

            });
            models.AddRange(GetModels.OBJMODELVT(@"植物/bflow1.obj", @"植物/bflow1.jpg"));
            models.Last().tri_list.ForEach(s =>
            {
                s.DiffsePassChance = 0.33f;
                s.DiffsePassAngle = 0.25f;
                s.DiffsePassReturn = 1;
            });
            models.AddRange(GetModels.OBJMODELVT(@"植物/grass1.obj", @"植物/grass1_0.jpg"));
            models.Last().tri_list.ForEach(s =>
            {
                s.DiffsePassChance = 0.33f;
                s.DiffsePassAngle = 0.25f;
                s.DiffsePassReturn = 1;
                //s.反射率 = 0.01f;
            });





            models.AddRange(GetModels.OBJMODEL(@"灯具/d1.obj"));
            models.Last().tri_list.ForEach(s =>
            {
                if (s.Color == new vec3s(0x80, 0x80, 0x80))
                {
                    s.Refl = 0.2f;
                }
                if (s.Color == new vec3s(0xFC, 0xFD, 0xFF))
                {
                    s.Refl = 0.45f;
                }
                if (s.Color == new vec3s(0x66, 0xCC, 0xFF))
                {
                    s.Refl = 0.65f;
                }
                if (s.Color == new vec3s(0x80, 0x80, 0x80))
                {
                    s.Refl = 0.05f;
                }
                if (s.Color == new vec3s(0xD8, 0xE4, 0xFF))
                {
                    s.luminosity = 1000;
                }
                if (s.Color == new vec3s(0xF2, 0xF5, 0xF7))
                {
                    s.Refr = 1.5f;
                    s.Trans = 0.95f;
                }

            });
            models.AddRange(GetModels.OBJMODEL(@"灯具/d2.obj"));
            models.Last().tri_list.ForEach(s =>
            {
                if (s.Color == new vec3s(0x80, 0x80, 0x80))
                {
                    s.Refl = 0.2f;
                }
                if (s.Color == new vec3s(0xFC, 0xFD, 0xFF))
                {
                    s.Refl = 0.45f;
                }
                if (s.Color == new vec3s(0x66, 0xCC, 0xFF))
                {
                    s.Refl = 0.65f;
                }
                if (s.Color == new vec3s(0x80, 0x80, 0x80))
                {
                    s.Refl = 0.05f;
                }
                if (s.Color == new vec3s(0xD8, 0xE4, 0xFF))
                {
                    s.luminosity = 1000;
                }

            });
            models.AddRange(GetModels.OBJMODEL(@"物品/像框架.obj"));
            models.Last().tri_list.ForEach(s =>
            {
                if (s.Color == new vec3s(0x66, 0xCC, 0xFF))
                {
                    s.Refl = 0.65f;
                }
                else s.Refl = 0.45f;
            });
            models.AddRange(GetModels.OBJMODELVT(@"物品/画框.obj", @"物品/拿破仑.jpg"));
            models.Last().tri_list.ForEach(s =>
            {
                s.Refl = 0.01f;
            });
            models.AddRange(GetModels.OBJMODELVT(@"物品/画框.obj", @"物品/雅典学院.jpg"));
            models.Last().tri_list.ForEach(s =>
            {
                s.Refl = 0.01f;
            });
            models.AddRange(GetModels.OBJMODELVT(@"物品/画框.obj", @"物品/肯特海滩.jpg"));
            models.Last().tri_list.ForEach(s =>
            {
                s.Refl = 0.01f;
            });


            models.AddRange(GetModels.OBJMODELVT(@"地形/oceanvt.obj", "地形/forrest_ground_01_diff_2k.jpg"));
            models.AddRange(GetModels.OBJMODEL(@"地形/ocean.obj"));
            models.Last().tri_list.ForEach(s =>
            {
                s.Color = new vec3s(252, 202, 150);
                s.Refr = 1.33f;
                s.Trans = 0.95f;
            });

            models.AddRange(GetModels.OBJMODEL(@"物品/椅子1.obj"));
            models.Last().tri_list.ForEach(s =>
            {
                if (s.Color == new vec3s(0xD8, 0xDD, 0xD9))
                {
                    s.Refl = 0.55f;
                }
                if (s.Color == new vec3s(0xE7, 0xB4, 0x5C))
                {
                    s.Refl = 0.55f;
                }
                if (s.Color == new vec3s(0xA4, 0x76, 0xA3))
                {
                    s.Refl = 0.55f;
                }
                if (s.Color == new vec3s(0x11, 0x11, 0x28))
                {
                    s.Refl = 0.05f;
                }
            });

            models.AddRange(GetModels.OBJMODELVT(@"角色/091_Aya.obj", @"角色/091_Aya.jpg")); models.Last().Rotation.x = -90;
            models.Last().tri_list.ForEach(s =>
            {
                s.DiffsePassChance = 0.05f;
                s.DiffsePassAngle = 0.5f;
                s.DiffsePassReturn = 1;
            });
            models.AddRange(GetModels.OBJMODELVT(@"角色\00208_Quint009.OBJ", @"角色\00208_Quint009_Diffuse.JPG")); models.Last().Rotation.x = -90;
            models.Last().tri_list.ForEach(s =>
            {
                s.DiffsePassChance = 0.05f;
                s.DiffsePassAngle = 0.5f;
                s.DiffsePassReturn = 1;
            });
            models.AddRange(GetModels.OBJMODELVT(@"角色\00219_Paul008.OBJ", @"角色\00219_Paul008_Diffuse.JPG")); models.Last().Rotation.x = -90;
            models.Last().tri_list.ForEach(s =>
            {
                s.DiffsePassChance = 0.05f;
                s.DiffsePassAngle = 0.5f;
                s.DiffsePassReturn = 1;
            });
            models.AddRange(GetModels.OBJMODELVT(@"角色\00218_Jon005.OBJ", @"角色\00218_Jon005_Diffuse.jpg")); models.Last().Rotation.x = -90;
            models.Last().tri_list.ForEach(s =>
            {
                s.DiffsePassChance = 0.05f;
                s.DiffsePassAngle = 0.5f;
                s.DiffsePassReturn = 1;
            });
            models.AddRange(GetModels.OBJMODELVT(@"角色\00246_Zeef006.OBJ", @"角色\00246_Zeef006_Diffuse.jpg")); models.Last().Rotation.x = -90;
            models.Last().tri_list.ForEach(s =>
            {
                s.DiffsePassChance = 0.05f;
                s.DiffsePassAngle = 0.5f;
                s.DiffsePassReturn = 1;
            });
            models.AddRange(GetModels.OBJMODELVT(@"角色\00256_Daud008.OBJ", @"角色\00256_Daud008_Diffuse.jpg")); models.Last().Rotation.x = -90;
            models.Last().tri_list.ForEach(s =>
            {
                s.DiffsePassChance = 0.05f;
                s.DiffsePassAngle = 0.5f;
                s.DiffsePassReturn = 1;
            });


            models.AddRange(GetModels.OBJMODEL(@"物品\窗帘.OBJ"));
            models.Last().tri_list.ForEach(s =>
            {
                if (s.Color == new vec3s(0xB5, 0xA5, 0x00))
                {
                    s.DiffsePassAngle = 0.7f;
                    s.DiffsePassChance = 0.8f;
                }
                else
                {
                    s.DiffsePassChance = 0.33f;
                    s.DiffsePassAngle = 0.5f;
                    s.DiffsePassReturn = 1;
                }
            });


            models.AddRange(GetModels.OBJMODEL(@"建筑\桥.OBJ"));
            models.Last().tri_list.ForEach(s =>
            {
                if (s.Color == new vec3s(0x4C, 0xBB, 0x21))
                {
                    s.Refr = 1.5f;
                    s.Trans = 0.95f;
                }
                else
                {
                    s.Color = new vec3s(198, 198, 198);
                    s.DiffsePassChance = 0.05f;
                    s.DiffsePassAngle = 0.5f;
                    s.DiffsePassReturn = 1;
                }
            });

            models.AddRange(GetModels.OBJMODEL(@"建筑/女雕像1.obj"));
            models.Last().tri_list.ForEach(s =>
            {
                // s.反射率 = 0.65f;
                s.Refr = 1.5f;
                s.Trans = 0.95f;
            });

            models.AddRange(GetModels.OBJMODEL(@"机械/rockets1.obj"));
            models.Last().tri_list.ForEach(s =>
            {
                if (s.Refr == 0)
                {
                    s.Refl = 0.65f;
                }

            });
            models.AddRange(GetModels.OBJMODEL(@"物品/台1.obj"));
            models.Last().tri_list.ForEach(s =>
            {
                s.Refr = 1.5f;
                s.Trans = 0.95f;

            });

            models.AddRange(GetModels.OBJMODEL(@"机械/rocketlaucher.obj"));
            models.Last().tri_list.ForEach(s =>
            {
                if (s.Refr == 0)
                {
                    s.Refl = 0.65f;
                }

            });
            models.AddRange(GetModels.OBJMODELVT(@"机械/ship1_2.obj", @"机械/ship1.png"));
            models.Last().tri_list.ForEach(s =>
            {
                s.Refl = 0.65f;
            });


            models.AddRange(GetModels.OBJMODEL(@"物品/太阳能板1.obj"));
            models.Last().tri_list.ForEach(s =>
            {
                if (s.Color == new vec3s(0x97, 0x4D, 0x29))
                {
                    s.Refl = 0.25f;
                }
                else if (s.Color == new vec3s(0x66, 0xCC, 0xFF))
                {
                    s.Refl = 0.65f;
                }
                else
                {
                    s.Refl = 0.45f;
                }
            });

            models.AddRange(GetModels.OBJMODEL(@"物品/瓶子1.obj"));
            models.Last().tri_list.ForEach(s =>
            {
                if (s.luminosity != 0) s.luminosity = 1000;
                else
                {
                    s.Refr = 1.5f;
                    s.Trans = 0.95f;
                    s.Color = new vec3s(245, 245, 245);
                }
            });

            /*
            models.AddRange(GetModels.OBJMODELVT(@"植物/treevt3.obj", @"植物/treevt3.jpg"));
            models.Last().tri_list.ForEach(s =>
            {
                s.DiffsePassChance = 0.05f;
                s.DiffsePassAngle = 0.5f;
                s.DiffsePassReturn = 1;
            });
            models.AddRange(GetModels.OBJMODELVT(@"植物/treevt4.obj", @"植物/treevt4.jpg"));
            models.Last().tri_list.ForEach(s =>
            {
                s.DiffsePassChance = 0.05f;
                s.DiffsePassAngle = 0.5f;
                s.DiffsePassReturn = 1;
            });
            */






            camera.Point.x = -100;
            camera.Point.y = -100;
            camera.Point.z = 20;
            camera.Wid = (int)wid;
            camera.Hei = (int)hei;
            camera.fovW = 118;
            camera.fovH = hei / wid * camera.fovW;
            camera.u = 0;
            camera.v = 0;

            var RayV = Math2.UVRay(45, 45).Norm();
            DataEx.SkyLight SL = new DataEx.SkyLight();
            SL.MissColor1 = 253f / 255f;
            SL.MissColor2 = 232f / 255f;
            SL.MissColor3 = 222f / 255f;
            SL.Missluminosity = 150f;
            SL.LightNx = RayV.x;
            SL.LightNy = RayV.y;
            SL.LightNz = RayV.z;

            Math2.InitTri(models, camera);
            int TriCount = models.Sum(s => s.tri_list.Count());

            CudaOperation.InitCuda(camera);

            CudaOperation.OptixInit(camera.Wid, camera.Hei, models, SL);
            return TriCount;
        }
        public static int TestSence(List<Model> models, ViewPort camera, float wid, float hei)
        {

            /*
            models.AddRange(GetModels.OBJMODEL(@"物品/折射望远镜2.obj")); 
            models.Last().tri_list.ForEach(s =>
            { 
                s.P1 *= 0.1f;
                s.P2 *= 0.1f;
                s.P3 *= 0.1f;
              

                if (s.Color == new vec3s(0xA4, 0x49, 0xA3))
                {
                    s.反射率 = 0.65f;
                }
                if (s.Color == new vec3s(0x58, 0xB7, 0x58))
                {
                    s.折射率 = 1.5f;
                    s.透光率 = 0.95f;
                    s.Color = new vec3s(245, 245, 245);
                }
                
            });  
            */

            models.AddRange(GetModels.OBJMODELVT(@"wood1.obj", "wood1.jpg"));
            models.Last().tri_list.ForEach(s =>
            {

            });

            models.AddRange(GetModels.STLMODEL(@"fo1.stl"));
            models.Last().tri_list.ForEach(s =>
            {
                s.Color = new vec3s(255, 55, 0);
                s.Refr = 1.6f;
                s.Trans = 0.95f;
                s.luminosity = 100f;
            });

            /*
            models.AddRange(GetModels.OBJMODEL(@"物品/太阳能板1.obj"));
            models.Last().tri_list.ForEach(s =>
            {
                if (s.Color == new vec3s(0x97, 0x4D, 0x29))
                {
                    s.反射率 = 0.25f;
                }
                else if (s.Color == new vec3s(0x66, 0xCC, 0xFF))
                {
                    s.反射率 = 0.65f;
                }
                else
                {
                    s.反射率 = 0.45f;
                }
            });*/

            /*
            models.AddRange(GetModels.OBJMODELVT(@"skybox/earth.obj", @"skybox/0027.exr", ComputeSrcLight: true, SkyBox: true));
            models.Last().tri_list.ForEach(s =>
            {
                s.P1 *= 10000000;
                s.P2 *= 10000000;
                s.P3 *= 10000000;
            });
            models.Last().Rotation.x = -90;*/




            //models.AddRange(GetModels.OBJMODELVT(@"地形/destg.obj", @"地形/destg.jpg")); 

            //  models.AddRange(GetModels.CircleLight(MaxRadios: 1000000, 45, 45, luminosity: 7000, Angle: 2, new vec3s(193, 222, 252)));


            camera.Point.x = -100;
            camera.Point.y = -100;
            camera.Point.z = 20;
            camera.Wid = (int)wid;
            camera.Hei = (int)hei;
            camera.fovW = 118;
            camera.fovH = hei / wid * camera.fovW;
            camera.u = 0;
            camera.v = 0;

            var RayV = Math2.UVRay(45, 45).Norm();
            DataEx.SkyLight SL = new DataEx.SkyLight();
            SL.MissColor1 = 253f / 255f;
            SL.MissColor2 = 232f / 255f;
            SL.MissColor3 = 222f / 255f;
            SL.Missluminosity = 150f;
            SL.LightNx = RayV.x;
            SL.LightNy = RayV.y;
            SL.LightNz = RayV.z;

            Math2.InitTri(models, camera);
            int TriCount = models.Sum(s => s.tri_list.Count());

            CudaOperation.InitCuda(camera);

            CudaOperation.OptixInit(camera.Wid, camera.Hei, models, SL);
            return TriCount;
        }
    }
}

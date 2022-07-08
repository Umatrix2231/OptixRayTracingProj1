using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptixProject1
{
    public class GetModels
    {
        private static vec3s Compute(Model model)
        {
            float XAVG = model.tri_list.Average(s => (s.P1.x + s.P2.x + s.P3.x) / 3f);
            float YAVG = model.tri_list.Average(s => (s.P1.y + s.P2.y + s.P3.y) / 3f);
            float ZAVG = model.tri_list.Average(s => (s.P1.z + s.P2.z + s.P3.z) / 3f);

            vec3s vec3S = new vec3s(XAVG, YAVG, ZAVG);

            return vec3S;
        }
        private static void SetDiffZ(Model model)
        {
            float MINZ = model.tri_list.Average(s => Math.Min(Math.Min(s.P1.z, s.P2.z), s.P3.z));
            model.DiffZ = model.Point.z - MINZ;
        }
        public static Model[] STLMODEL(string file)
        {
            Model models = Model_READER.Read_STL(file);

            {
                vec3s vec3S = Compute(models);

                models.tri_list.ForEach(s => { s.P1 -= vec3S; s.P2 -= vec3S; s.P3 -= vec3S; });

                models.Point = vec3S.Clone();

                SetDiffZ(models);
                vec3S.Print(file + ":");
            }

            return new[] { models };
        }
        public static Model[] OBJMODEL(string file, bool Combind = true)
        {
            Model[] models = Model_READER.Read_OBJ(file, Combind);

            for (int i = 0; i < models.Length; i++)
            {
                vec3s vec3S = Compute(models[i]);

                models[i].tri_list.ForEach(s => { s.P1 -= vec3S; s.P2 -= vec3S; s.P3 -= vec3S; });

                models[i].Point = vec3S.Clone();

                SetDiffZ(models[i]);
                vec3S.Print(file + ":");
            }

            return models;
        }
        public static Model[] OBJMODELVT(string file, string texture, string Refl = "", string Refr = "", bool ComputeSrcLight = false, bool SkyBox = false)
        {
            Model[] models = Model_READER.Read_OBJVT(file, texture, Refl, Refr, ComputeSrcLight, SkyBox);

            for (int i = 0; i < models.Length; i++)
            {
                vec3s vec3S = Compute(models[i]);

                models[i].tri_list.ForEach(s => { s.P1 -= vec3S; s.P2 -= vec3S; s.P3 -= vec3S; });

                models[i].Point = vec3S.Clone();

                SetDiffZ(models[i]);
                vec3S.Print(file + ":");
            }

            return models;
        }
        public static Model[] MTLMODEL(string file, string mtlfile, bool Combind = true)
        {
            Model[] models = Model_READER.Read_MTL(file, mtlfile, Combind);

            for (int i = 0; i < models.Length; i++)
            {
                vec3s vec3S = Compute(models[i]);

                models[i].tri_list.ForEach(s => { s.P1 -= vec3S; s.P2 -= vec3S; s.P3 -= vec3S; });

                models[i].Point = vec3S.Clone();

                SetDiffZ(models[i]);
                vec3S.Print(file + ":");
            }

            return models;
        }


        public static Model ToCenterAll(Model models)
        {
            float ZAVG = models.tri_list.Average(s => (s.P1.z + s.P2.z + s.P3.z) / 3f);
            float XAVG = models.tri_list.Average(s => (s.P1.x + s.P2.x + s.P3.x) / 3f);
            float YAVG = models.tri_list.Average(s => (s.P1.y + s.P2.y + s.P3.y) / 3f);

            vec3s vec3S = new vec3s(XAVG, YAVG, ZAVG);

            models.tri_list.ForEach(s => { s.P1 -= vec3S; s.P2 -= vec3S; s.P3 -= vec3S; });

            return models;
        }

        public static Model[] CircleLight(float MaxRadios, float LightU, float LightV, float luminosity, float Angle, vec3s Color)
        {



            float MaxRadio = MaxRadios;
            var ToPoint = Math2.UVRay(LightU, LightV) * MaxRadios;
            float B = ToPoint.Dist(new vec3s());
            float C = B / (float)Math.Cos(Angle / 180d * Math.PI);
            float A = (float)Math.Sqrt(C * C - B * B) * 2f / 10f;

            var modelsLight = OBJMODEL("lightcirecle2.obj");

            modelsLight.Last().tri_list.ForEach(s => { s.P1 *= A; s.P2 *= A; s.P3 *= A; });



            modelsLight.Last().tri_list.ForEach((s) =>
            {
                s.Color = Color;
                s.luminosity = luminosity;
            });

            modelsLight[0].Point = ToPoint;



            return new[] { modelsLight[0] };
        }


        public static Model TestSence()
        {
            float[,] A = new float[,]
                {
                    // Floor  -- white lambert
                    { 0.0f,    0.0f,    0.0f, 0.0f },
    { 0.0f,    0.0f,  559.2f, 0.0f },
    { 556.0f,    0.0f,  559.2f, 0.0f },
    { 0.0f,    0.0f,    0.0f, 0.0f },
    { 556.0f,    0.0f,  559.2f, 0.0f },
    { 556.0f,    0.0f,    0.0f, 0.0f },

    // Ceiling -- white lambert
    { 0.0f,  548.8f,    0.0f, 0.0f },
    { 556.0f,  548.8f,    0.0f, 0.0f },
    { 556.0f,  548.8f,  559.2f, 0.0f },

    { 0.0f,  548.8f,    0.0f, 0.0f },
    { 556.0f,  548.8f,  559.2f, 0.0f },
    { 0.0f,  548.8f,  559.2f, 0.0f },

    // Back wall -- white lambert
    { 0.0f,    0.0f,  559.2f, 0.0f },
    { 0.0f,  548.8f,  559.2f, 0.0f },
    { 556.0f,  548.8f,  559.2f, 0.0f },

    { 0.0f,    0.0f,  559.2f, 0.0f },
    { 556.0f,  548.8f,  559.2f, 0.0f },
    { 556.0f,    0.0f,  559.2f, 0.0f },

    // Right wall -- green lambert
    { 0.0f,    0.0f,    0.0f, 0.0f },
    { 0.0f,  548.8f,    0.0f, 0.0f },
    { 0.0f,  548.8f,  559.2f, 0.0f },

    { 0.0f,    0.0f,    0.0f, 0.0f },
    { 0.0f,  548.8f,  559.2f, 0.0f },
    { 0.0f,    0.0f,  559.2f, 0.0f },

    // Left wall -- red lambert
    { 556.0f,    0.0f,    0.0f, 0.0f },
    { 556.0f,    0.0f,  559.2f, 0.0f },
    { 556.0f,  548.8f,  559.2f, 0.0f },

    { 556.0f,    0.0f,    0.0f, 0.0f },
    { 556.0f,  548.8f,  559.2f, 0.0f },
    { 556.0f,  548.8f,    0.0f, 0.0f },

    // Short block -- white lambert
    { 130.0f,  165.0f,   65.0f, 0.0f },
    { 82.0f,  165.0f,  225.0f, 0.0f },
    { 242.0f,  165.0f,  274.0f, 0.0f },

    { 130.0f,  165.0f,   65.0f, 0.0f },
    { 242.0f,  165.0f,  274.0f, 0.0f },
    { 290.0f,  165.0f,  114.0f, 0.0f },

    { 290.0f,    0.0f,  114.0f, 0.0f },
    { 290.0f,  165.0f,  114.0f, 0.0f },
    { 240.0f,  165.0f,  272.0f, 0.0f },

    { 290.0f,    0.0f,  114.0f, 0.0f },
    { 240.0f,  165.0f,  272.0f, 0.0f },
    { 240.0f,    0.0f,  272.0f, 0.0f },

    { 130.0f,    0.0f,   65.0f, 0.0f },
    { 130.0f,  165.0f,   65.0f, 0.0f },
    { 290.0f,  165.0f,  114.0f, 0.0f },

    { 130.0f,    0.0f,   65.0f, 0.0f },
    { 290.0f,  165.0f,  114.0f, 0.0f },
    { 290.0f,    0.0f,  114.0f, 0.0f },

    { 82.0f,    0.0f,  225.0f, 0.0f },
    { 82.0f,  165.0f,  225.0f, 0.0f },
    { 130.0f,  165.0f,   65.0f, 0.0f },

    { 82.0f,    0.0f,  225.0f, 0.0f },
    { 130.0f,  165.0f,   65.0f, 0.0f },
    { 130.0f,    0.0f,   65.0f, 0.0f },

    { 240.0f,    0.0f,  272.0f, 0.0f },
    { 240.0f,  165.0f,  272.0f, 0.0f },
    { 82.0f,  165.0f,  225.0f, 0.0f },

    { 240.0f,    0.0f,  272.0f, 0.0f },
    { 82.0f,  165.0f,  225.0f, 0.0f },
    { 82.0f,    0.0f,  225.0f, 0.0f },

    // Tall block -- white lambert
    { 423.0f,  330.0f,  247.0f, 0.0f },
    { 265.0f,  330.0f,  296.0f, 0.0f },
    { 314.0f,  330.0f,  455.0f, 0.0f },

    { 423.0f,  330.0f,  247.0f, 0.0f },
    { 314.0f,  330.0f,  455.0f, 0.0f },
    { 472.0f,  330.0f,  406.0f, 0.0f },

    { 423.0f,    0.0f,  247.0f, 0.0f },
    { 423.0f,  330.0f,  247.0f, 0.0f },
    { 472.0f,  330.0f,  406.0f, 0.0f },

    { 423.0f,    0.0f,  247.0f, 0.0f },
    { 472.0f,  330.0f,  406.0f, 0.0f },
    { 472.0f,    0.0f,  406.0f, 0.0f },

    { 472.0f,    0.0f,  406.0f, 0.0f },
    { 472.0f,  330.0f,  406.0f, 0.0f },
    { 314.0f,  330.0f,  456.0f, 0.0f },

    { 472.0f,    0.0f,  406.0f, 0.0f },
    { 314.0f,  330.0f,  456.0f, 0.0f },
    { 314.0f,    0.0f,  456.0f, 0.0f },

    { 314.0f,    0.0f,  456.0f, 0.0f },
    { 314.0f,  330.0f,  456.0f, 0.0f },
    { 265.0f,  330.0f,  296.0f, 0.0f },

    { 314.0f,    0.0f,  456.0f, 0.0f },
    { 265.0f,  330.0f,  296.0f, 0.0f },
    { 265.0f,    0.0f,  296.0f, 0.0f },

    { 265.0f,    0.0f,  296.0f, 0.0f },
    { 265.0f,  330.0f,  296.0f, 0.0f },
    { 423.0f,  330.0f,  247.0f, 0.0f },

    { 265.0f,    0.0f,  296.0f, 0.0f },
    { 423.0f,  330.0f,  247.0f, 0.0f },
    { 423.0f,    0.0f,  247.0f, 0.0f },

    // Ceiling light -- emmissive
    { 343.0f,  548.6f,  227.0f, 0.0f },
    { 213.0f,  548.6f,  227.0f, 0.0f },
    { 213.0f,  548.6f,  332.0f, 0.0f },

    { 343.0f,  548.6f,  227.0f, 0.0f },
    { 213.0f,  548.6f,  332.0f, 0.0f },
    { 343.0f,  548.6f,  332.0f, 0.0f }

            };
            var models = new Model();
            for (int i = 0; i < A.GetLength(0); i += 3)
            {
                triangle t1 = new triangle();
                t1.P1.z = A[i, 1];
                t1.P1.x = A[i, 2];
                t1.P1.y = A[i, 0];
                t1.P2.z = A[i + 1, 1];
                t1.P2.x = A[i + 1, 2];
                t1.P2.y = A[i + 1, 0];
                t1.P3.z = A[i + 2, 1];
                t1.P3.x = A[i + 2, 2];
                t1.P3.y = A[i + 2, 0];
                models.tri_list.Add(t1);
            }
            Console.WriteLine(models.tri_list.Count);
            models.tri_list[models.tri_list.Count - 1].luminosity = 700;
            models.tri_list[models.tri_list.Count - 2].luminosity = 700;
            models.tri_list[6].Color = new vec3s(255, 55, 55);
            models.tri_list[7].Color = new vec3s(255, 55, 55);
            models.tri_list[8].Color = new vec3s(55, 55, 255);
            models.tri_list[9].Color = new vec3s(55, 55, 255);

            return models;
        }
        public static List<triangle> Create_AabbBox(Model model)
        {
            float Minx = model.tri_list.Min(s => s.TriangleMin().x);
            float Miny = model.tri_list.Min(s => s.TriangleMin().y);
            float Minz = model.tri_list.Min(s => s.TriangleMin().z);

            float Maxx = model.tri_list.Max(s => s.TriangleMax().x);
            float Maxy = model.tri_list.Max(s => s.TriangleMax().y);
            float Maxz = model.tri_list.Max(s => s.TriangleMax().z);

            vec3s u1 = new vec3s(Minx, Miny, Maxz);
            vec3s u2 = new vec3s(Maxx, Miny, Maxz);
            vec3s u3 = new vec3s(Maxx, Maxy, Maxz);
            vec3s u4 = new vec3s(Minx, Maxy, Maxz);

            vec3s d1 = new vec3s(Minx, Miny, Minz);
            vec3s d2 = new vec3s(Maxx, Miny, Minz);
            vec3s d3 = new vec3s(Maxx, Maxy, Minz);
            vec3s d4 = new vec3s(Minx, Maxy, Minz);

            List<triangle> DATA = new List<triangle>();
            DATA.Add(new triangle());
            DATA.Last().P1 = u1.Clone();
            DATA.Last().P2 = u2.Clone();
            DATA.Last().P3 = u3.Clone();

            DATA.Add(new triangle());
            DATA.Last().P1 = u1.Clone();
            DATA.Last().P2 = u4.Clone();
            DATA.Last().P3 = u3.Clone();

            DATA.Add(new triangle());
            DATA.Last().P1 = d1.Clone();
            DATA.Last().P2 = d2.Clone();
            DATA.Last().P3 = d3.Clone();

            DATA.Add(new triangle());
            DATA.Last().P1 = d1.Clone();
            DATA.Last().P2 = d4.Clone();
            DATA.Last().P3 = d3.Clone();

            DATA.Add(new triangle());
            DATA.Last().P1 = u1.Clone();
            DATA.Last().P2 = u2.Clone();
            DATA.Last().P3 = d1.Clone();

            DATA.Add(new triangle());
            DATA.Last().P1 = d1.Clone();
            DATA.Last().P2 = d2.Clone();
            DATA.Last().P3 = u2.Clone();

            DATA.Add(new triangle());
            DATA.Last().P1 = u3.Clone();
            DATA.Last().P2 = u4.Clone();
            DATA.Last().P3 = d4.Clone();

            DATA.Add(new triangle());
            DATA.Last().P1 = d3.Clone();
            DATA.Last().P2 = d4.Clone();
            DATA.Last().P3 = u3.Clone();

            DATA.Add(new triangle());
            DATA.Last().P1 = u1.Clone();
            DATA.Last().P2 = u4.Clone();
            DATA.Last().P3 = d1.Clone();

            DATA.Add(new triangle());
            DATA.Last().P1 = d1.Clone();
            DATA.Last().P2 = d4.Clone();
            DATA.Last().P3 = u4.Clone();

            DATA.Add(new triangle());
            DATA.Last().P1 = u2.Clone();
            DATA.Last().P2 = u3.Clone();
            DATA.Last().P3 = d2.Clone();

            DATA.Add(new triangle());
            DATA.Last().P1 = d2.Clone();
            DATA.Last().P2 = d3.Clone();
            DATA.Last().P3 = u3.Clone();


            return DATA;
        }
        public static Model 金字塔()
        {
            var models = new Model();
            triangle t1 = new triangle();
            t1.P1.z = 200;
            t1.P1.x = 0;
            t1.P1.y = 0;
            t1.P2.z = 0;
            t1.P2.x = 100;
            t1.P2.y = 100;
            t1.P3.z = 0;
            t1.P3.x = -100;
            t1.P3.y = 100;
            triangle t2 = new triangle();
            t2.P1.z = 200;
            t2.P1.x = 0;
            t2.P1.y = 0;
            t2.P2.z = 0;
            t2.P2.x = -100;
            t2.P2.y = 100;
            t2.P3.z = 0;
            t2.P3.x = -100;
            t2.P3.y = -100;
            triangle t3 = new triangle();
            t3.P1.z = 200;
            t3.P1.x = 0;
            t3.P1.y = 0;
            t3.P2.z = 0;
            t3.P2.x = 100;
            t3.P2.y = 100;
            t3.P3.z = 0;
            t3.P3.x = 100;
            t3.P3.y = -100;
            triangle t4 = new triangle();
            t4.P1.z = 200;
            t4.P1.x = 0;
            t4.P1.y = 0;
            t4.P2.z = 0;
            t4.P2.x = -100;
            t4.P2.y = -100;
            t4.P3.z = 0;
            t4.P3.x = 100;
            t4.P3.y = -100;

            models.tri_list.AddRange(new[] { t1, t2, t3, t4 });
            return models;
        }
        public static Model 地板()
        {
            Model model1 = new Model();
            triangle t1 = new triangle();
            t1.P1.z = 0;
            t1.P1.x = -3000;
            t1.P1.y = -3000;
            t1.P2.z = 0;
            t1.P2.x = -3000;
            t1.P2.y = 3000;
            t1.P3.z = 0;
            t1.P3.x = 3000;
            t1.P3.y = -3000;
            triangle t2 = new triangle();
            t2.P1.z = 0;
            t2.P1.x = 3000;
            t2.P1.y = -3000;
            t2.P2.z = 0;
            t2.P2.x = 3000;
            t2.P2.y = 3000;
            t2.P3.z = 0;
            t2.P3.x = -3000;
            t2.P3.y = 3000;
            model1.tri_list.AddRange(new[] { t1, t2 });
            model1.x = -1000;
            return model1;
        }
        static float D1 = 705.8f * 2f;
        static float D2 = 2457.6f;
        public static Model 灯源1()
        {
            Model model1 = new Model();
            triangle t1 = new triangle();
            t1.Color = new vec3s(25, 255, 25);
            t1.luminosity = D1;
            t1.P1.z = 300;
            t1.P1.x = 0;
            t1.P1.y = 600;
            t1.P2.z = 0;
            t1.P2.x = 0;
            t1.P2.y = 600;
            t1.P3.z = 0;
            t1.P3.x = 100;
            t1.P3.y = 600;
            triangle t2 = new triangle();
            t2.Color = new vec3s(25, 255, 25);
            t2.luminosity = D1;
            t2.P1.z = 300;
            t2.P1.x = 100;
            t2.P1.y = 600;
            t2.P2.z = 0;
            t2.P2.x = 100;
            t2.P2.y = 600;
            t2.P3.z = 300;
            t2.P3.x = 0;
            t2.P3.y = 600;
            model1.tri_list.AddRange(new[] { t1, t2 });
            return model1;
        }
        public static Model 灯源2()
        {
            Model model1 = new Model();
            triangle t1 = new triangle();
            t1.Color = new vec3s(25, 25, 255);
            t1.luminosity = D1;
            t1.P1.z = 300;
            t1.P1.x = 600;
            t1.P1.y = 100;
            t1.P2.z = 0;
            t1.P2.x = 600;
            t1.P2.y = 100;
            t1.P3.z = 0;
            t1.P3.x = 600;
            t1.P3.y = 0;
            triangle t2 = new triangle();
            t2.Color = new vec3s(25, 25, 255);
            t2.luminosity = D1;
            t2.P1.z = 300;
            t2.P1.x = 600;
            t2.P1.y = 0;
            t2.P2.z = 300;
            t2.P2.x = 600;
            t2.P2.y = 100;
            t2.P3.z = 0;
            t2.P3.x = 600;
            t2.P3.y = 0;
            model1.tri_list.AddRange(new[] { t1, t2 });
            return model1;
        }
        public static Model 灯源3()
        {
            Model model1 = new Model();
            triangle t1 = new triangle();
            t1.Color = new vec3s(255, 25, 25);
            t1.luminosity = D1;
            t1.P1.z = 300;
            t1.P1.x = -600;
            t1.P1.y = 100;
            t1.P2.z = 0;
            t1.P2.x = -600;
            t1.P2.y = 100;
            t1.P3.z = 0;
            t1.P3.x = -600;
            t1.P3.y = 0;
            triangle t2 = new triangle();
            t2.Color = new vec3s(255, 25, 25);
            t2.luminosity = D1;
            t2.P1.z = 300;
            t2.P1.x = -600;
            t2.P1.y = 0;
            t2.P2.z = 300;
            t2.P2.x = -600;
            t2.P2.y = 100;
            t2.P3.z = 0;
            t2.P3.x = -600;
            t2.P3.y = 0;
            model1.tri_list.AddRange(new[] { t1, t2 });
            return model1;
        }
        public static Model 灯源4()
        {
            Model model1 = new Model();
            {
                triangle t1 = new triangle();
                t1.Color = new vec3s(70, 70, 70);
                t1.luminosity = D2;
                t1.P1.z = 600;
                t1.P1.x = -300;
                t1.P1.y = -300;
                t1.P2.z = 600;
                t1.P2.x = 300;
                t1.P2.y = -300;
                t1.P3.z = 600;
                t1.P3.x = -300;
                t1.P3.y = 300;
                triangle t2 = new triangle();
                t2.Color = new vec3s(70, 70, 70);
                t2.luminosity = D2;
                t2.P1.z = 600;
                t2.P1.x = 300;
                t2.P1.y = 300;
                t2.P2.z = 600;
                t2.P2.x = 300;
                t2.P2.y = -300;
                t2.P3.z = 600;
                t2.P3.x = -300;
                t2.P3.y = 300;
                model1.tri_list.AddRange(new[] { t1, t2 });
            }
            return model1;
        }

        public static Model 正方形()
        {
            var models = new Model();
            triangle t1 = new triangle();
            t1.P1 = new vec3s(-100f, -100f, -100f);
            t1.P2 = new vec3s(100f, -100f, -100f);
            t1.P3 = new vec3s(100f, 100f, -100f);
            triangle t2 = new triangle();
            t2.P1 = new vec3s(100f, 100f, -100f);
            t2.P2 = new vec3s(-100f, 100f, -100f);
            t2.P3 = new vec3s(-100f, -100f, -100f);
            triangle t3 = new triangle();
            t3.P1 = new vec3s(-100f, -100f, 100f);
            t3.P2 = new vec3s(100f, -100f, 100f);
            t3.P3 = new vec3s(100f, 100f, 100f);
            triangle t4 = new triangle();
            t4.P1 = new vec3s(100f, 100f, 100f);
            t4.P2 = new vec3s(-100f, 100f, 100f);
            t4.P3 = new vec3s(-100f, -100f, 100f);
            triangle t5 = new triangle();
            t5.P1 = new vec3s(-100f, 100f, 100f);
            t5.P2 = new vec3s(-100f, 100f, -100f);
            t5.P3 = new vec3s(-100f, -100f, -100f);
            triangle t6 = new triangle();
            t6.P1 = new vec3s(-100f, -100f, -100f);
            t6.P2 = new vec3s(-100f, -100f, 100f);
            t6.P3 = new vec3s(-100f, 100f, 100f);
            triangle t7 = new triangle();
            t7.P1 = new vec3s(100f, 100f, 100f);
            t7.P2 = new vec3s(100f, 100f, -100f);
            t7.P3 = new vec3s(100f, -100f, -100f);
            triangle t8 = new triangle();
            t8.P1 = new vec3s(100f, -100f, -100f);
            t8.P2 = new vec3s(100f, -100f, 100f);
            t8.P3 = new vec3s(100f, 100f, 100f);
            triangle t9 = new triangle();
            t9.P1 = new vec3s(-100f, -100f, -100f);
            t9.P2 = new vec3s(100f, -100f, -100f);
            t9.P3 = new vec3s(100f, -100f, 100f);
            triangle t10 = new triangle();
            t10.P1 = new vec3s(100f, -100f, 100f);
            t10.P2 = new vec3s(-100f, -100f, 100f);
            t10.P3 = new vec3s(-100f, -100f, -100f);
            triangle t11 = new triangle();
            t11.P1 = new vec3s(-100f, 100f, -100f);
            t11.P2 = new vec3s(100f, 100f, -100f);
            t11.P3 = new vec3s(100f, 100f, 100f);
            triangle t12 = new triangle();
            t12.P1 = new vec3s(100f, 100f, 100f);
            t12.P2 = new vec3s(-100f, 100f, 100f);
            t12.P3 = new vec3s(-100f, 100f, -100f);



            models.tri_list.AddRange(new[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12 });

            return models;
        }
    }
}

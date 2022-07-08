using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Threading;
namespace OptixProject1
{
    internal class ModelOperation
    {
        public static void ModelIndexShow(ListView LV, List<Model> models)
        {
            LV.Items.Clear();

            for (int i = 0; i < models.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Tag = i;
                lvi.Text = "Model " + i + ":" + models[i].Name;

                lvi.SubItems.Add(models[i].Tex_w > 0 ? "True" : "False");
                lvi.SubItems.Add(models[i].ReflTex_w > 0 ? "True" : "False");
                lvi.SubItems.Add(models[i].RefrTex_w > 0 ? "True" : "False");
                LV.Items.Add(lvi);
            }


            LV.Update();

        }

        public static (vec3s Min, vec3s Max) ModelMinMax(Model model)
        {
            float Xmin = model.tri_list.Min(s => Math.Min(Math.Min(s.P1.x, s.P2.x), s.P3.x));
            float Ymin = model.tri_list.Min(s => Math.Min(Math.Min(s.P1.y, s.P2.y), s.P3.y));
            float Zmin = model.tri_list.Min(s => Math.Min(Math.Min(s.P1.z, s.P2.z), s.P3.z));


            float Xmax = model.tri_list.Max(s => Math.Max(Math.Max(s.P1.x, s.P2.x), s.P3.x));
            float Ymax = model.tri_list.Max(s => Math.Max(Math.Max(s.P1.y, s.P2.y), s.P3.y));
            float Zmax = model.tri_list.Max(s => Math.Max(Math.Max(s.P1.z, s.P2.z), s.P3.z));

            return (new vec3s(Xmin, Ymin, Zmin), new vec3s(Xmax, Ymax, Zmax));
        }
        public static vec3s ModelAvg(Model model)
        {
            float Xmin = model.tri_list.Average(s => (s.P1.x + s.P2.x + s.P3.x) / 3f);
            float Ymin = model.tri_list.Average(s => (s.P1.y + s.P2.y + s.P3.y) / 3f);
            float Zmin = model.tri_list.Average(s => (s.P1.z + s.P2.z + s.P3.z) / 3f);

            return new vec3s(Xmin, Ymin, Zmin);
        }
        private static DataEx.ModelTransformParam[] MTFP = new DataEx.ModelTransformParam[1];
        public static DataEx.ModelTransformParam[] GetMTFP(List<Model> models)
        {
            if (MTFP.Length != models.Count) MTFP = new DataEx.ModelTransformParam[models.Count];

            for (int i = 0; i < models.Count; i++)
            {
                if (i >= MTFP.Length) break;
                MTFP[i].TextureID = models[i].TextureID == -1 ? -1 : i;
                MTFP[i].ReflTextureID = models[i].ReflTextureID == -1 ? -1 : i;
                MTFP[i].RefrTextureID = models[i].RefrTextureID == -1 ? -1 : i;

                MTFP[i].Tex_Wid = models[i].Tex_w;
                MTFP[i].Tex_Hei = models[i].Tex_h;

                MTFP[i].ReflTex_Wid = models[i].ReflTex_w;
                MTFP[i].ReflTex_Hei = models[i].ReflTex_h;

                MTFP[i].RefrTex_Wid = models[i].RefrTex_w;
                MTFP[i].RefrTex_Hei = models[i].RefrTex_h;

                MTFP[i].TextureScale = models[i].TextureScale;
                MTFP[i].luminosityMul = models[i].luminosityMul;

                MTFP[i].CenterX = models[i].Point.x;
                MTFP[i].CenterY = models[i].Point.y;
                MTFP[i].CenterZ = models[i].Point.z;

                MTFP[i].SkyBox = models[i].SkyBox;


                float[] m = Math2.GetM12_Fast(
                    models[i].Rotation.x,
                    models[i].Rotation.y,
                    models[i].Rotation.z,
                    models[i].Point.x,
                    models[i].Point.y,
                    models[i].Point.z,
                    models[i].Scale.x,
                    models[i].Scale.y,
                    models[i].Scale.z
                       );

                MTFP[i].m0 = m[0];
                MTFP[i].m1 = m[1];
                MTFP[i].m2 = m[2];
                MTFP[i].m3 = m[3];
                MTFP[i].m4 = m[4];
                MTFP[i].m5 = m[5];
                MTFP[i].m6 = m[6];
                MTFP[i].m7 = m[7];
                MTFP[i].m8 = m[8];
                MTFP[i].m9 = m[9];
                MTFP[i].m10 = m[10];
                MTFP[i].m11 = m[11];



                MTFP[i].ModelID = i;
            }


            return MTFP;
        }



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumSharp;
using OpenCvSharp;
namespace OptixProject1
{
    static class RandomClass
    {
        public static Random ran = new Random();
    }
    [Serializable]
    public class Model : triangle
    {
        public string Name;
        public vec3s Point = new vec3s();
        public vec3s Rotation = new vec3s(0, 0, 0, 0);
        public vec3s Scale = new vec3s(1, 1, 1);
        public float luminosityMul = 1;
        public List<triangle> tri_list = new List<triangle>();
        public int Tex_w, Tex_h;
        public int ReflTex_w, ReflTex_h;
        public int RefrTex_w, RefrTex_h;
        public int TextureID = -1;
        public int ReflTextureID = -1;
        public int RefrTextureID = -1;
        public IntPtr TextureData = new IntPtr();
        public IntPtr ReflTextureData = new IntPtr();
        public IntPtr RefrTextureData = new IntPtr();
        public float TextureScale = 1;
        public int TriCount = 0;
        public int SkyBox = 0;
        public float DiffZ = 0;
        public int CloneID = -1;


        public Model ModelClone()
        {
            Model newmodel = new Model();
            newmodel.DiffZ = DiffZ;
            newmodel.tri_list = tri_list;
            newmodel.Name = Name;
            newmodel.Point = Point.Clone();
            newmodel.Rotation = Rotation.Clone();
            newmodel.Scale = Scale.Clone();
            newmodel.luminosityMul = luminosityMul;
            newmodel.Tex_w = Tex_w;
            newmodel.Tex_h = Tex_h;
            newmodel.ReflTex_w = ReflTex_w;
            newmodel.ReflTex_h = ReflTex_h;
            newmodel.TextureID = TextureID;
            newmodel.ReflTextureID = ReflTextureID;
            newmodel.RefrTextureID = RefrTextureID;
            newmodel.TextureScale = TextureScale;
            newmodel.TriCount = TriCount;
            return newmodel;

        }
    }
}

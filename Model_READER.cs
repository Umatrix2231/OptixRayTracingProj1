using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using OpenCvSharp;
using OpenCvSharp.Extensions;
namespace OptixProject1
{
    internal class Model_READER
    {
        class ObjReadClass : vec3s
        {
            public vec3s Color = new vec3s();
            public int P1i, P2i, P3i;
            public int P1vt, P2vt, P3vt;
        }
        public static Model Read_STL(string filename)
        {
            FileStream bw = new FileStream(filename, FileMode.Open);
            bw.Seek(80, SeekOrigin.Begin);

            byte[] data = new byte[4];
            bw.Read(data, 0, 4);
            byte[] Cdata = new byte[2];

            int TriCount = BitConverter.ToInt32(data, 0);
            Console.WriteLine(TriCount);



            List<triangle> TriData = new List<triangle>();

            for (int i = 0; i < TriCount; i++)
            {
                triangle Temp = new triangle();
                vec3s[] v3 = new vec3s[3];
                bw.Read(data, 0, 4);
                // float NX = BitConverter.ToSingle(data, 0);
                bw.Read(data, 0, 4);
                // float NY = BitConverter.ToSingle(data, 0);
                bw.Read(data, 0, 4);
                // float NZ = BitConverter.ToSingle(data, 0);

                // var N = new vec3s(NX, NY, NZ);


                for (int j = 0; j < 3; j++)
                {
                    bw.Read(data, 0, 4);
                    float X = BitConverter.ToSingle(data, 0);
                    bw.Read(data, 0, 4);
                    float Y = BitConverter.ToSingle(data, 0);
                    bw.Read(data, 0, 4);
                    float Z = BitConverter.ToSingle(data, 0);
                    v3[j] = new vec3s(X, Y, Z);
                }
                bw.Read(Cdata, 0, 2);


                Temp.P1 = v3[0];
                Temp.P2 = v3[1];
                Temp.P3 = v3[2];

                TriData.Add(Temp);
            }

            Model model = new Model();
            model.tri_list = TriData;

            bw.Close();
            return model;

        }


        unsafe public static Model[] Read_OBJVT(string filename, string TexFile, string Refl = "", string Refr = "", bool ComputeSrcLight = false, bool SkyBox = false)
        {

            if (File.Exists(filename)) Console.WriteLine("Find File");

            Console.WriteLine("ReadAllLine Start");
            string[] AllLines = File.ReadAllLines(filename);
            Console.WriteLine("ReadAllLine End:" + AllLines.Length);



            List<ObjReadClass> INDEX_VERTEX_VT = new List<ObjReadClass>();
            List<ObjReadClass> PointAndColor = new List<ObjReadClass>();
            List<ObjReadClass> PointVT = new List<ObjReadClass>();
            for (int i = 0; i < AllLines.Length; i++)
            {
                string DataString = AllLines[i];
                if (DataString.Length < 2) continue;

                if (DataString[0] == 'v' && DataString[1] == ' ')
                {
                    string[] S = DataString.Replace("v ", "").Split(' ');

                    ObjReadClass Temp = new ObjReadClass();
                    if (S[0].Length > 0)
                    {
                        Temp.x = float.Parse(S[0]);
                        Temp.y = float.Parse(S[1]);
                        Temp.z = float.Parse(S[2]);
                    }
                    else
                    {
                        Temp.x = float.Parse(S[1]);
                        Temp.y = float.Parse(S[2]);
                        Temp.z = float.Parse(S[3]);
                    }

                    PointAndColor.Add(Temp);
                }
                if (DataString[0] == 'v' && DataString[1] == 't' && DataString[2] == ' ')
                {
                    string[] S = DataString.Replace("vt ", "").Split(' ');
                    ObjReadClass Temp = new ObjReadClass();

                    Temp.x = float.Parse(S[0]);
                    Temp.y = float.Parse(S[1]);

                    PointVT.Add(Temp);
                }
                if (DataString[0] == 'f' && DataString[1] == ' ')
                {
                    string[] S = DataString.Split(' ');
                    if (S[1].IndexOf('/') >= 0)
                    {
                        ObjReadClass Temp = new ObjReadClass();
                        Temp.P1i = int.Parse(S[1].Split('/')[0]);
                        Temp.P2i = int.Parse(S[2].Split('/')[0]);
                        Temp.P3i = int.Parse(S[3].Split('/')[0]);
                        Temp.P1vt = int.Parse(S[1].Split('/')[1]);
                        Temp.P2vt = int.Parse(S[2].Split('/')[1]);
                        Temp.P3vt = int.Parse(S[3].Split('/')[1]);
                        INDEX_VERTEX_VT.Add(Temp);
                    }
                    else
                    {
                        ObjReadClass Temp = new ObjReadClass();
                        Temp.P1i = int.Parse(S[1]);
                        Temp.P2i = int.Parse(S[2]);
                        Temp.P3i = int.Parse(S[3]);
                        Temp.P1vt = 1;
                        Temp.P2vt = 1;
                        Temp.P3vt = 1;
                        INDEX_VERTEX_VT.Add(Temp);
                    }
                }

            }

            Console.WriteLine("Read Finidshid:" + INDEX_VERTEX_VT.Count);



            List<Model> model = new List<Model>();
            List<triangle> TriData = new List<triangle>();
            for (int i = 0; i < INDEX_VERTEX_VT.Count; i++)
            {
                triangle Temp = new triangle();
                Temp.P1 = PointAndColor[INDEX_VERTEX_VT[i].P1i - 1];
                Temp.P2 = PointAndColor[INDEX_VERTEX_VT[i].P2i - 1];
                Temp.P3 = PointAndColor[INDEX_VERTEX_VT[i].P3i - 1];
                if (INDEX_VERTEX_VT[i].P1vt - 1 < 0 || INDEX_VERTEX_VT[i].P2vt - 1 < 0 || INDEX_VERTEX_VT[i].P3vt - 1 < 0)
                {
                    Temp.VT1 = new vec3s();
                    Temp.VT2 = new vec3s();
                    Temp.VT3 = new vec3s();
                }
                else
                {
                    Temp.VT1 = PointVT[INDEX_VERTEX_VT[i].P1vt - 1];
                    Temp.VT2 = PointVT[INDEX_VERTEX_VT[i].P2vt - 1];
                    Temp.VT3 = PointVT[INDEX_VERTEX_VT[i].P3vt - 1];
                }
                Temp.Color = new vec3s(255, 255, 255);


                TriData.Add(Temp);
            }
            model.Add(new Model());
            model.Last().TriCount = TriData.Count();
            model.Last().tri_list = TriData;
            model.Last().Name = filename;
            model.Last().SkyBox = SkyBox ? 9999 : -9999;

            //////////////TextAddIn


            Mat Texture = Cv2.ImRead(TexFile, ImreadModes.LoadGdal | ImreadModes.AnyDepth);
            if (Texture.Channels() == 3)
            {
                Cv2.CvtColor(Texture, Texture, ColorConversionCodes.BGR2BGRA);
                Cv2.Multiply(Texture, new Scalar(1, 1, 1, 0), Texture);
            }
            Texture.ConvertTo(Texture, MatType.CV_32FC4);

            if (ComputeSrcLight == true)
            {
                Cv2.Threshold(Texture, Texture, 100, 100, ThresholdTypes.Trunc);
                Cv2.Add(Texture, new Scalar(0, 0, 0, 255), Texture);
            }



            Vec4f[] TextureData;
            Texture.GetArray<Vec4f>(out TextureData);
            model.Last().TextureID = 0;
            model.Last().TextureData = DataEx.StructureToPtr<Vec4f>(TextureData);
            model.Last().Tex_w = Texture.Width;
            model.Last().Tex_h = Texture.Height;


            Mat ReflTexture = null;
            Vec3b[] ReflTextureData = null;
            if (Refl != "")
            {
                ReflTexture = new Mat(Refl);
                ReflTexture.GetArray<Vec3b>(out ReflTextureData);
            }
            model.Last().ReflTextureID = Refl != "" ? 0 : -1;
            model.Last().ReflTextureData = Refl != "" ? DataEx.StructureToPtr<Vec3b>(ReflTextureData) : new IntPtr();
            model.Last().ReflTex_w = Refl != "" ? ReflTexture.Width : 0;
            model.Last().ReflTex_h = Refl != "" ? ReflTexture.Height : 0;


            Mat RerlTexture = null;
            Vec3b[] RefrTextureData = null;
            if (Refr != "")
            {
                RerlTexture = new Mat(Refr);
                RerlTexture.GetArray<Vec3b>(out RefrTextureData);
            }
            model.Last().RefrTextureID = Refr != "" ? 0 : -1;
            model.Last().RefrTextureData = Refr != "" ? DataEx.StructureToPtr<Vec3b>(RefrTextureData) : new IntPtr();
            model.Last().RefrTex_w = Refr != "" ? RerlTexture.Width : 0;
            model.Last().RefrTex_h = Refr != "" ? RerlTexture.Height : 0;



            return model.ToArray();

        }
        public static Model[] Read_OBJ(string filename, bool Combind)
        {
            if (File.Exists(filename)) Console.WriteLine("Find File");

            Console.WriteLine("ReadAllLine Start");
            string[] AllLines = File.ReadAllLines(filename);
            Console.WriteLine("ReadAllLine End:" + AllLines.Length);


            List<List<ObjReadClass>> IP = new List<List<ObjReadClass>>();
            List<ObjReadClass> PointAndColor = new List<ObjReadClass>();
            for (int i = 0; i < AllLines.Length; i++)
            {
                string DataString = AllLines[i];
                if (DataString.Length < 2) continue;

                if (DataString[0] == 'o' && DataString[1] == ' ')
                {
                    if (Combind && IP.Count == 1)
                    {

                    }
                    else
                    {
                        List<ObjReadClass> INDEX1 = new List<ObjReadClass>();
                        IP.Add(INDEX1);
                    }
                }
                if (DataString[0] == 'v' && DataString[1] == ' ')
                {
                    string[] S = DataString.Split(' ');
                    ObjReadClass Temp = new ObjReadClass();
                    if (S[1].Length > 1)
                    {
                        Temp.x = float.Parse(S[1]);
                        Temp.y = float.Parse(S[2]);
                        Temp.z = float.Parse(S[3]);

                        if (S.Length > 6)
                        {
                            Temp.Color.x = float.Parse(S[6]);
                            Temp.Color.y = float.Parse(S[5]);
                            Temp.Color.z = float.Parse(S[4]);
                        }
                    }
                    else
                    {
                        Temp.x = float.Parse(S[2]);
                        Temp.y = float.Parse(S[3]);
                        Temp.z = float.Parse(S[4]);
                        if (S.Length > 6)
                        {
                            Temp.Color.x = float.Parse(S[7]);
                            Temp.Color.y = float.Parse(S[6]);
                            Temp.Color.z = float.Parse(S[5]);
                        }
                    }
                    PointAndColor.Add(Temp);
                }


                if (DataString[0] == 'f' && DataString[1] == ' ')
                {
                    string[] S = DataString.Split(' ');
                    if (S[1].IndexOf('/') < 0)
                    {
                        ObjReadClass Temp = new ObjReadClass();
                        Temp.P1i = int.Parse(S[1]);
                        Temp.P2i = int.Parse(S[2]);
                        Temp.P3i = int.Parse(S[3]);
                        IP.Last().Add(Temp);
                    }
                    else
                    {
                        ObjReadClass Temp = new ObjReadClass();
                        Temp.P1i = int.Parse(S[1].Split('/')[0]);
                        Temp.P2i = int.Parse(S[2].Split('/')[0]);
                        Temp.P3i = int.Parse(S[3].Split('/')[0]);
                        IP.Last().Add(Temp);
                    }
                }

            }

            Console.WriteLine(IP.Sum(s => s.Count));



            List<Model> model = new List<Model>();

            for (int j = 0; j < IP.Count; j++)
            {
                List<triangle> TriData = new List<triangle>();
                for (int i = 0; i < IP[j].Count; i++)
                {
                    triangle Temp = new triangle();
                    Temp.P1 = PointAndColor[IP[j][i].P1i - 1];
                    Temp.P2 = PointAndColor[IP[j][i].P2i - 1];
                    Temp.P3 = PointAndColor[IP[j][i].P3i - 1];
                    Temp.Color = PointAndColor[IP[j][i].P1i - 1].Color;

                    if (Temp.Color.z == 0 && Temp.Color.y == 0)
                    {
                        Temp.luminosity = Temp.Color.x * 150f * 18f;
                        // Temp.Color = new vec3s(93,172,252);
                        Temp.Color = new vec3s(153, 172, 222);
                        //Temp.Color = new vec3s(ran.Next(100,255), ran.Next(100, 255), ran.Next(100, 255));
                    }
                    if (Temp.Color.x == 0 && Temp.Color.y == 0)
                    {
                        Temp.Refl = Temp.Color.z / 10f;
                        Temp.Color = new vec3s(240, 240, 240);
                    }
                    if (Temp.Color.x == 0 && Temp.Color.z == 0)
                    {
                        Temp.Refr = 1.5f;
                        Temp.Trans = Temp.Color.y / 10f;
                        Temp.Color = new vec3s(240, 240, 240);
                    }

                    TriData.Add(Temp);
                };
                model.Add(new Model());
                model.Last().TriCount = TriData.Count();
                model.Last().tri_list = TriData;
                model.Last().Name = filename;
            }

            return model.ToArray();

        }
        public static Model[] Read_MTL(string filename, string mtlfile, bool Combind)
        {
            if (File.Exists(filename)) Console.WriteLine("Find File");
            FileStream stream1 = new FileStream(filename, FileMode.Open);
            FileStream stream2 = new FileStream(mtlfile, FileMode.Open);
            StreamReader reader = new StreamReader(stream1);
            StreamReader mtlreader = new StreamReader(stream2);

            List<(string MTLNAME, vec3s Color)> ColorData = new List<(string MTLNAME, vec3s Color)>();

            string MTLNAMETEMP = "";
            vec3s COLORTEMP = new vec3s();
            while (!mtlreader.EndOfStream)
            {
                string temps = mtlreader.ReadLine();
                if (temps.IndexOf("newmtl") >= 0) MTLNAMETEMP = temps.Split(' ')[1];
                if (temps.IndexOf("Kd") >= 0)
                {
                    COLORTEMP.x = float.Parse(temps.Split(' ')[3]) * 255f;
                    COLORTEMP.y = float.Parse(temps.Split(' ')[2]) * 255f;
                    COLORTEMP.z = float.Parse(temps.Split(' ')[1]) * 255f;
                    ColorData.Add((MTLNAMETEMP, COLORTEMP.Clone()));
                    Console.WriteLine(MTLNAMETEMP);
                }

            }


            List<List<ObjReadClass>> IP = new List<List<ObjReadClass>>();
            List<ObjReadClass> PointAndColor = new List<ObjReadClass>();

            while (!reader.EndOfStream)
            {
                string DataString = reader.ReadLine();

                if (DataString.IndexOf("o O") >= 0)
                {
                    if (Combind && IP.Count == 1)
                    {

                    }
                    else
                    {
                        List<ObjReadClass> INDEX1 = new List<ObjReadClass>();
                        IP.Add(INDEX1);
                    }
                }
                if (DataString.IndexOf("v ") >= 0)
                {
                    string[] S = DataString.Replace("v ", " ").Split(' ');
                    ObjReadClass Temp = new ObjReadClass();
                    if (S[0].Length > 0)
                    {
                        Temp.x = float.Parse(S[0]);
                        Temp.y = float.Parse(S[1]);
                        Temp.z = float.Parse(S[2]);
                        Temp.Color.x = COLORTEMP.x;
                        Temp.Color.y = COLORTEMP.y;
                        Temp.Color.z = COLORTEMP.z;
                        if (S.Length > 5)
                        {
                            Temp.Color.x = float.Parse(S[5]);
                            Temp.Color.y = float.Parse(S[4]);
                            Temp.Color.z = float.Parse(S[3]);
                        }
                    }
                    else
                    {
                        Temp.x = float.Parse(S[1]);
                        Temp.y = float.Parse(S[2]);
                        Temp.z = float.Parse(S[3]);
                        Temp.Color.x = COLORTEMP.x;
                        Temp.Color.y = COLORTEMP.y;
                        Temp.Color.z = COLORTEMP.z;
                        if (S.Length > 5)
                        {
                            Temp.Color.x = float.Parse(S[6]);
                            Temp.Color.y = float.Parse(S[5]);
                            Temp.Color.z = float.Parse(S[4]);
                        }
                    }
                    PointAndColor.Add(Temp);
                }

                if (DataString.IndexOf("usemtl") >= 0)
                {
                    COLORTEMP = ColorData.Where(s => s.MTLNAME == DataString.Split(' ')[1]).First().Color.Clone();
                }
                else if (DataString.Length == 0) COLORTEMP = new vec3s(-1, -1, -1);

                if (DataString.IndexOf("f ") >= 0 && DataString.IndexOf("/") >= 0)
                {
                    string[] S = DataString.Split(' ');
                    ObjReadClass Temp = new ObjReadClass();
                    Temp.P1i = int.Parse(S[1].Split('/')[0]);
                    Temp.P2i = int.Parse(S[2].Split('/')[0]);
                    Temp.P3i = int.Parse(S[3].Split('/')[0]);
                    if (COLORTEMP != new vec3s(-1, -1, -1))
                    {
                        Temp.Color = COLORTEMP.Clone();
                    }
                    else Temp.Color = new vec3s(-1, -1, -1);
                    IP.Last().Add(Temp);
                }
                else if (DataString.IndexOf("f ") >= 0)
                {
                    string[] S = DataString.Split(' ');
                    ObjReadClass Temp = new ObjReadClass();
                    Temp.P1i = int.Parse(S[1]);
                    Temp.P2i = int.Parse(S[2]);
                    Temp.P3i = int.Parse(S[3]);
                    if (COLORTEMP != new vec3s(-1, -1, -1))
                    {
                        Temp.Color = COLORTEMP.Clone();
                    }
                    else Temp.Color = new vec3s(-1, -1, -1);
                    IP.Last().Add(Temp);
                }

            }

            Console.WriteLine(IP.Sum(s => s.Count));


            List<Model> model = new List<Model>();

            for (int j = 0; j < IP.Count; j++)
            {
                List<triangle> TriData = new List<triangle>();
                for (int i = 0; i < IP[j].Count; i++)
                {
                    triangle Temp = new triangle();
                    Temp.P1 = PointAndColor[IP[j][i].P1i - 1];
                    Temp.P2 = PointAndColor[IP[j][i].P2i - 1];
                    Temp.P3 = PointAndColor[IP[j][i].P3i - 1];
                    if (IP[j][i].Color != new vec3s(-1, -1, -1)) Temp.Color = IP[j][i].Color.Clone();
                    else Temp.Color = PointAndColor[IP[j][i].P1i - 1].Color;

                    if (Temp.Color.z == 0 && Temp.Color.y == 0)
                    {
                        Temp.luminosity = Temp.Color.x * 150f * 18f;
                        // Temp.Color = new vec3s(93,172,252);
                        Temp.Color = new vec3s(153, 172, 222);
                        //Temp.Color = new vec3s(ran.Next(100,255), ran.Next(100, 255), ran.Next(100, 255));
                    }
                    if (Temp.Color.x == 0 && Temp.Color.y == 0)
                    {
                        Temp.Refl = Temp.Color.z / 10f;
                        Temp.Color = new vec3s(240, 240, 240);
                    }
                    if (Temp.Color.x == 0 && Temp.Color.z == 0)
                    {
                        Temp.Refr = 1.5f;
                        Temp.Trans = Temp.Color.y / 10f;
                        Temp.Color = new vec3s(240, 240, 240);
                    }

                    TriData.Add(Temp);
                }
                model.Add(new Model());
                model.Last().TriCount = TriData.Count();
                model.Last().tri_list = TriData;
                model.Last().Name = filename;
            }



            stream1.Close();
            stream2.Close();
            return model.ToArray();

        }
    }
}

using System;
using System.Runtime.InteropServices;
using System.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using OpenCvSharp;
namespace OptixProject1
{
    public class DataEx
    {
        public struct ParticleSystemsStruct
        {
            public float x, y, z;
            public float dx, dy, dz;
        }
        public struct DevieParams
        {
            public int DeletInstance;
            public int CloneInstance;
            public int Denose;
            public int ChangeNow;
            public int TraceMotion;
            public int SeletScreenX;
            public int SeletScreenY;
            public int InstanceIndex;
            public float InterPointX;
            public float InterPointY;
            public float InterPointZ;
            public int FramePerTime;
            public int MaxReflectTimes;
            public byte ComputeTransform;
        }
        [Serializable]
        public struct SkyLight
        {
            public float PaintingFlag;
            public float PaintingDist;
            public float PaintingScreenX;
            public float PaintingScreenY;
            public float PaintingToColor1;
            public float PaintingToColor2;
            public float PaintingToColor3;
            public float PaintingToColor4;
            public float ReflectValue;
            public float RefractionValue;
            public float TransmittanceValue;
            public float MissColor1;
            public float MissColor2;
            public float MissColor3;
            public float Missluminosity;
            public float LightNx;
            public float LightNy;
            public float LightNz;
            public float VolLightWeight;
            public float FogWeight, FogSinTime, FogSinX, FogSinY, FogZup, FogZDown, FogWaveSize, FogHeightBlurWeight;

        };
        public struct Camera_Param_Cuda
        {
            public float x;
            public float y;
            public float z;
            public float lookx;
            public float looky;
            public float lookz;
            public float upx;
            public float upy;
            public float upz;
            public float FovW;
            public float FovH;
        };
        public struct Byte4_Cuda
        {
            public byte x;
            public byte y;
            public byte z;
            public byte w;
        };

        public struct Vec3s_Cuda
        {
            public float x;
            public float y;
            public float z;
        };

        public struct Vec4s_Cuda
        {
            public float x;
            public float y;
            public float z;
            public float w;
        };

        public struct ModelTransformParam
        {
            public float luminosityMul;
            public int ModelID;
            public int TextureID;
            public int ReflTextureID;
            public int RefrTextureID;
            public int Tex_Wid, Tex_Hei;
            public int ReflTex_Wid, ReflTex_Hei;
            public int RefrTex_Wid, RefrTex_Hei;
            public float TextureScale;
            public float m0, m1, m2, m3, m4, m5, m6, m7, m8, m9, m10, m11;
            public float CenterX, CenterY, CenterZ;
            public float SkyBox;
        };

        public struct Triangle_Cuda
        {
            public byte DiffsePassToReturn;
            public byte DiffsePassAngle;
            public byte DiffsePassChance;
            public byte FogPassChance;
            public byte MASK;
            public float u1;
            public float v1;
            public float u2;
            public float v2;
            public float u3;
            public float v3;
            public float Nx;
            public float Ny;
            public float Nz;
            public float Color1;
            public float Color2;
            public float Color3;
            public float luminosity;
            public byte ReflectValue;
            public byte RefractionValue;
            public byte TransmittanceValue;
            public int ModelID;
        };
        public struct CameraParam_Cuda
        {

            public float x;
            public float y;
            public float z;
            public float u;
            public float v;
            public float fovW;
            public float fovH;
            public int Wid;
            public int Hei;
            public float StartX;
            public float StartY;
            public float StartZ;
            public float WVX;
            public float WVY;
            public float WVZ;
            public float HVX;
            public float HVY;
            public float HVZ;
        };
        public static byte[] PtrToByte<T>(IntPtr Ptr, int Len)
        {

            byte[] bytes = new byte[Marshal.SizeOf(typeof(T)) * Len];
            Marshal.Copy(Ptr, bytes, 0, Marshal.SizeOf(typeof(T)) * Len);

            Marshal.FreeCoTaskMem(Ptr);

            return bytes;
        }
        public static IntPtr ArrayToPtr(float[] V)
        {
            IntPtr Vptr = AllocHGlobal<float>(V);
            Marshal.Copy(V, 0, Vptr, V.Length);

            return Vptr;
        }

        public static T[] PtrToStructure<T>(IntPtr Ptr, int Len)
        {

            T[] ret = new T[Len];

            for (int i = 0; i < Len; i++)
            {
                byte[] bytes = new byte[Marshal.SizeOf(typeof(T))];
                Marshal.Copy(Ptr + Marshal.SizeOf(typeof(T)) * i, bytes, 0, Marshal.SizeOf(typeof(T)));


                int size = Marshal.SizeOf(typeof(T));
                IntPtr buffer = Marshal.AllocHGlobal(size);

                Marshal.Copy(bytes, 0, buffer, size);
                ret[i] = (T)Marshal.PtrToStructure(buffer, typeof(T));

                Marshal.FreeHGlobal(buffer);
            }
            Marshal.FreeCoTaskMem(Ptr);

            return ret;
        }
        public static T[] PtrToStructure_NoFree<T>(IntPtr Ptr, int Len)
        {

            T[] ret = new T[Len];

            for (int i = 0; i < Len; i++)
            {
                byte[] bytes = new byte[Marshal.SizeOf(typeof(T))];
                Marshal.Copy(Ptr + Marshal.SizeOf(typeof(T)) * i, bytes, 0, Marshal.SizeOf(typeof(T)));


                int size = Marshal.SizeOf(typeof(T));
                IntPtr buffer = Marshal.AllocHGlobal(size);

                Marshal.Copy(bytes, 0, buffer, size);
                ret[i] = (T)Marshal.PtrToStructure(buffer, typeof(T));

                Marshal.FreeHGlobal(buffer);
            }

            return ret;
        }
        public static T PtrToStructure<T>(IntPtr Ptr)
        {

            byte[] bytes = new byte[Marshal.SizeOf(typeof(T))];
            Marshal.Copy(Ptr, bytes, 0, Marshal.SizeOf(typeof(T)));

            int size = Marshal.SizeOf(typeof(T));
            IntPtr buffer = Marshal.AllocHGlobal(size);

            Marshal.Copy(bytes, 0, buffer, size);
            T ret = (T)Marshal.PtrToStructure(buffer, typeof(T));

            Marshal.FreeHGlobal(buffer);

            Marshal.FreeCoTaskMem(Ptr);

            return ret;
        }
        public static IntPtr StructureToPtr<T>(T[] Stru)
        {
            IntPtr A = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)) * Stru.Length);

            for (int i = 0; i < Stru.Length; i++)
            {
                Marshal.StructureToPtr(Stru[i], A + i * Marshal.SizeOf(typeof(T)), false);
            }
            return A;
        }
        public static IntPtr StructureToPtr_ReCopy<T>(IntPtr A, T[] Stru)
        {
            for (int i = 0; i < Stru.Length; i++)
            {
                Marshal.StructureToPtr(Stru[i], A + i * Marshal.SizeOf(typeof(T)), false);
            }
            return A;
        }
        public static byte[] StructToBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            byte[] bytes = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structObj, structPtr, false);
            Marshal.Copy(structPtr, bytes, 0, size);
            Marshal.FreeHGlobal(structPtr);
            return bytes;
        }
        public static IntPtr StructureToByteToPtr<T>(T[] Stru)
        {
            List<byte> DATA = new List<byte>();
            for (int i = 0; i < Stru.Length; i++) DATA.AddRange(StructToBytes(Stru[i]));

            IntPtr A = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * DATA.Count);
            Marshal.Copy(A, DATA.ToArray(), 0, Marshal.SizeOf(typeof(byte)) * DATA.Count);
            Console.WriteLine("byte:" + Marshal.SizeOf(typeof(byte)) * DATA.Count);
            DATA.Clear();

            return A;
        }
        public static IntPtr StructureToPtr<T>(T Stru)
        {
            IntPtr A = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));

            Marshal.StructureToPtr(Stru, A, false);

            return A;
        }
        public static IntPtr AllocHGlobal<T>(T[] Stru)
        {
            return Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)) * Stru.Length);

        }
        public static IntPtr AllocHGlobal<T>(int Len)
        {
            return Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)) * Len);

        }
        public static IntPtr AllocHGlobal<T>()
        {
            return Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));

        }

    }
}

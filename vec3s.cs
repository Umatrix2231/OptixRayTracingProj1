using NumSharp;
using NumSharp.Backends;
using NumSharp.Backends.Unmanaged;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq; 
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
namespace OptixProject1
{
    [Serializable]
    unsafe public class vec3s
    {
        public readonly float 空气折射率 = 1.00029f;
        public float x = 0, y = 0, z = 0, w = 0;
        public float 介质折射率 = 1.00029f;
        public vec3s()
        {

        }

        public vec3s(double _x, double _y, double _z, double _w)
        {
            x = (float)_x;
            y = (float)_y;
            z = (float)_z;
            w = (float)_w;
        }
        public vec3s(float* _v)
        {
            x = _v[0];
            y = _v[1];
            z = _v[2];
        }
        public vec3s(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
        public vec3s(double _x, double _y, double _z)
        {
            x = (float)_x;
            y = (float)_y;
            z = (float)_z;
        }
        public float Max()
        {
            return Math.Max(Math.Max(x, y), z);
        }
        public float[] ToFloat()
        {
            return new float[] { x, y, z };
        }
        public float[][][][] ToFloat4Dim()
        {
            return new float[1][][][].Select(s => s = new float[1][][].Select(f => f = new float[1][].Select(r => r = new float[3] { x, y, z }).ToArray()).ToArray()).ToArray();
        }
        public vec3s Clone()
        {
            return new vec3s(x, y, z);
        }
        public bool IsZero()
        {
            return x == 0 && y == 0 && z == 0;
        }
        public static vec3s operator +(vec3s a, vec3s b)
        {
            return new vec3s(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static vec3s operator -(vec3s a, vec3s b)
        {
            return new vec3s(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        public static vec3s operator *(vec3s a, vec3s b)
        {
            return new vec3s(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        public static vec3s operator /(vec3s a, vec3s b)
        {
            return new vec3s(a.x / b.x, a.y / b.y, a.z / b.z);
        }
        public static vec3s operator +(vec3s a, float b)
        {
            return new vec3s(a.x + b, a.y + b, a.z + b);
        }
        public static vec3s operator -(vec3s a, float b)
        {
            return new vec3s(a.x - b, a.y - b, a.z - b);
        }
        public static vec3s operator *(vec3s a, float b)
        {
            return new vec3s(a.x * b, a.y * b, a.z * b);
        }
        public static vec3s operator /(vec3s a, float b)
        {
            return new vec3s(a.x / b, a.y / b, a.z / b);
        }
        public static vec3s operator +(float b, vec3s a)
        {
            return new vec3s(b + a.x, b + a.y, b + a.z);
        }
        public static vec3s operator -(float b, vec3s a)
        {
            return new vec3s(b - a.x, b - a.y, b - a.z);
        }
        public static vec3s operator *(float b, vec3s a)
        {
            return new vec3s(b * a.x, b * a.y, b * a.z);
        }
        public static vec3s operator /(float b, vec3s a)
        {
            return new vec3s(b / a.x, b / a.y, b / a.z);
        }
        public static bool operator ==(vec3s b, vec3s a)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }
        public static bool operator !=(vec3s b, vec3s a)
        {
            return !(a.x == b.x && a.y == b.y && a.z == b.z);
        }
    }
    public static class 扩展
    { 

        public static void ToShapString(this int[] Value, string S = "")
        {
            Console.WriteLine(S + " " + string.Join(",", Value));
        }
        public static float Dist(this vec3s a, vec3s b)
        {
            float dist = (float)Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2) + Math.Pow(a.z - b.z, 2));
            return dist;
        }
        public static float dot(this vec3s a, vec3s b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }
        public static vec3s cross(this vec3s a, vec3s b)
        {
            return new vec3s(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
        }
        public static vec3s Norm(this vec3s a)
        {
            float dist = (float)Math.Sqrt((double)(a.x * a.x + a.y * a.y + a.z * a.z)) + 1e-5f;
            return new vec3s(a.x / dist, a.y / dist, a.z / dist);
        }
        public static void Print(this vec3s a, string s = "")
        {
            Console.WriteLine(s + "x:" + a.x + " y:" + a.y + " z:" + a.z);
        }
        public static string ReturnString(this vec3s a, string s = "")
        {
            return (s + "x:" + a.x + " y:" + a.y + " z:" + a.z);
        }
        public static void PrintClass<T>(this T t)
        {
            var type = t.GetType();
            var Fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            StringBuilder sb = new StringBuilder();
            foreach (var finfo in Fields)
            {
                var test = finfo.GetValue(t);
                if (test == null)
                    continue;
                sb.Append(finfo.Name.ToString());
                sb.Append(": ");
                sb.Append(test.ToString());
                sb.AppendLine();
            }
            Console.WriteLine(sb.ToString());
        }
        public static void PrintClass<T>(this T[] A)
        {
            string SS = "";
            foreach (var t in A)
            {
                var type = t.GetType();
                var Fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                StringBuilder sb = new StringBuilder();
                foreach (var finfo in Fields)
                {
                    var test = finfo.GetValue(t);
                    if (test == null)
                        continue;
                    sb.Append(finfo.Name.ToString());
                    sb.Append(": ");
                    sb.Append(test.ToString());
                    sb.AppendLine();
                }
                SS += sb.ToString();
            }
            Console.WriteLine(SS);
        }
        private static int ToBytesPerPixel(this PixelFormat format)
        {
            int ret;
            switch (format)
            {
                case PixelFormat.Format16bppArgb1555:
                    ret = 16;
                    break;
                case PixelFormat.Format16bppGrayScale:
                    ret = 16;
                    break;
                case PixelFormat.Format16bppRgb555:
                    ret = 16;
                    break;
                case PixelFormat.Format16bppRgb565:
                    ret = 16;
                    break;
                case PixelFormat.Format24bppRgb:
                    ret = 24;
                    break;
                case PixelFormat.Format32bppArgb:
                    ret = 32;
                    break;
                case PixelFormat.Format32bppPArgb:
                    ret = 32;
                    break;
                case PixelFormat.Format32bppRgb:
                    ret = 32;
                    break;
                case PixelFormat.Format48bppRgb:
                    ret = 48;
                    break;
                case PixelFormat.Format64bppArgb:
                    ret = 64;
                    break;
                case PixelFormat.Format64bppPArgb:
                    ret = 64;
                    break;
                case PixelFormat.Format1bppIndexed:
                case PixelFormat.Format8bppIndexed:
                case PixelFormat.Format4bppIndexed:
                case PixelFormat.Alpha:
                case PixelFormat.Canonical:
                case PixelFormat.Extended:
                case PixelFormat.Gdi:
                case PixelFormat.Indexed:
                case PixelFormat.Max:
                case PixelFormat.PAlpha:
                    throw new ArgumentException($"Given PixelFormat: {format} is not supported.");
                case PixelFormat.DontCare:
                    throw new ArgumentException("Given PixelFormat can't be DontCare.");
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }

            return ret / 8;
        }
        private static unsafe Bitmap ToBitmap(this NDArray nd, int width, int height, PixelFormat format = PixelFormat.DontCare)
        {
            if (nd == null)
                throw new ArgumentNullException(nameof(nd));

            //if flat then initialize based on given format
            if (nd.ndim == 1 && format != PixelFormat.DontCare)
                nd = nd.reshape(1, height, width, format.ToBytesPerPixel()); //theres a check internally for size mismatch.

            if (nd.ndim != 4)
                throw new ArgumentException("ndarray was expected to be of 4-dimensions, (1, bmpData.Height, bmpData.Width, bytesPerPixel)");

            if (nd.shape[0] != 1)
                throw new ArgumentException($"ndarray has more than one picture in it ({nd.shape[0]}) based on the first dimension.");

            var bbp = nd.shape[3]; //bytes per pixel.
            if (bbp != extractFormatNumber())
                throw new ArgumentException($"Given PixelFormat: {format} does not match the number of bytes per pixel in the 4th dimension of given ndarray.");

            if (bbp * width * height != nd.size)
                throw new ArgumentException($"The expected size does not match the size of given ndarray. (expected: {bbp * width * height}, actual: {nd.size})");

            var ret = new Bitmap(width, height, format);
            var bitdata = ret.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, format);
            try
            {
                var dst = new ArraySlice<byte>(new UnmanagedMemoryBlock<byte>((byte*)bitdata.Scan0, bitdata.Stride * bitdata.Height));
                if (nd.Shape.IsContiguous)
                    nd.CopyTo(dst);
                else
                    MultiIterator.Assign(new UnmanagedStorage(dst, Shape.Vector(bitdata.Stride * bitdata.Height)), nd.Unsafe.Storage);
            }
            finally
            {
                try
                {
                    ret.UnlockBits(bitdata);
                }
                catch (ArgumentException)
                {
                    //swallow
                }
            }

            return ret;

            int extractFormatNumber()
            {
                if (format == PixelFormat.DontCare)
                {
                    switch (bbp)
                    {
                        case 3:
                            format = PixelFormat.Format24bppRgb;
                            break;
                        case 4:
                            format = PixelFormat.Format32bppArgb;
                            break;
                        case 6:
                            format = PixelFormat.Format48bppRgb;
                            break;
                        case 8:
                            format = PixelFormat.Format64bppArgb;
                            break;
                    }

                    return bbp;
                }

                return format.ToBytesPerPixel();
            }
        }

        public static Bitmap ToBitmap_MustByteNDArray(this NDArray nd, PixelFormat format = PixelFormat.Format24bppRgb)
        {
            if (nd == null)
                throw new ArgumentNullException(nameof(nd));
            if (nd.ndim != 4)
                throw new ArgumentException("ndarray was expected to be of 4-dimensions, (1, bmpData.Height, bmpData.Width, bytesPerPixel)");
            if (nd.shape[0] != 1)
                throw new ArgumentException($"ndarray has more than one picture in it ({nd.shape[0]}) based on the first dimension.");

            var height = nd.shape[1];
            var width = nd.shape[2];

            return ToBitmap(nd, width, height, format);
        }
    }
}

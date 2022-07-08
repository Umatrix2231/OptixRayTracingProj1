using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptixProject1
{
    internal class sTimer
    {
        static System.DateTime T1 = DateTime.Now;
        static System.DateTime DelayRecord = DateTime.Now;
        public static void Start()
        {
            T1 = DateTime.Now;
        }
        public static string End(string s = "")
        {
            string S = s + (DateTime.Now - T1).TotalMilliseconds.ToString("f0") + " Milliseconds";
            Console.WriteLine(S);
            return S;
        }
        public static float EndFloat()
        {

            return (float)((DateTime.Now - T1).TotalMilliseconds);
        }

        public static bool GetDelayed(float delay)
        {
            if ((DateTime.Now - DelayRecord).TotalMilliseconds > delay)
            {
                DelayRecord = DateTime.Now;
                return true;
            }
            else return false;
        }
    }
}

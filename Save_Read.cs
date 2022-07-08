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
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using System.Xml;
namespace OptixProject1
{
    public class Save_Read
    {
        public static void SAVE_AS_XML<T>(T MC, string FilePath)
        {
            if (FilePath == null) return;

            FileStream stream = new FileStream(FilePath, FileMode.Create);

            XmlSerializer xmlserilize = new XmlSerializer(typeof(T));

            xmlserilize.Serialize(stream, MC);

            stream.Close();

        }
        public static T READ_AS_XML<T>(string FilePath)
        {
            FileStream stream = new FileStream(FilePath, FileMode.Open);
            XmlSerializer xmlserilize = new XmlSerializer(typeof(T));

            var MC = xmlserilize.Deserialize(stream);

            stream.Close();
            return (T)MC;
        }
        public static void SAVE(object MC, string FilePath)
        {
            if (FilePath == null) return;
            Stream steam = File.Open(FilePath, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(steam, MC);
            steam.Close();
        }
        public static Object READ(string FilePath)
        {
            if (FilePath == null) return null;
            string fName = FilePath;
            Stream steam2 = File.Open(fName, FileMode.Open);
            BinaryFormatter bf2 = new BinaryFormatter();
            Object MC = bf2.Deserialize(steam2);
            steam2.Close();
            return MC;
        }
        public static string SaveReturnFilePath(string suffix = "*.bmp")
        {
            SaveFileDialog openFileDialog = new SaveFileDialog(); 
            openFileDialog.Filter = "File|" + suffix;

            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return null;
        }
        public static string OpenReturnFilePath(string s = "*.models")
        {
            OpenFileDialog openFileDialog = new OpenFileDialog(); 
            openFileDialog.Filter = "File|" + s;

            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return null;
        }
    }
}

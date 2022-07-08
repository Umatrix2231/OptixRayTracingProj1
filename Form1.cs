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
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }




        bool MoveToThere = false;
        bool MoveAxisZ = false;
        Mat TemoColorShow = null;
        List<Model> models = new List<Model>();
        ViewPort camera = new ViewPort();
        static Random Random = new Random();
        public static readonly object locker = new object();
        static long TriCount = 0;
        DataEx.DevieParams DP = new DataEx.DevieParams();
        DataEx.SkyLight SkyL = new DataEx.SkyLight();
        static int FrameCount = 0;
        List<DataEx.ParticleSystemsStruct> PSS = new List<DataEx.ParticleSystemsStruct>(); 

        private unsafe void Form1_Load(object sender, EventArgs e)
        {



            TemoColorShow = new Mat(new OpenCvSharp.Size(pictureBox2.Width, pictureBox2.Height), MatType.CV_8UC3);
            DP.CloneInstance = -1;
            DP.InstanceIndex = -1;
            DP.DeletInstance = -1;
            SaveInstanceIndex = -1;
            DP.ComputeTransform = 1;
            DP.TraceMotion = -1;
            DP.FramePerTime = vScrollBar9.Value;
            DP.MaxReflectTimes = vScrollBar10.Value;
            SkyL.MissColor1 = (float)trackBar2.Value / 255f / 255f;
            SkyL.MissColor2 = (float)trackBar3.Value / 255f / 255f;
            SkyL.MissColor3 = (float)trackBar4.Value / 255f / 255f;
            SkyL.Missluminosity = (float)trackBar5.Value;
            SkyL.VolLightWeight = 0;
            SkyL.FogWeight = 0;
            SkyL.FogSinTime = trackBar14.Value;
            SkyL.FogZup = trackBar15.Value;
            SkyL.FogZDown = trackBar17.Value;
            SkyL.FogWaveSize = trackBar16.Value;
            SkyL.FogHeightBlurWeight = (float)trackBar18.Value / 1000f;


            this.ActiveControl = this.pictureBox1;
            new Thread(() => {

                TriCount = InitSence.SenceRoom(models, camera, pictureBox1.Width, pictureBox1.Height,listView1);

                BeginInvoke(new Action(() => { ModelOperation.ModelIndexShow(listView1, models); }));

                Rander();

            }).Start();


        }
        private void button1_Click(object sender, EventArgs e)
        {
            Rander();
        }


        private unsafe void Rander()
        {
            if (TriCount == 0) return;


            new Thread(() => {
                lock (locker)
                {
                    do
                    {
                        if (ReadModels == true)
                        {
                            ReadModels = false;
                            var IAWAIT = BeginInvoke(new Action(() => { models = CudaOperation.ReadModels(models, listView1, Save_Read.OpenReturnFilePath()); }));
                            while (!IAWAIT.IsCompleted) Thread.Sleep(10);
                            DP.ChangeNow = 1;
                        }
                        FrameCount++;
                        System.DateTime T1 = DateTime.Now;
                        (vec3s StartPoint, vec3s WV, vec3s HV) = Math2.ScreenGay(camera);
                        if (MoveW) { camera.Point += WV.cross(HV).Norm() * Scale_Move * 10f; }
                        if (MoveS) { camera.Point -= WV.cross(HV).Norm() * Scale_Move * 10f; }
                        if (MoveA) { camera.Point -= WV.Norm() * Scale_Move * 10f; }
                        if (MoveD) { camera.Point += WV.Norm() * Scale_Move * 10f; }
                        if (MoveUp) { camera.Point.z += Scale_Move * 10f; }
                        if (MoveDown) { camera.Point.z -= Scale_Move * 10f; }


                        DP.Denose = checkBox4.Checked ? 1 : 0;
                        (Mat ShowMap, float RayTime) = CudaOperation.OptixInter(camera, ref DP, SkyL, ModelOperation.GetMTFP(models));


                        var IA = BeginInvoke(new Action(() =>
                        {
                            if (SaveImageBool == true)
                            {
                                var ThisTime = System.DateTime.Now;
                                SaveImageBool = false;
                                ShowMap.SaveImage(@"SaveImages/" + ThisTime.Year + "年" + ThisTime.Month + "月" + ThisTime.Day + "日" + ThisTime.Hour + "时" + ThisTime.Minute + "分" + ThisTime.Second + "秒.bmp");
                            }
                          
                            float DisplayTime = (float)((DateTime.Now - T1).TotalMilliseconds) - RayTime;
                            string ShowString2 = "R:" + RayTime.ToString("f2") + " D:" + DisplayTime.ToString("f2") + " FPS:" + (1000f / RayTime).ToString("f0") + " FPS(s):" + (1000f / RayTime * (float)DP.FramePerTime).ToString("f2");
                            string ShowString1 = "MRD:" + DP.MaxReflectTimes + " LD:" + DP.FramePerTime;
                            Cv2.PutText(ShowMap, ShowString1, new OpenCvSharp.Point(10, 20), HersheyFonts.HersheyComplex, 0.7, new Scalar(0, 0, 0), 5);
                            Cv2.PutText(ShowMap, ShowString1, new OpenCvSharp.Point(10, 20), HersheyFonts.HersheyComplex, 0.7, new Scalar(255, 255, 255), 1);
                            Cv2.PutText(ShowMap, ShowString2, new OpenCvSharp.Point(10, 50), HersheyFonts.HersheyComplex, 0.7, new Scalar(0, 0, 0), 5);
                            Cv2.PutText(ShowMap, ShowString2, new OpenCvSharp.Point(10, 50), HersheyFonts.HersheyComplex, 0.7, new Scalar(255, 255, 255), 1);
                            if (SaveInstanceIndex != -1)
                            {
                                Cv2.PutText(ShowMap, models[SaveInstanceIndex].Point.ReturnString(), new OpenCvSharp.Point(10, pictureBox1.Height - 25), HersheyFonts.HersheyComplex, 0.7, new Scalar(0, 0, 0), 5);
                                Cv2.PutText(ShowMap, models[SaveInstanceIndex].Point.ReturnString(), new OpenCvSharp.Point(10, pictureBox1.Height - 25), HersheyFonts.HersheyComplex, 0.7, new Scalar(255, 255, 255), 1);
                            }
                            pictureBox1.Image = BitmapConverter.ToBitmap(ShowMap);
                            ShowMap.Dispose();
                        }));
                        while (!IA.IsCompleted) ;


                    } while (RunGoOn);
                }
            }).Start();
        }







        bool RunGoOn = true;
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            RunGoOn = checkBox2.Checked;

        }

        private void button12_Click(object sender, EventArgs e)
        {
            var cameraTemp = (ViewPort)Save_Read.READ("camera");
            camera.Point = cameraTemp.Point;
            camera.u = cameraTemp.u;
            camera.v = cameraTemp.v;
            camera.w = cameraTemp.w;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Save_Read.SAVE(camera, "camera");
        }

        float Scale_Move = 1;
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            Scale_Move = (float)trackBar1.Value / (float)trackBar1.Maximum * 10f;
            Scale_Move *= (float)trackBar13.Value / 10f;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            camera.w += 1f * Scale_Move;
        }

        private void button14_Click(object sender, EventArgs e)
        {

            camera.w -= 1f * Scale_Move;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(0);
        }

        int ModelIndex = -1;
        bool TextInChange = false;
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ModelIndex = listView1.Items.IndexOf(listView1.FocusedItem);
            if (ModelIndex >= 0 && ModelIndex < models.Count)
            {
                TextInChange = true;
                textBox2.Text = models[ModelIndex].Rotation.x.ToString() + "," + models[ModelIndex].Rotation.y.ToString() + "," + models[ModelIndex].Rotation.z.ToString();
                textBox6.Text = models[ModelIndex].Scale.x.ToString() + "," + models[ModelIndex].Scale.y.ToString() + "," + models[ModelIndex].Scale.z.ToString();
                new Thread(() => { Thread.Sleep(1000); TextInChange = false; }).Start();
            }
        }


        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ModelIndex = listView1.Items.IndexOf(listView1.FocusedItem);
                if (ModelIndex >= 0 && ModelIndex < models.Count)
                {
                    if (models[ModelIndex].CloneID == -1) { MessageBox.Show("Can't Delete"); return; }
                    models.RemoveAt(ModelIndex);
                    DP.DeletInstance = ModelIndex;
                    DP.ChangeNow = 1;
                    ModelOperation.ModelIndexShow(listView1, models);
                }
            }
        }



        bool ReadModels = false;
        private void button20_Click(object sender, EventArgs e)
        {
            ReadModels = true;
        }

        private void button19_Click(object sender, EventArgs e)
        {
            Save_Read.SAVE(models, Save_Read.SaveReturnFilePath("*.models"));
        }

        bool MouseRightDowns = false;
        bool MouseLeftDowns = false;
        bool TextureMouseDowns = false;
        float BefMouseX, BefMouseY;
        int SaveInstanceIndex = -1;
        vec3s SeletModelInterPoint = new vec3s();

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (FrameCount < 10) return;
            if (e.Button == MouseButtons.Left)
            {

                MouseLeftDowns = true;
                BefMouseX = e.X;
                BefMouseY = e.Y;
            }
            if (e.Button == MouseButtons.Middle)
            {
                TextureMouseDowns = true;
            }
            if (e.Button == MouseButtons.Right)
            {
                SeletModelInterPoint = new vec3s();
                DP.TraceMotion = 1;
                MouseRightDowns = true;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (FrameCount < 10) return;
            if (e.Button == MouseButtons.Left)
            {
                MouseLeftDowns = false;
            }
            if (e.Button == MouseButtons.Right)
            {
                MouseRightDowns = false;
                DP.TraceMotion = -1;
                DP.InstanceIndex = -1;
                SaveInstanceIndex = -1;
                if (ModelClone == true || ModelCloneToThere == true)
                {
                    bool[] CheckdItems = new bool[listView1.Items.Count];
                    for (int i = 0; i < CheckdItems.Length; i++) CheckdItems[i] = listView1.Items[i].Checked;
                    ModelOperation.ModelIndexShow(listView1, models);
                    for (int i = 0; i < CheckdItems.Length; i++) listView1.Items[i].Checked = CheckdItems[i];
                }
            }
            if (e.Button == MouseButtons.Middle)
            {
                TextureMouseDowns = false;
                SkyL.PaintingFlag = 0;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            DP.SeletScreenX = e.X;
            DP.SeletScreenY = e.Y;
            if (DP.InstanceIndex != -1 && MouseRightDowns == true && SaveInstanceIndex == -1) SaveInstanceIndex = DP.InstanceIndex;
            if (FrameCount < 10) return;
            if (TextureMouseDowns == true)
            {
                SkyL.PaintingDist = vScrollBar7.Value;
                SkyL.PaintingScreenX = e.X;
                SkyL.PaintingScreenY = e.Y;
                SkyL.PaintingToColor1 = vScrollBar1.Value;
                SkyL.PaintingToColor2 = vScrollBar2.Value;
                SkyL.PaintingToColor3 = vScrollBar3.Value;
                SkyL.PaintingToColor4 = vScrollBar8.Value;
                SkyL.ReflectValue = vScrollBar4.Value;
                SkyL.RefractionValue = vScrollBar5.Value;
                SkyL.TransmittanceValue = vScrollBar6.Value;
                SkyL.PaintingFlag = comboBox1.SelectedIndex + 1;
                DP.ChangeNow = 1;
            }
            if (MouseLeftDowns == true)
            {
                float uSet = e.X - BefMouseX;
                float VSet = e.Y - BefMouseY;
                BefMouseX = e.X;
                BefMouseY = e.Y;
                camera.u += uSet / 2f;
                camera.v -= VSet / 2f;

            }
            if (MoveToThere == true)
            {
                if (ModelIndex >= 0 && ModelIndex < models.Count && MouseRightDowns == true && DP.InstanceIndex != -1 && DP.InstanceIndex != ModelIndex)
                {

                    models[ModelIndex].Point = new vec3s(DP.InterPointX, DP.InterPointY, DP.InterPointZ);
                    models[ModelIndex].Point.z += models[ModelIndex].DiffZ * models[ModelIndex].Scale.z + 0.2f;
                    DP.ChangeNow = 1;
                }
            }
            else if (ModelClone == true || ModelCloneToThere == true)
            {
                if (ModelIndex >= 0 && ModelIndex < models.Count && MouseRightDowns == true)
                {
                    try
                    {
                        if (DP.CloneInstance == -1 && (ModelClone == true || ModelCloneToThere == true) && sTimer.GetDelayed(float.Parse(textBox1.Text)))
                        {
                            SeletModelInterPoint = new vec3s(DP.InterPointX, DP.InterPointY, DP.InterPointZ);


                            float X_OFFSET = float.Parse(textBox4.Text.Split(',')[0]);
                            float Y_OFFSET = float.Parse(textBox4.Text.Split(',')[1]);
                            float Z_OFFSET = float.Parse(textBox4.Text.Split(',')[2]);
                            float DiffZPer = float.Parse(textBox4.Text.Split(',')[3]) / 100f;
                            float X_RO = (float)(RandomClass.ran.NextDouble() * 2f - 1f) * float.Parse(textBox5.Text.Split(',')[0]);
                            float Y_RO = (float)(RandomClass.ran.NextDouble() * 2f - 1f) * float.Parse(textBox5.Text.Split(',')[1]);
                            float Z_RO = (float)(RandomClass.ran.NextDouble() * 2f - 1f) * float.Parse(textBox5.Text.Split(',')[2]);
                            float Scale = (float)(RandomClass.ran.NextDouble() * 2f - 1f) * float.Parse(textBox7.Text);

                            var NewModel = models[ModelIndex].ModelClone();
                            NewModel.CloneID = ModelIndex;


                            if (ModelClone == true)
                            {
                                float WSet = (float)e.X / (float)pictureBox1.Width;
                                float HSet = (float)e.Y / (float)pictureBox1.Height;
                                (vec3s StartPoint, vec3s WV, vec3s HV) = Math2.ScreenGay(camera);
                                vec3s LookAt = StartPoint + WV * WSet + HV * HSet;
                                vec3s RayV = LookAt - camera.Point;
                                vec3s N = new vec3s(0, 0, 1);
                                float dot1 = (NewModel.Point - camera.Point).dot(N);
                                float dot2 = (RayV).dot(N);
                                float t = dot1 / dot2;
                                if (t < 0) return;
                                vec3s Target = camera.Point + RayV * t;
                                NewModel.Point.x += Target.x - NewModel.Point.x;
                                NewModel.Point.y += Target.y - NewModel.Point.y;
                                NewModel.Rotation += new vec3s(X_RO, Y_RO, Z_RO);

                                models.Add(NewModel);
                                DP.CloneInstance = ModelIndex;
                                DP.ChangeNow = 1;
                                MouseRightDowns = false;
                            }
                            else if (ModelCloneToThere == true)
                            {
                                vec3s Target = SeletModelInterPoint;
                                NewModel.Point.x = Target.x;
                                NewModel.Point.y = Target.y;
                                NewModel.Point.z = Target.z;
                                NewModel.Point += new vec3s(X_OFFSET, Y_OFFSET, Z_OFFSET);
                                NewModel.Point.z += NewModel.DiffZ * NewModel.Scale.z * DiffZPer * (Scale + 1f);
                                NewModel.Rotation += new vec3s(X_RO, Y_RO, Z_RO);
                                NewModel.Scale *= (Scale + 1f);

                                if (DP.InstanceIndex == -1 || DP.InstanceIndex >= listView1.Items.Count) return;
                                if (listView1.Items[DP.InstanceIndex].Checked == false) return;

                                models.Add(NewModel);
                                DP.CloneInstance = ModelIndex;
                                DP.ChangeNow = 1;
                            }

                        }
                    }
                    catch { }
                }
            }
            else if (SaveInstanceIndex != -1)
            {
                DP.ChangeNow = 1;

                try
                {
                    if (MoveAxisZ == false)
                    {
                        if (SeletModelInterPoint == new vec3s()) SeletModelInterPoint = new vec3s(DP.InterPointX, DP.InterPointY, DP.InterPointZ);
                        float WSet = (float)e.X / (float)pictureBox1.Width;
                        float HSet = (float)e.Y / (float)pictureBox1.Height;
                        (vec3s StartPoint, vec3s WV, vec3s HV) = Math2.ScreenGay(camera);
                        vec3s LookAt = StartPoint + WV * WSet + HV * HSet;
                        vec3s RayV = LookAt - camera.Point;
                        vec3s N = new vec3s(0, 0, 1);
                        float dot1 = (SeletModelInterPoint - camera.Point).dot(N);
                        float dot2 = (RayV).dot(N);
                        float t = dot1 / dot2;
                        if (t < 0) return;
                        vec3s Target = camera.Point + RayV * t;
                        models[SaveInstanceIndex].Point.x += Target.x - SeletModelInterPoint.x;
                        models[SaveInstanceIndex].Point.y += Target.y - SeletModelInterPoint.y;
                        SeletModelInterPoint = Target.Clone();
                    }
                    else
                    {
                        if (SeletModelInterPoint == new vec3s()) SeletModelInterPoint = new vec3s(DP.InterPointX, DP.InterPointY, DP.InterPointZ);
                        float WSet = (float)e.X / (float)pictureBox1.Width;
                        float HSet = (float)e.Y / (float)pictureBox1.Height;
                        (vec3s StartPoint, vec3s WV, vec3s HV) = Math2.ScreenGay(camera);
                        vec3s LookAt = StartPoint + WV * WSet + HV * HSet;
                        vec3s RayV = LookAt - camera.Point;
                        vec3s N = RayV * new vec3s(1, 1, 0);
                        float dot1 = (SeletModelInterPoint - camera.Point).dot(N);
                        float dot2 = (RayV).dot(N);
                        float t = dot1 / dot2;
                        if (t < 0) return;
                        vec3s Target = camera.Point + RayV * t;
                        models[SaveInstanceIndex].Point.z += Target.z - SeletModelInterPoint.z;
                        SeletModelInterPoint = Target.Clone();

                    }
                }
                catch { }
            }
        }


        bool MoveW = false;
        bool MoveA = false;
        bool MoveS = false;
        bool MoveD = false;
        bool MoveUp = false;
        bool MoveDown = false;
        private void Pic1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) MoveW = true;
            if (e.KeyCode == Keys.A) MoveA = true;
            if (e.KeyCode == Keys.S) MoveS = true;
            if (e.KeyCode == Keys.D) MoveD = true;
            if (e.KeyCode == Keys.E) MoveUp = true;
            if (e.KeyCode == Keys.Q) MoveDown = true;
        }
        private void Pic1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) MoveW = false;
            if (e.KeyCode == Keys.A) MoveA = false;
            if (e.KeyCode == Keys.S) MoveS = false;
            if (e.KeyCode == Keys.D) MoveD = false;
            if (e.KeyCode == Keys.E) MoveUp = false;
            if (e.KeyCode == Keys.Q) MoveDown = false;
        }
        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }

        bool SaveImageBool = false;
        private void button22_Click(object sender, EventArgs e)
        {
            SaveImageBool = true;
        }


        private void SetAxisAndAngle()
        {
            try
            {
                if (TextInChange == true) return;
                models[ModelIndex].Rotation.x = float.Parse(textBox2.Text.Split(',')[0]);
                models[ModelIndex].Rotation.y = float.Parse(textBox2.Text.Split(',')[1]);
                models[ModelIndex].Rotation.z = float.Parse(textBox2.Text.Split(',')[2]);
                models[ModelIndex].Scale.x = float.Parse(textBox6.Text.Split(',')[0]);
                models[ModelIndex].Scale.y = float.Parse(textBox6.Text.Split(',')[1]);
                models[ModelIndex].Scale.z = float.Parse(textBox6.Text.Split(',')[2]);
                DP.ChangeNow = 1;
            }
            catch { }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            SetAxisAndAngle();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            new Thread(() => {
                try
                {
                    while (checkBox1.Checked == true)
                    {
                        if (DP.ChangeNow == 0)
                        {
                            DP.ChangeNow = 1;
                            models[ModelIndex].Rotation.x += float.Parse(textBox3.Text.Split(',')[0]);
                            models[ModelIndex].Rotation.y += float.Parse(textBox3.Text.Split(',')[1]);
                            models[ModelIndex].Rotation.z += float.Parse(textBox3.Text.Split(',')[2]);
                        }
                        Thread.Sleep(5);
                    }
                }
                catch { }
            }).Start();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            new Thread(() => {
                try
                {
                    while (checkBox3.Checked == true)
                    {
                        if (DP.ChangeNow == 0)
                        {
                            DP.ChangeNow = 1;
                            models[ModelIndex].Rotation.x -= float.Parse(textBox3.Text.Split(',')[0]);
                            models[ModelIndex].Rotation.y -= float.Parse(textBox3.Text.Split(',')[1]);
                            models[ModelIndex].Rotation.z -= float.Parse(textBox3.Text.Split(',')[2]);
                        }
                        Thread.Sleep(5);
                    }
                }
                catch { }
            }).Start();
        }


        private void ProcessSkyLight()
        {

            SkyL.MissColor1 = (float)trackBar2.Value / 255f / 255f;
            SkyL.MissColor2 = (float)trackBar3.Value / 255f / 255f;
            SkyL.MissColor3 = (float)trackBar4.Value / 255f / 255f;
            SkyL.Missluminosity = (float)trackBar5.Value;
            SkyL.VolLightWeight = (float)trackBar10.Value / 100f * (float)trackBar11.Value;
            DP.ChangeNow = 1;
        }
        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            ProcessSkyLight();
        }
        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            ProcessSkyLight();
        }

        private void trackBar4_ValueChanged(object sender, EventArgs e)
        {
            ProcessSkyLight();
        }

        private void trackBar5_ValueChanged(object sender, EventArgs e)
        {
            ProcessSkyLight();
        }
        private void trackBar9_ValueChanged(object sender, EventArgs e)
        {
            ProcessSkyLight();
        }
        private void trackBar10_ValueChanged(object sender, EventArgs e)
        {
            ProcessSkyLight();
        }
        private void trackBar11_ValueChanged(object sender, EventArgs e)
        {
            ProcessSkyLight();
        }

        private void trackBar6_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                models[ModelIndex].luminosityMul = (float)trackBar6.Value * trackBar12.Value / 100f;
                DP.ChangeNow = 1;
            }
            catch { }
        }
        private void trackBar12_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                models[ModelIndex].luminosityMul = (float)trackBar6.Value * trackBar12.Value / 100f;
                DP.ChangeNow = 1;
            }
            catch { }
        }
        private void trackBar7_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                models[ModelIndex].TextureScale = (float)trackBar7.Value / 100f * (float)trackBar8.Value / 20f;
                DP.ChangeNow = 1;
            }
            catch { }
        }

        private void trackBar8_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                models[ModelIndex].TextureScale = (float)trackBar7.Value / 100f * (float)trackBar8.Value / 20f;
                DP.ChangeNow = 1;
            }
            catch { }
        }


        private void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            int B, G, R;
            B = vScrollBar1.Value;
            G = vScrollBar2.Value;
            R = vScrollBar3.Value;
            TemoColorShow.SetTo(new Scalar(B, G, R));
            pictureBox2.Image = TemoColorShow.ToBitmap();
        }

        private void vScrollBar4_ValueChanged(object sender, EventArgs e)
        {
            label13.Text = "Refl:" + (float)vScrollBar4.Value / 100f;
            label14.Text = "Refr:" + (float)vScrollBar5.Value / 100f;
            label15.Text = "Tans:" + (float)vScrollBar6.Value / 100f;
        }

        private void vScrollBar7_ValueChanged(object sender, EventArgs e)
        {
            label16.Text = "D:" + vScrollBar7.Value;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            MoveAxisZ = checkBox5.Checked;
        }



        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            SetAxisAndAngle();
        }

        bool ModelClone = false;
        bool ModelCloneToThere = false;
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            ModelClone = checkBox6.Checked;
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            MoveToThere = checkBox7.Checked;
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            ModelCloneToThere = checkBox8.Checked;
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
        }

        private void vScrollBar9_ValueChanged(object sender, EventArgs e)
        {
            DP.FramePerTime = vScrollBar9.Value;
            label23.Text = "FramesPerTime:" + vScrollBar9.Value;
        }

        private void vScrollBar10_ValueChanged(object sender, EventArgs e)
        {
            DP.MaxReflectTimes = vScrollBar10.Value;
            label24.Text = "MaxReflectTimes:" + vScrollBar10.Value;
            DP.ChangeNow = 1;
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            DP.ComputeTransform = checkBox9.Checked == true ? (byte)1 : (byte)0;
        }

        private void trackBar9_ValueChanged_1(object sender, EventArgs e)
        {
            SkyL.FogWeight = 1f - (float)trackBar9.Value / 1000f;
            SkyL.FogSinTime = trackBar14.Value;
            SkyL.FogZup = trackBar15.Value;
            SkyL.FogZDown = trackBar15.Value - trackBar17.Value;
            SkyL.FogWaveSize = trackBar16.Value;
            SkyL.FogHeightBlurWeight = (float)trackBar18.Value / 1000f;
            DP.ChangeNow = 1;
        }

        bool RecordVideo = false;
        int RecordFrameEven = 0;
        int RecordFrameIter = 0;
        int RecordFrameCount = 0;
        float Step_SinTime = 0.05f;
        float Step_SinX = 0.05f;
        float Step_SinY = 0.05f;
        vec3s OBJ_Rotation = new vec3s();
        bool Step_Test = false;
        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            RecordFrameEven = int.Parse(textBox8.Text);
        }
        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            Step_SinTime = float.Parse(textBox9.Text);
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

            Step_SinX = float.Parse(textBox10.Text);
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {

            Step_SinY = float.Parse(textBox11.Text);
        }
        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OBJ_Rotation.x -= float.Parse(textBox12.Text.Split(',')[0]);
                OBJ_Rotation.y = float.Parse(textBox12.Text.Split(',')[1]);
                OBJ_Rotation.z = float.Parse(textBox12.Text.Split(',')[2]);
                OBJ_Rotation.Print();
            }
            catch { }
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            Step_Test = checkBox11.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            SkyL = (DataEx.SkyLight)Save_Read.READ("SkyL");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Save_Read.SAVE(SkyL, "SkyL");
        }


        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
           

        }


    }
}

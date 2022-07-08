using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using System.Windows.Forms;
namespace OptixProject1
{
    internal class CudaOperation
    {

        [DllImport("RanderCore.dll")]
        private static extern int InitCuda(int wid, int hei);

        [DllImport("RanderCore.dll")]
        private static extern int Compute_Inter_Dist_Fast(IntPtr Color, IntPtr Normal);


        [DllImport("optixDynamicGeometry.dll")]
        private static extern int main(int w, int h);

        [DllImport("optixDynamicGeometry.dll")]
        private static extern int RunInter(IntPtr GetFinalColor, IntPtr Camera_Param, IntPtr DP, IntPtr SkyL, IntPtr ModelsTransform, int MTFPLen);

        [DllImport("optixDynamicGeometry.dll")]
        private static extern float SendTriangleToOptix(IntPtr TriAbit, IntPtr TriFloat3, int LenTriAbit, int LenTriFloat3, int _MTFPlen, IntPtr Skylights);



        [DllImport("optixDynamicGeometry.dll")]
        private static extern void SendTexture(IntPtr TextureData, int wid, int hei);
        [DllImport("optixDynamicGeometry.dll")]
        private static extern void SendReflTexture(IntPtr TextureData, int wid, int hei);
        [DllImport("optixDynamicGeometry.dll")]
        private static extern void SendRefrTexture(IntPtr TextureData, int wid, int hei);
        [DllImport("optixDynamicGeometry.dll")]
        private static extern void PushTextureToDevice();
        [DllImport("optixDynamicGeometry.dll")]
        private static extern void CloneToInstanceInput_2();
        [DllImport("optixDynamicGeometry.dll")]
        private static extern void CloneToInstance_1(int InstanceIndex);

        [DllImport("optixDynamicGeometry.dll")]
        private static extern void Find_New_Model_0(int InstanceIndex);

        public static void CloneToOptix(int InstanceIndex) => CloneToInstance_1(InstanceIndex);
        public static void CloneToOptixDone() => CloneToInstanceInput_2();

        private static bool IsCloneModel(Model Mv) => Mv.CloneID == -1 ? false : true;
        public static List<Model> ReadModels(List<Model> models, ListView Lv,string file)
        {
            var LoadModels = (List<Model>)Save_Read.READ(file);

            List<Model> NewModel = new List<Model>();
            List<Model> FinalList = new List<Model>();


            Console.WriteLine("GetModelSrc");
            for (int i = 0; i < LoadModels.Count; i++)
                if (!IsCloneModel(LoadModels[i]))
                {
                    FinalList.Add(LoadModels[i]);
                }

            for (int i = 0; i < FinalList.Count; i++)
            {
                FinalList[i].DiffZ = models[i].DiffZ;
                FinalList[i].Name = models[i].Name;
            }

            int MaxSrcModelCount = FinalList.Count;
            int NewModelDiffIndexToCLoneModel = models.Count - LoadModels.Count;

            Console.WriteLine("AddNewModel");
            for (int i = 0; i < models.Count; i++)
            {

                if (i >= LoadModels.Count())
                {
                    Console.WriteLine("AddNewModel Done");
                    NewModel.Add(models[i]);
                }
                else if (IsCloneModel(LoadModels[i]) && !IsCloneModel(models[i]))
                {
                    Console.WriteLine("AddNewModel Done");
                    NewModel.Add(models[i]);
                }
            }




            Console.WriteLine("AddNewModel To List");
            for (int j = 0; j < NewModel.Count; j++)
            {
                FinalList.Add(NewModel[j]);
            }


            Console.WriteLine("GetCloneModel");
            for (int j = 0; j < LoadModels.Count; j++)
                if (IsCloneModel(LoadModels[j]))
                {
                    FinalList.Add(LoadModels[j]);
                }


            ModelOperation.ModelIndexShow(Lv, FinalList);


            Console.WriteLine("SendCloneModelToOptix");
            for (int i = 0; i < FinalList.Count; i++)
                if (IsCloneModel(FinalList[i]))
                {
                    int RealCloneID = FinalList[i].CloneID;
                    if (RealCloneID >= MaxSrcModelCount) RealCloneID = FinalList[i].CloneID + NewModelDiffIndexToCLoneModel;
                    CloneToInstance_1(RealCloneID);
                }

            Console.WriteLine("Finishi Load");
            CloneToInstanceInput_2();

            Console.WriteLine("Finishi Load");
            return FinalList;
        }

        public static void OptixInit(int w, int h, List<Model> models, DataEx.SkyLight SL)
        {
            OptixSendTriangle(models, SL);
            models.ForEach(s => {
                SendTexture(s.TextureData, s.Tex_w, s.Tex_h);
                Marshal.FreeCoTaskMem(s.TextureData);
            });
            models.ForEach(s =>
            {
                SendReflTexture(s.ReflTextureData, s.ReflTex_w, s.ReflTex_h);
                Marshal.FreeCoTaskMem(s.ReflTextureData);
            }
            );
            models.ForEach(s =>
            {
                SendRefrTexture(s.RefrTextureData, s.RefrTex_w, s.RefrTex_h);
                Marshal.FreeCoTaskMem(s.RefrTextureData);
            });
            PushTextureToDevice();
            main(w, h);
        }
        public static void OptixSendTriangle(List<Model> models, DataEx.SkyLight SL)
        {
            IntPtr SkyLights = DataEx.StructureToPtr(SL);

            for (int j = 0; j < models.Count; j++)
            {
                List<DataEx.Triangle_Cuda> TriAbitData = new List<DataEx.Triangle_Cuda>();
                List<Vec3f> TriFloat3Data = new List<Vec3f>();
                for (int i = 0; i < models[j].tri_list.Count; i++)
                {
                    var P1 = models[j].tri_list[i].P1;
                    var P2 = models[j].tri_list[i].P2;
                    var P3 = models[j].tri_list[i].P3;
                    var uv1 = models[j].tri_list[i].VT1;
                    var uv2 = models[j].tri_list[i].VT2;
                    var uv3 = models[j].tri_list[i].VT3;
                    var N = models[j].tri_list[i].N;

                    DataEx.Triangle_Cuda TriAbitTemp = new DataEx.Triangle_Cuda();


                    TriAbitTemp.DiffsePassToReturn = models[j].tri_list[i].DiffsePassReturn;
                    TriAbitTemp.DiffsePassAngle = (byte)(models[j].tri_list[i].DiffsePassAngle * 100f);
                    TriAbitTemp.DiffsePassChance = (byte)(models[j].tri_list[i].DiffsePassChance * 100f);
                    TriAbitTemp.FogPassChance = (byte)(models[j].tri_list[i].FOG * 100f);
                    TriAbitTemp.MASK = (byte)(models[j].tri_list[i].MASK);
                    TriAbitTemp.u1 = uv1.x;
                    TriAbitTemp.u2 = uv2.x;
                    TriAbitTemp.u3 = uv3.x;

                    TriAbitTemp.v1 = uv1.y;
                    TriAbitTemp.v2 = uv2.y;
                    TriAbitTemp.v3 = uv3.y;

                    TriAbitTemp.Nx = N.x;
                    TriAbitTemp.Ny = N.y;
                    TriAbitTemp.Nz = N.z;
                    TriAbitTemp.Color1 = models[j].tri_list[i].Color.x;
                    TriAbitTemp.Color2 = models[j].tri_list[i].Color.y;
                    TriAbitTemp.Color3 = models[j].tri_list[i].Color.z;
                    TriAbitTemp.luminosity = models[j].tri_list[i].luminosity / 255f;
                    TriAbitTemp.ReflectValue = (byte)(models[j].tri_list[i].Refl * 100f);
                    TriAbitTemp.RefractionValue = (byte)(models[j].tri_list[i].Refr * 100f);
                    TriAbitTemp.TransmittanceValue = (byte)(models[j].tri_list[i].Trans * 100f);
                    TriAbitTemp.ModelID = j;

                    TriAbitData.Add(TriAbitTemp);
                    TriFloat3Data.Add(new Vec3f(P1.x, P1.y, P1.z));
                    TriFloat3Data.Add(new Vec3f(P2.x, P2.y, P2.z));
                    TriFloat3Data.Add(new Vec3f(P3.x, P3.y, P3.z));
                }

                IntPtr TriAbitDataPtr = DataEx.StructureToPtr(TriAbitData.ToArray());
                IntPtr TriFloat3DataPtr = DataEx.StructureToPtr(TriFloat3Data.ToArray());

                Console.WriteLine("TriSend:" + SendTriangleToOptix(TriAbitDataPtr, TriFloat3DataPtr, TriAbitData.Count, TriFloat3Data.Count(), models.Count, SkyLights));


                Marshal.FreeCoTaskMem(TriAbitDataPtr);
                Marshal.FreeCoTaskMem(TriFloat3DataPtr);
            }

            Marshal.FreeCoTaskMem(SkyLights);
            models.ForEach(s =>
            {
                s.tri_list.Clear();
            });
        }

        static ViewPort CmaeraBefore = new ViewPort();
        public static (Mat mat, float RanderTime) OptixInter(ViewPort camera, ref DataEx.DevieParams DP, DataEx.SkyLight SkyL, DataEx.ModelTransformParam[] MTFP)
        {
            if (CmaeraBefore.Point == new vec3s(0, 0, 0)) CmaeraBefore = camera.Clone();
            bool Changed = CmaeraBefore.ChangedValue(camera) ? true : false;
            CmaeraBefore = camera.Clone();
            DP.ChangeNow = Changed == true ? 1 : DP.ChangeNow;

            IntPtr GetCameraParams = DataEx.StructureToPtr(camera.GetCameraParams());
            IntPtr GetDeviceData = DataEx.AllocHGlobal<Vec3b>(camera.Wid * camera.Hei);
            IntPtr MTFP_SEND = DataEx.StructureToPtr(MTFP);
            IntPtr DPParams = DataEx.StructureToPtr(DP);
            IntPtr SkyLParams = DataEx.StructureToPtr(SkyL);

            sTimer.Start();
            RunInter(GetDeviceData, GetCameraParams, DPParams, SkyLParams, MTFP_SEND, MTFP.Length);
            float Raytime = sTimer.EndFloat();

            var TempDP = DataEx.PtrToStructure<DataEx.DevieParams>(DPParams);
            DP.ChangeNow = TempDP.ChangeNow == 1 ? 0 : DP.ChangeNow;
            DP.InstanceIndex = TempDP.InstanceIndex;
            DP.InterPointX = TempDP.InterPointX;
            DP.InterPointY = TempDP.InterPointY;
            DP.InterPointZ = TempDP.InterPointZ;
            if (TempDP.CloneInstance != -1) DP.CloneInstance = -1;
            if (TempDP.DeletInstance != -1) DP.DeletInstance = -1;


            Marshal.FreeCoTaskMem(GetCameraParams);
            Marshal.FreeCoTaskMem(MTFP_SEND);
            // Marshal.FreeCoTaskMem(DPParams);
            Marshal.FreeCoTaskMem(SkyLParams);

            var DATA = DataEx.PtrToByte<Vec3b>(GetDeviceData, camera.Wid * camera.Hei);

            Mat mat = new Mat(camera.Hei, camera.Wid, MatType.CV_8UC3, DATA);

            return (mat, Raytime);
        }

        public static void InitCuda(ViewPort camera)
        {
            // Console.WriteLine("InitCuda...");
            // Console.WriteLine("InitCuda:" + InitCuda(camera.Wid, camera.Hei));
        }

    }
}

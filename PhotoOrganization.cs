using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Harmony;
using VRCModLoader;

namespace PhotoOrganization
{
    public static class ModInfo
    {
        public const string Name = "PhotoOrganization";
        public const string Author = "Herp Derpinstine";
        public const string Company = "NanoNuke @ nanonuke.net";
        public const string Version = "1.0.0";
    }
    [VRCModInfo(ModInfo.Name, ModInfo.Version, ModInfo.Author)]

    public class PhotoOrganization : VRCMod
    {
        private static string FolderPath;

        void OnApplicationStart()
        {
            FolderPath = string.Format("{0}/VRChat", new object[] { Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) });
            if (Directory.Exists(FolderPath))
            {
                string[] filepaths = Directory.GetFiles(FolderPath, "VRChat_*.png", SearchOption.TopDirectoryOnly);
                if (filepaths.Length > 0)
                {
                    foreach (string file in filepaths)
                    {
                        DateTime fileinfo = File.GetLastWriteTime(file);
                        string filefolderpath = string.Format("{0}/{1}", new object[] { FolderPath, fileinfo.ToString("yyyy-MM-dd") });
                        if (!Directory.Exists(filefolderpath))
                            Directory.CreateDirectory(filefolderpath);
                        File.Move(file, string.Format("{0}/{1}.png", new object[] { filefolderpath, fileinfo.ToString("HH-mm-ss.fff") }));
                    }
                }
            }

            HarmonyInstance harmonyInstance = HarmonyInstance.Create("photoorganization");
            harmonyInstance.Patch(typeof(VRCCaptureUtils.CameraUtil).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Where(x => ((x.GetParameters().Count() == 2) && (x.GetParameters()[0].ParameterType == typeof(int)) && (x.GetParameters()[1].ParameterType == typeof(int)))).ToArray()[5], new HarmonyMethod(typeof(PhotoOrganization).GetMethod("CameraFolderOrganize", BindingFlags.Static | BindingFlags.NonPublic)));
        }

        private static bool CameraFolderOrganize(ref string __result) { __result = string.Format("{0}/{1}/{2}.png", new object[] { FolderPath, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH-mm-ss.fff") }); return false; }
    }
}

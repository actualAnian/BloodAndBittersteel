using System.IO;

namespace LanceSystem
{
    public class PathHelper
    {
        public static string MainModuleRootPath => Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).Parent.Parent.FullName;
        public static string MainModuleDataPath => Path.Combine(MainModuleRootPath, "ModuleData");
        public static string OutsideConfigPath => Path.Combine(MainModuleDataPath, "Configs");
    }
}

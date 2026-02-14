using TaleWorlds.ModuleManager;

namespace BloodAndBittersteel
{
    public static class PathHelper
    {
        public static string BaBMainModuleRootPath => ModuleHelper.GetModuleFullPath("BloodAndBittersteel");
        public static string BaBMainModuleDataPath => BaBMainModuleRootPath + "ModuleData/";
        public static string BaBOutsideConfigPath => BaBMainModuleDataPath + "Configs/";
    }
}

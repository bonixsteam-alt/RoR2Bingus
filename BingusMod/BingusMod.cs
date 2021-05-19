using BepInEx;
using BepInEx.Logging;
using R2API;
using R2API.Utils;

namespace BingusMod
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI))]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class CustomItem : BaseUnityPlugin
    {
        private const string ModVer = "1.0.0";
        private const string ModName = "Bingus Mod";
        public const string ModGuid = "com.bonix.bingusmod";

        internal new static ManualLogSource Logger; // allow access to the logger across the plugin classes

        public void Awake()
        {
            Logger = base.Logger;

            Assets.Init();
        }
    }
}
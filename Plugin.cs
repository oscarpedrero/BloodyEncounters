using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using Bloodstone.API;
using Unity.Entities;
using VampireCommandFramework;
using BloodyEncounters.DB;
using Bloodstone.Hooks;
using BloodyEncounters.Configuration;
using BloodyEncounters.Systems;
using Bloody.Core;
using Bloody.Core.API.v1;

namespace BloodyEncounters
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("gg.deca.VampireCommandFramework")]
    [BepInDependency("gg.deca.Bloodstone")]
    [BepInDependency("trodi.Bloody.Core")]
    public class Plugin : BasePlugin, IRunOnInitialized
    {
        internal static Plugin Instance { get; private set; }

        private static Harmony _harmony;

        public static Bloody.Core.Helper.v1.Logger Logger;
        public static SystemsCore SystemsCore;


        public override void Load()
        {
            Logger = new(Log);
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
            EventsHandlerSystem.OnInitialize += GameDataOnInitialize;
            EventsHandlerSystem.OnDestroy += GameDataOnDestroy;

            CommandRegistry.RegisterAll();

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        public override bool Unload()
        {
            EventsHandlerSystem.OnInitialize -= GameDataOnInitialize;
            EventsHandlerSystem.OnDestroy -= GameDataOnDestroy;
            Config.Clear();
            EncounterSystem.Destroy();
            _harmony?.UnpatchSelf();
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is unloaded!");
            return true;
        }

        public void OnGameInitialized()
        {
            
        }

        private static void GameDataOnInitialize(World world)
        {

            SystemsCore = Core.SystemsCore;

            Logger.LogInfo("Loading main data");
            Database.Initialize();
            Logger.LogInfo("Binding configuration");
            PluginConfig.Initialize();

            EncounterSystem.Initialize();

            if (PluginConfig.Enabled.Value)
            {
                TimerSystem.StartEncounterTimer();
            }

        }

        private static void GameDataOnDestroy()
        {
            //Logger.LogInfo("GameDataOnDestroy");
        }
    }
}

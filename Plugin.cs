using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Configuration;
using BepInEx.Logging;
using VRising.GameData;
using HarmonyLib;
using Bloodstone.API;
using Unity.Entities;
using VampireCommandFramework;
using BloodyEncounters.DB;
using Bloodstone.Hooks;
using BloodyEncounters.Configuration;
using StunMetrics.Metrics;
using BloodyEncounters.Systems;
using BloodyEncounters.Patch;
using BloodyEncounters.Services;

namespace BloodyEncounters
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("gg.deca.VampireCommandFramework")]
    [BepInDependency("gg.deca.Bloodstone")]

    public class Plugin : BasePlugin, IRunOnInitialized
    {

        public static ManualLogSource Logger;
        internal static Plugin Instance { get; private set; }

        private static Harmony _harmony;

        public static World World;


        public override void Load()
        {
            Logger = Log;
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
            _harmony.PatchAll(typeof(ServerEvents));
            _harmony.PatchAll(typeof(UnitSpawnerService));
            GameData.OnInitialize += GameDataOnInitialize;
            GameData.OnDestroy += GameDataOnDestroy;

            GameFrame.Initialize();

            CommandRegistry.RegisterAll();

            // Plugin startup logic
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        public override bool Unload()
        {
            GameData.OnInitialize -= GameDataOnInitialize;
            GameData.OnDestroy -= GameDataOnDestroy;
            Config.Clear();
            TimerSystem._encounterTimer?.Stop();
            EncounterSystem.Destroy();
            GameFrame.Uninitialize();
            _harmony?.UnpatchSelf();
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is unloaded!");
            return true;
        }

        public void OnGameInitialized()
        {
            World = VWorld.Server;
        }

        private static void GameDataOnInitialize(World world)
        {
            Logger.LogInfo("Loading main data");
            Database.Initialize();
            Logger.LogInfo("Binding configuration");
            PluginConfig.Initialize();

            EncounterSystem.Initialize();

            TimerSystem._encounterTimer = new Components.Timer();
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

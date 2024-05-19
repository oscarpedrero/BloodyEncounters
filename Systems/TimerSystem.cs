using BloodyEncounters.Configuration;
using BloodyEncounters.Patch;
using System;

namespace BloodyEncounters.Systems
{
    internal class TimerSystem
    {
        public static Action encounterAction;
        public static void StartEncounterTimer()
        {

            DateTime newEncounter = DateTime.Now.AddSeconds(new Random().Next(PluginConfig.EncounterTimerMin.Value, PluginConfig.EncounterTimerMax.Value));
            Plugin.Logger.LogInfo($"Start Timner for encounters");
            Plugin.Logger.LogInfo($"Next match will be shot at {newEncounter:yyyy-MM-dd HH:mm:ss}");

            encounterAction = () =>
            {
                if (PluginConfig.Enabled.Value)
                {
                    if(newEncounter <= DateTime.Now)
                    {
                        EncounterSystem.StartEncounter();
                        newEncounter = DateTime.Now.AddSeconds(new Random().Next(PluginConfig.EncounterTimerMin.Value, PluginConfig.EncounterTimerMax.Value));
                        Plugin.Logger.LogInfo($"Next match will be shot at {newEncounter:yyyy-MM-dd HH:mm:ss}");
                    }
                    ActionSchedulerPatch.RunActionOnceAfterFrames(encounterAction, 30);
                }
            };
            ActionSchedulerPatch.RunActionOnceAfterFrames(encounterAction, 30);
        }
    }
}

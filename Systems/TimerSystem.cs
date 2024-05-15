using Bloody.Core.Patch.Server;
using BloodyEncounters.Configuration;
using BloodyEncounters.DB;
using System;

namespace BloodyEncounters.Systems
{
    internal class TimerSystem
    {

        public static void StartEncounterTimer()
        {
            Plugin.Logger.LogInfo("Start Timner for encounters");
            var action = () =>
            {
                if (PluginConfig.Enabled.Value)
                {
                    EncounterSystem.StartEncounter();
                }
            };
            ActionSchedulerPatch.RunActionEveryInterval(action, new Random().Next(PluginConfig.EncounterTimerMin.Value, PluginConfig.EncounterTimerMax.Value));
        }
    }
}

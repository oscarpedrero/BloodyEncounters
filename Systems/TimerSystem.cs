using Bloody.Core.API.v1;
using Bloody.Core.Patch.Server;
using BloodyEncounters.Configuration;
using System;

namespace BloodyEncounters.Systems
{
    internal class TimerSystem
    {
        public static Action encounterAction;
        public static void StartEncounterTimer()
        {

            Plugin.Logger.LogInfo($"Start Timner for encounters");

            encounterAction = () =>
            {
                if (PluginConfig.Enabled.Value)
                {
                    Plugin.Logger.LogInfo($"Encounters Timer");
                    var startAction = () =>
                    {
                        EncounterSystem.StartEncounter();
                    };

                    ActionScheduler.RunActionOnMainThread( startAction );
                    
                }
            };

            CoroutineHandler.StartRandomIntervalCoroutine(encounterAction, PluginConfig.EncounterTimerMin.Value, PluginConfig.EncounterTimerMax.Value);
        }
    }
}

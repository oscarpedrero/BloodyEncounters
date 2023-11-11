using BloodyEncounters.Components;
using BloodyEncounters.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodyEncounters.Systems
{
    internal class TimerSystem
    {
        public static Timer _encounterTimer;

        public static void StartEncounterTimer()
        {
            _encounterTimer.Start(
                _ =>
                {
                    Plugin.Logger.LogDebug($"{DateTime.Now.ToString()} Starting an encounter.");
                    EncounterSystem.StartEncounter();
                },
                input =>
                {
                    if (input is not int onlineUsersCount)
                    {
                        Plugin.Logger.LogError("Encounter timer delay function parameter is not a valid integer");
                        return TimeSpan.MaxValue;
                    }
                    if (onlineUsersCount < 1)
                    {
                        onlineUsersCount = 1;
                    }
                    var seconds = new Random().Next(PluginConfig.EncounterTimerMin.Value, PluginConfig.EncounterTimerMax.Value);
                    Plugin.Logger.LogDebug($"{DateTime.Now.ToString()} Next encounter will start in {seconds / onlineUsersCount} seconds.");
                    return TimeSpan.FromSeconds(seconds) / onlineUsersCount;
                });
        }
    }
}

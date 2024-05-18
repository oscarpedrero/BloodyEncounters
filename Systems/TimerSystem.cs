using Bloody.Core.Patch.Server;
using BloodyEncounters.Configuration;
using BloodyEncounters.DB;
using ProjectM.UI;
using RootMotion.FinalIK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace BloodyEncounters.Systems
{
    internal class TimerSystem
    {

        public static int CurrentFrameCount = 0;
        public static ConcurrentQueue<Action> actionsToExecuteOnMainThread = new ConcurrentQueue<Action>();
        public static List<Timer> activeTimers = [];

        public static void OnGameFrame()
        {
            CurrentFrameCount++;

            while (actionsToExecuteOnMainThread.TryDequeue(out Action action))
            {
                action?.Invoke();
            }
        }

        public static Timer RunActionEveryInterval(Action action, double intervalInSeconds)
        {
            return new Timer(_ =>
            {
                actionsToExecuteOnMainThread.Enqueue(action);
            }, null, TimeSpan.FromSeconds(intervalInSeconds), TimeSpan.FromSeconds(intervalInSeconds));
        }

        public static Timer RunActionOnceAfterDelay(Action action, double delayInSeconds)
        {
            Timer timer = null;

            timer = new Timer(_ =>
            {
                // Enqueue the action to be executed on the main thread
                actionsToExecuteOnMainThread.Enqueue(() =>
                {
                    action.Invoke();  // Execute the action
                    timer?.Dispose(); // Dispose of the timer after the action is executed
                });
            }, null, TimeSpan.FromSeconds(delayInSeconds), Timeout.InfiniteTimeSpan); // Prevent periodic signaling

            return timer;
        }

        public static Timer RunActionOnceAfterFrames(Action action, int frameDelay)
        {
            int startFrame = CurrentFrameCount;
            Timer timer = null;

            timer = new Timer(_ =>
            {
                if (CurrentFrameCount - startFrame >= frameDelay)
                {
                    // Enqueue the action to be executed on the main thread
                    actionsToExecuteOnMainThread.Enqueue(() =>
                    {
                        action.Invoke();  // Execute the action
                    });
                    timer?.Dispose();
                }
            }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(8));

            return timer;
        }

        public static Timer RunActionAtTime(Action action, DateTime scheduledTime)
        {
            // Calculate the delay in milliseconds from now until the scheduled time
            var now = DateTime.Now;
            var delay = scheduledTime - now;

            // If the scheduled time is in the past, execute immediately or adjust according to your needs
            if (delay.TotalMilliseconds < 0)
            {
                return null;
            }
            else
            {
                return RunActionOnceAfterDelay(action, delay.TotalSeconds);
            }
        }


        public static void RunActionOnMainThread(Action action)
        {
            // Enqueue the action to be executed on the main thread
            actionsToExecuteOnMainThread.Enqueue(() =>
            {
                action.Invoke();  // Execute the action
            });
        }

        public static void StartEncounterTimer()
        {

            DateTime newEncounter = DateTime.Now.AddSeconds(120);
            Plugin.Logger.LogInfo($"Start Timner for encounters");
            Plugin.Logger.LogInfo($"Next match will be shot at {newEncounter:yyyy-MM-dd HH:mm:ss}");

            var action = () =>
            {
                //Plugin.Logger.LogInfo("Test Timner fo BloodyEncounters");
                if (PluginConfig.Enabled.Value)
                {
                    if(newEncounter <= DateTime.Now)
                    {
                        EncounterSystem.StartEncounter();
                        newEncounter = DateTime.Now.AddSeconds(new Random().Next(PluginConfig.EncounterTimerMin.Value, PluginConfig.EncounterTimerMax.Value));
                        Plugin.Logger.LogInfo($"Next match will be shot at {newEncounter:yyyy-MM-dd HH:mm:ss}");
                    }
                }
            };

            RunActionEveryInterval(action, 1);
        }
    }
}

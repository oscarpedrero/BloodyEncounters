using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using BepInEx.Configuration;
using Bloodstone.Hooks;
using static Il2CppSystem.Globalization.HebrewNumber;
using static ProjectM.UI.UISpawningSystem;
using static RootMotion.FinalIK.InteractionObject;
using Unity.Entities;

namespace BloodyEncounters.Configuration
{
    internal class PluginConfig
    {
        private static ConfigFile _mainConfig;

        public static ConfigEntry<bool> Enabled { get; private set; }
        public static ConfigEntry<bool> SkipPlayersInCastle { get; private set; }
        public static ConfigEntry<int> EncounterTimerMin { get; private set; }
        public static ConfigEntry<int> EncounterTimerMax { get; private set; }
        public static ConfigEntry<int> EncounterMinLevel { get; private set; }
        public static ConfigEntry<int> BuffForWorldBoss { get; private set; }
        public static ConfigEntry<string> EncounterMessageTemplate { get; private set; }
        public static ConfigEntry<string> RewardMessageTemplate { get; private set; }
        public static ConfigEntry<string> SpawnMessageBossTemplate { get; private set; }
        public static ConfigEntry<string> DespawnMessageBossTemplate { get; private set; }
        public static ConfigEntry<string> KillMessageBossTemplate { get; private set; }
        public static ConfigEntry<string> RewardAnnouncementMessageTemplate { get; private set; }
        public static ConfigEntry<string> VBloodFinalConcatCharacters { get; private set; }
        public static ConfigEntry<bool> NotifyAdminsAboutEncountersAndRewards { get; private set; }
        public static ConfigEntry<bool> NotifyAllPlayersAboutRewards { get; private set; }
        public static ConfigEntry<bool> SkipPlayersInCombat { get; private set; }



        public static void Initialize()
        {
            var bepInExConfigFolder = BepInEx.Paths.ConfigPath ?? Path.Combine("BepInEx", "config");
            var configFolder = Path.Combine(bepInExConfigFolder, "BloodyEncounters");
            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }
            var mainConfigFilePath = Path.Combine(configFolder, "BloodyEncounters.cfg");
            _mainConfig = File.Exists(mainConfigFilePath) ? new ConfigFile(mainConfigFilePath, false) : new ConfigFile(mainConfigFilePath, true);

            Enabled = _mainConfig.Bind("Main", "Enabled", true, "Determines whether the random encounter timer is enabled or not.");
            SkipPlayersInCastle = _mainConfig.Bind("Main", "SkipPlayersInCastle", true, "When enabled, players who are in a castle are excluded from encounters");
            SkipPlayersInCombat = _mainConfig.Bind("Main", "SkipPlayersInCombat", false, "When enabled, players who are in combat are excluded from the random encounters.");
            EncounterTimerMin = _mainConfig.Bind("Main", "EncounterTimerMin", 1200, "Minimum seconds before a new encounter is initiated. This value is divided by the online users count.");
            EncounterTimerMax = _mainConfig.Bind("Main", "EncounterTimerMax", 2400, "Maximum seconds before a new encounter is initiated. This value is divided by the online users count.");
            EncounterMinLevel = _mainConfig.Bind("Main", "EncounterMinLevel", 10, "The lower value for the Player level for encounter.");
            EncounterMessageTemplate = _mainConfig.Bind("Main", "EncounterMessageTemplate", "You have encountered a <color=#daa520>{0}</color>. You have <color=#daa520>{1}</color> seconds to kill it for a chance of a random reward.", "System message template for the encounter.");
            RewardMessageTemplate = _mainConfig.Bind("Main", "RewardMessageTemplate", "Congratulations. Your reward: <color={0}>{1}</color>.", "System message template for the reward.");
            RewardAnnouncementMessageTemplate = _mainConfig.Bind("Main", "RewardAnnouncementMessageTemplate", "{0} earned an encounter reward: <color={1}>{2}</color>.", "System message template for the reward announcement.");
            NotifyAdminsAboutEncountersAndRewards = _mainConfig.Bind("Main", "NotifyAdminsAboutEncountersAndRewards", true, "If enabled, all online admins are notified about encounters and rewards.");
            NotifyAllPlayersAboutRewards = _mainConfig.Bind("Main", "NotifyAllPlayersAboutRewards", false, "When enabled, all online players are notified about any player's rewards.");
            KillMessageBossTemplate = _mainConfig.Bind("Main", "KillMessageBossTemplate", "The World Boss has been defeated. Congratulations to #user# for beating #vblood#!", "The message that will appear globally once the boss gets killed.");
            SpawnMessageBossTemplate = _mainConfig.Bind("Main", "SpawnMessageBossTemplate", "A World Boss #worldbossname# has been summon you got #time# minutes to defeat it!. Use the command <color=#FBC01E>.encounter worldboss tp</color> to teleport to his position and start the fight!", "The message that will appear globally one the boss gets spawned.");
            DespawnMessageBossTemplate = _mainConfig.Bind("Main", "DespawnMessageBossTemplate", "You failed to kill the World Boss in time.", "The message that will appear globally if the players failed to kill the boss.");
            BuffForWorldBoss = _mainConfig.Bind("Main", "BuffForWorldBoss", 1163490655, "Buff that applies to each of the World Bosses that we create with our mod.");
            VBloodFinalConcatCharacters = _mainConfig.Bind("Main", "WorldBossFinalConcatCharacters", "and", "Final string for concat two or more players kill a WorldBoss Boss.");
        }
        public static void Destroy()
        {
            _mainConfig.Clear();
        }

        private static string CleanupName(string name)
        {
            var rgx = new Regex("[^a-zA-Z0-9 -]");
            return rgx.Replace(name, "");
        }

    }
}

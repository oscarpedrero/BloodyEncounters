using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bloodstone.API;
using BloodyEncounters.Commands;
using BloodyEncounters.Configuration;
using BloodyEncounters.DB;
using BloodyEncounters.DB.Models;
using ProjectM;
using Unity.Collections;
using VRising.GameData;

/**
 * 
 * Based in Code By syllabicat from VBloodKills (https://github.com/syllabicat/VBloodKills)
 * 
**/
namespace BloodyEncounters.Systems
{
    internal class WorldBossSystem
    {
        
        public static Dictionary<string, HashSet<string>> vbloodKills = new();
        private static DateTime lastDateMinute = DateTime.Now;
        private static DateTime lastDateSecond = DateTime.Now;
        
        public static void AddKiller(string vblood, string killerCharacterName)
        {
            if (!vbloodKills.ContainsKey(vblood))
            {
                vbloodKills[vblood] = new HashSet<string>();
            }
            vbloodKills[vblood].Add(killerCharacterName);
        }
        
        public static void RemoveKillers(string vblood)
        {
            vbloodKills[vblood] = new HashSet<string>();
        }
        
        public static List<string> GetKillers(string vblood)
        {
            return vbloodKills[vblood].ToList();
        }

        public static void SendAnnouncementMessage(string vblood, BossEncounterModel bossModel)
        {
            var message = GetAnnouncementMessage(vblood, bossModel.name);
            if (message != null)
            {
                var usersOnline = GameData.Users.Online;
                bossModel.DropItems(vblood);
                foreach (var user in usersOnline)
                {
                    ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, message);
                }
                RemoveKillers(vblood);
            }

        }

        public static string GetAnnouncementMessage(string vblood, string name)
        {
            var killers = GetKillers(vblood);
            var vbloodLabel = name;
            var sbKillersLabel = new StringBuilder();
            
            if (killers.Count == 0) return null;
            if (killers.Count == 1)
            {
                sbKillersLabel.Append(FontColorChatSystem.Yellow(killers[0]));
            }
            if (killers.Count == 2)
            {
                sbKillersLabel.Append($"{FontColorChatSystem.Yellow(killers[0])} {PluginConfig.VBloodFinalConcatCharacters.Value} {FontColorChatSystem.Yellow(killers[1])}");
            }
            if (killers.Count > 2)
            {
                for (int i = 0; i < killers.Count; i++)
                {
                    if (i == killers.Count - 1)
                    {
                        sbKillersLabel.Append($"{PluginConfig.VBloodFinalConcatCharacters.Value} {FontColorChatSystem.Yellow(killers[i])}");
                    }
                    else
                    {
                        sbKillersLabel.Append($"{FontColorChatSystem.Yellow(killers[i])}, ");
                    }
                }
            }

            var _message = PluginConfig.KillMessageBossTemplate.Value;
            _message = _message.Replace("#user#", $"{sbKillersLabel}");
            _message = _message.Replace("#vblood#", $"{FontColorChatSystem.Red(vbloodLabel)}");
            return FontColorChatSystem.Green($"{_message}");
        }

        public static void GameFrameUpdate()
        {
            var date = DateTime.Now;
            if(lastDateMinute.ToString("HH:mm") != date.ToString("HH:mm"))
            {
                lastDateMinute = date;
                var spawnsBoss = Database.WORLDBOSS.Where(x => x.Hour == date.ToString("HH:mm")).ToList();
                if(spawnsBoss != null && GameData.Users.Online.Count() > 0)
                {
                    var user = GameData.Users.Online.FirstOrDefault();
                    foreach(var spawnBoss in spawnsBoss)
                    {
                        spawnBoss.Spawn(user.Entity);
                        WorldBossCommand._lastBossSpawnModel = spawnBoss;
                    }
                    
                }
            }
            if (lastDateSecond.ToString("HH:mm:ss") != date.ToString("HH:mm:ss"))
            {
                lastDateSecond = date;
                var despawnsBoss = Database.WORLDBOSS.Where(x => x.HourDespawn == date.ToString("HH:mm:ss") && x.bossEntity != null).ToList();
                if (despawnsBoss != null && GameData.Users.Online.Count() > 0)
                {
                    foreach (var spawnBoss in despawnsBoss)
                    {
                        var _message = PluginConfig.DespawnMessageBossTemplate.Value;
                        _message = _message.Replace("#worldbossname#", FontColorChatSystem.Yellow($"{WorldBossCommand._lastBossSpawnModel.name}"));

                        ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChatSystem.Green($"{_message}"));
                        WorldBossCommand._lastBossSpawnModel = null;
                    }

                }
            }

        }
    }
}

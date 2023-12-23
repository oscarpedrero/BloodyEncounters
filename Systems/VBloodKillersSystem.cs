using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bloodstone.API;
using BloodyEncounters.Configuration;
using BloodyEncounters.DB.Models;
using ProjectM;
using VRising.GameData;

/**
 * 
 * Based in Code By syllabicat from VBloodKills (https://github.com/syllabicat/VBloodKills)
 * 
**/
namespace BloodyEncounters.Systems
{
    internal class VBloodKillersSystem
    {
        
        public static Dictionary<string, HashSet<string>> vbloodKills = new();

        
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
    }
}

using ProjectM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Bloody.Core.Models;
using Bloody.Core;
using System.Collections.Concurrent;
using Bloody.Core.Methods;
using BloodyEncounters.DB;
using BloodyEncounters.DB.Models;
using BloodyEncounters.Configuration;
using BloodyEncounters.Utils;
using Stunlock.Core;
using Bloodstone.API;
using Bloody.Core.API;
using UnityEngine.Rendering.HighDefinition;

namespace BloodyEncounters.Systems
{
    internal class EncounterSystem
    {
        private static readonly ConcurrentDictionary<ulong, ConcurrentDictionary<int, ItemEncounterModel>> RewardsMap = new();

        private static readonly ConcurrentDictionary<int, UserModel> NpcPlayerMap = new();

        private static PrefabCollectionSystem _prefabCollectionSystem = Plugin.SystemsCore.PrefabCollectionSystem;

        internal static Dictionary<long, (float actualDuration, Action<Entity> Actions)> PostActions = new();

        public static System.Random Random = new System.Random();

        public static bool EncounterStarted = false;

        private static string MessageTemplate => PluginConfig.EncounterMessageTemplate.Value;

        internal static void Initialize()
        {
            EventsHandlerSystem.OnDeath += ServerEvents_OnDeath;
        }

        internal static void Destroy()
        {
            EventsHandlerSystem.OnDeath -= ServerEvents_OnDeath;
        }

        internal static void StartEncounter(UserModel user = null)
        {
            if (!EncounterStarted)
            {
                var world = Core.World;

                if (user == null)
                {
                    var users = Core.Users.Online.Where(u => Core.Users.FromEntity(u.Entity).Character.Equipment.Level >= PluginConfig.EncounterMinLevel.Value);

                    if (PluginConfig.SkipPlayersInCastle.Value)
                    {
                        users = users.Where(u => !u.IsInCastle());
                    }

                    if (PluginConfig.SkipPlayersInCombat.Value)
                    {
                        users = users.Where(u => !u.IsInCombat());
                    }

                    user = users.OrderBy(_ => Random.Next()).FirstOrDefault();
                }

                if (user == null)
                {
                    Plugin.Logger.LogMessage("Could not find any eligible players for a random encounter...");
                    return;
                }

                var npc = DataFactory.GetRandomNpc();

                if (npc == null)
                {
                    Plugin.Logger.LogWarning($"Could not find any NPCs");
                    return;
                }
                Plugin.Logger.LogMessage($"Attempting to start a new encounter for {user.CharacterName} with {npc.name}");
                try
                {

                    NpcPlayerMap[npc.PrefabGUID] = user;
                    npc.SpawnWithLocation(user.Entity, user.Position);
                    EncounterStarted = true;

                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogError(ex);
                    EncounterStarted = false;
                    // Suppress
                }
            }
            
        }

        public static void ServerEvents_OnDeath(DeathEventListenerSystem sender, NativeArray<DeathEvent> deathEvents)
        {
            foreach (var deathEvent in deathEvents)
            {
                if (!sender.EntityManager.HasComponent<PlayerCharacter>(deathEvent.Killer))
                {
                    continue;
                }

                var playerCharacter = sender.EntityManager.GetComponentData<PlayerCharacter>(deathEvent.Killer);
                var userModel = Core.Users.FromEntity(playerCharacter.UserEntity);
                var npcGUID = deathEvent.Died.Read<PrefabGUID>();
                var npc = _prefabCollectionSystem._PrefabDataLookup[npcGUID].AssetName;

                var modelNpc = Database.NPCS.Where(x => x.AssetName == npc.ToString()).FirstOrDefault();

                if (modelNpc == null)
                {
                    EncounterStarted = false;
                    continue;
                }
                var modelItem = DataFactory.GetRandomItem(modelNpc);


                var itemGuid = new PrefabGUID(modelItem.ItemID);
                var quantity = modelItem.Stack;
                if (!userModel.TryGiveItem(itemGuid, quantity, out _))
                {
                    Plugin.Logger.LogError($"{userModel.CharacterName} Error Drop {modelItem.name}");
                }
                var message = string.Format(PluginConfig.RewardMessageTemplate.Value, modelItem.Color, modelItem.name);
                userModel.SendSystemMessage(message);
                Plugin.Logger.LogDebug($"{userModel.CharacterName} earned reward: {modelItem.name}");
                var globalMessage = string.Format(PluginConfig.RewardAnnouncementMessageTemplate.Value, userModel.CharacterName, modelItem.Color, modelItem.name);
                if (PluginConfig.NotifyAllPlayersAboutRewards.Value)
                {
                    var onlineUsers = Core.Users.Online;
                    foreach (var model in onlineUsers.Where(u => u.PlatformId != userModel.PlatformId))
                    {
                        model.SendSystemMessage(globalMessage);
                    }

                }
                else if (PluginConfig.NotifyAdminsAboutEncountersAndRewards.Value)
                {
                    var onlineAdmins = Core.Users.Online.Where(x => x.IsAdmin == true);
                    foreach (var onlineAdmin in onlineAdmins)
                    {
                        onlineAdmin.SendSystemMessage($"{userModel.CharacterName} earned an encounter reward: <color={modelItem.Color}>{modelItem.name}</color>");
                    }
                }
                    
                    
                

                EncounterStarted = false;


            }
        }

        private static bool probabilityGeneratingReward(int percentage)
        {
            var number = new System.Random().Next(1, 100);

            if (number <= (percentage * 100))
            {
                return true;
            }

            return false;
        }

        internal static void OnDeath(DeathEventListenerSystem sender, NativeArray<DeathEvent> deathEvents)
        {
            throw new NotImplementedException();
        }
    }
}

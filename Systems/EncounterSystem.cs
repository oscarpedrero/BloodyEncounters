using ProjectM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using VRising.GameData.Models;
using VRising.GameData;
using System.Collections.Concurrent;
using VRising.GameData.Methods;
using BloodyEncounters.DB;
using BloodyEncounters.DB.Models;
using BloodyEncounters.Configuration;
using BloodyEncounters.Utils;
using BloodyEncounters.Services;
using BloodyEncounters.Patch;

namespace BloodyEncounters.Systems
{
    internal class EncounterSystem
    {
        private static readonly ConcurrentDictionary<ulong, ConcurrentDictionary<int, ItemEncounterModel>> RewardsMap = new();

        private static readonly ConcurrentDictionary<int, UserModel> NpcPlayerMap = new();

        internal static Dictionary<long, (float actualDuration, Action<Entity> Actions)> PostActions = new();

        public static System.Random Random = new System.Random();

        private static string MessageTemplate => PluginConfig.EncounterMessageTemplate.Value;

        internal static void Initialize()
        {
            ServerEvents.OnDeath += ServerEvents_OnDeath;
            ServerEvents.OnUnitSpawned += ServerEvents_OnUnitSpawned;
        }

        internal static void Destroy()
        {
            ServerEvents.OnDeath -= ServerEvents_OnDeath;
            ServerEvents.OnUnitSpawned -= ServerEvents_OnUnitSpawned;
        }

        internal static void StartEncounter(UserModel user = null)
        {
            var world = Plugin.World;

            if (user == null)
            {
                var users = GameData.Users.Online.Where(u => GameData.Users.FromEntity(u.Entity).Character.Equipment.Level >= PluginConfig.EncounterMinLevel.Value);

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

            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError(ex);
                // Suppress
            }
        }

        public static void ServerEvents_OnUnitSpawned(World world, Entity entity)
        {


            var entityManager = world.EntityManager;
            if (!entityManager.HasComponent<PrefabGUID>(entity))
            {
                return;
            }
  
            var prefabGuid = entityManager.GetComponentData<PrefabGUID>(entity);
            if (!NpcPlayerMap.TryGetValue(prefabGuid.GuidHash, out var user))
            {
                return;
            }
            if (!entityManager.HasComponent<LifeTime>(entity))
            {
                return;
            }
          
            if (!Database.GetNPCFromEntity(entity, out NpcEncounterModel npc))
            {
                return;
            }
           
            var lifeTime = entityManager.GetComponentData<LifeTime>(entity);
            if (Math.Abs(lifeTime.Duration - npc.Lifetime) > 0.001)
            {
                return;
            }
      
            NpcPlayerMap.TryRemove(prefabGuid.GuidHash, out _);
            
            if (!RewardsMap.ContainsKey(user.PlatformId))
            {
                RewardsMap[user.PlatformId] = new ConcurrentDictionary<int, ItemEncounterModel>();
            }
          
            var message =
                string.Format(
                    MessageTemplate,
                    npc.name, npc.Lifetime);

            user.SendSystemMessage(message);
            Plugin.Logger.LogDebug($"Encounters started: {user.CharacterName} vs. {npc.name}");
  
            if (PluginConfig.NotifyAdminsAboutEncountersAndRewards.Value)
            {
                var onlineAdmins = GameData.Users.Online.Where(x => x.IsAdmin == true);
                foreach (var onlineAdmin in onlineAdmins)
                {
                    onlineAdmin.SendSystemMessage($"Encounter started: {user.CharacterName} vs. {npc.name}");
                }
            }
            
            RewardsMap[user.PlatformId][entity.Index] = DataFactory.GetRandomItem(npc);
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
                var userModel = GameData.Users.FromEntity(playerCharacter.UserEntity);


                if (RewardsMap.TryGetValue(userModel.PlatformId, out var bounties) &&
                    bounties.TryGetValue(deathEvent.Died.Index, out var itemModel))
                {
                    var itemGuid = new PrefabGUID(itemModel.ItemID);
                    var quantity = itemModel.Stack;
                    if (!userModel.TryGiveItem(new PrefabGUID(itemModel.ItemID), quantity, out _))
                    {
                        userModel.DropItemNearby(itemGuid, quantity);
                    }
                    var message = string.Format(PluginConfig.RewardMessageTemplate.Value, itemModel.Color, itemModel.name);
                    userModel.SendSystemMessage(message);
                    bounties.TryRemove(deathEvent.Died.Index, out _);
                    Plugin.Logger.LogDebug($"{userModel.CharacterName} earned reward: {itemModel.name}");
                    var globalMessage = string.Format(PluginConfig.RewardAnnouncementMessageTemplate.Value,
                        userModel.CharacterName, itemModel.Color, itemModel.name);
                    if (PluginConfig.NotifyAllPlayersAboutRewards.Value)
                    {
                        var onlineUsers = GameData.Users.Online;
                        foreach (var model in onlineUsers.Where(u => u.PlatformId != userModel.PlatformId))
                        {
                            model.SendSystemMessage(globalMessage);
                        }

                    }
                    else if (PluginConfig.NotifyAdminsAboutEncountersAndRewards.Value)
                    {
                        var onlineAdmins = GameData.Users.Online.Where(x => x.IsAdmin == true);
                        foreach (var onlineAdmin in onlineAdmins)
                        {
                            onlineAdmin.SendSystemMessage($"{userModel.CharacterName} earned an encounter reward: <color={itemModel.Color}>{itemModel.name}</color>");
                        }
                    }
                }
            }
        }
    }
}

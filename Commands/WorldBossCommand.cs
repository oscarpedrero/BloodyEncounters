using Bloodstone.API;
using BloodyEncounters.Configuration;
using BloodyEncounters.DB;
using BloodyEncounters.DB.Models;
using BloodyEncounters.Exceptions;
using BloodyEncounters.Systems;
using ProjectM;
using System;
using Unity.Mathematics;
using VampireCommandFramework;
using VRising.GameData;
using VRising.GameData.Methods;
using static RootMotion.FinalIK.InteractionObject;

namespace BloodyEncounters.Commands
{
    [CommandGroup("encounter worldboss")]
    internal class WorldBossCommand
    {
        public static BossEncounterModel _lastBossSpawnModel = null;

        [Command("list", usage: "", description: "List of World Boss", adminOnly: true)]
        public void ListMerchant(ChatCommandContext ctx)
        {

            var worldboss = Database.WORLDBOSS;

            if (worldboss.Count == 0)
            {
                throw ctx.Error($"There are no world boss created");
            }
            ctx.Reply($"WORLDBOSS List");
            ctx.Reply($"----------------------------");
            ctx.Reply($"--");
            foreach (var boss in worldboss)
            {
                ctx.Reply($"World Boss {boss.name}");
                ctx.Reply($"--");
            }
            ctx.Reply($"----------------------------");
        }

        // .encounter worldboss create Test -1391546313 200 2 60 
        // .encounter worldboss items add Test ItemName -257494203 20 1
        // .encounter worldboss set location Test
        // .encounter worldboss start Test
        // 
        [Command("create", usage: "<NameOfBOSS> <PrefabGUIDOfBOSS> <Level> <Multiplier> <LifeTimeSeconds>", description: "Create a World Boss", adminOnly: true)]
        public void CreateBOSS(ChatCommandContext ctx, string bossName, int prefabGUID, int level, int multiplier, int lifeTime)
        {
            try
            {
                if (Database.AddBoss(bossName, prefabGUID, level, multiplier, lifeTime))
                {
                    ctx.Reply($"World Boss '{bossName}' created successfully");
                }
            }
            catch (NPCExistException)
            {
                throw ctx.Error($"World Boss with name '{bossName}' exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

        }

        [Command("remove", usage: "", description: "Remove a World Boss", adminOnly: true)]
        public void RemoveMerchant(ChatCommandContext ctx, string bossName)
        {

            try
            {
                if (Database.RemoveBoss(bossName))
                {
                    ctx.Reply($"World Boss '{bossName}' remove successfully");
                }
            }
            catch (NPCDontExistException)
            {
                throw ctx.Error($"World Boss with name '{bossName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

        }

        [Command("set location", usage: "<NameOfWorldBoss>", description: "Adds the current location of the player who sets it to the world boss.", adminOnly: true)]
        public void SetLocation(ChatCommandContext ctx, string WorldBossName)
        {
            try
            {
                var user = GameData.Users.GetUserByCharacterName(ctx.User.CharacterName.ToString());
                if (Database.GetBoss(WorldBossName, out BossEncounterModel worldBoss))
                {
                    worldBoss.SetLocation(user.Position);
                    ctx.Reply($"Position {user.Position.x},{user.Position.y},{user.Position.z} successfully set to WorldBoss '{WorldBossName}'");
                }
                else
                {
                    throw new WorldBossDontExistException();
                }
            }
            catch (WorldBossDontExistException)
            {
                throw ctx.Error($"WorldBoss with name '{WorldBossName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }


        }

        [Command("start", usage: "<NameOfWorldBoss>", description: "The confrontation with a World boss begins.", adminOnly: true)]
        public void start(ChatCommandContext ctx, string WorldBossName)
        {
            try
            {
                var user = GameData.Users.GetUserByCharacterName(ctx.User.CharacterName.ToString());
                if (Database.GetBoss(WorldBossName, out BossEncounterModel worldBoss))
                {
                    _lastBossSpawnModel = worldBoss;
                    worldBoss.Spawn(user.Entity);
                    var _message = PluginConfig.SpawnMessageBossTemplate.Value;
                    _message = _message.Replace("#time#", FontColorChatSystem.Yellow($"{worldBoss.Lifetime / 60}"));
                    _message = _message.Replace("#worldbossname#", FontColorChatSystem.Yellow($"{worldBoss.name}"));
                    
                    ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChatSystem.Green($"{_message}"));
                }
                else
                {
                    throw new WorldBossDontExistException();
                }
            }
            catch (WorldBossDontExistException)
            {
                throw ctx.Error($"WorldBoss with name '{WorldBossName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }


        }

        [Command("tp", usage: "", description: "Teleport to the encounter with world boss.", adminOnly: false)]
        public void teleportToWorldBoss(ChatCommandContext ctx)
        {
            try
            {
                    if(_lastBossSpawnModel == null)
                    {
                        throw ctx.Error($"There is no active encounter at this time.");
                    }

                    var user = GameData.Users.GetUserByCharacterName(ctx.User.CharacterName.ToString());

                    float3 position = new float3(_lastBossSpawnModel.x, _lastBossSpawnModel.y, _lastBossSpawnModel.z); 
                    
                    user.TeleportTo(position);

            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }
        }
    }
}

using Bloodstone.API;
using BloodyEncounters.Configuration;
using BloodyEncounters.Exceptions;
using BloodyEncounters.Services;
using BloodyEncounters.Systems;
using ProjectM;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using VRising.GameData;
using VRising.GameData.Methods;
using VRising.GameData.Models;

namespace BloodyEncounters.DB.Models
{
    internal class BossEncounterModel
    {
        public string name { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public string Hour { get; set; } = string.Empty;
        public string HourDespawn { get; set; } = string.Empty;
        public int PrefabGUID { get; set; }
        public int level { get; set; }
        public int multiplier { get; set; }
        public List<ItemEncounterModel> items { get; set; } = new();
        public Entity? bossEntity { get; set; } = null;
        public NpcModel npcModel { get; set; }
        public float Lifetime { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public List<ItemEncounterModel> GetItems()
        {
            return items;
        }

        public bool GetItem(int itemPrefabID, out ItemEncounterModel item)
        {
            item = items.Where(x => x.ItemID == itemPrefabID).FirstOrDefault();
            if (item == null)
            {
                return false;
            }
            return true;
        }

        public bool GetItemFromName(string ItemName, out ItemEncounterModel item)
        {
            item = items.Where(x => x.name == ItemName).FirstOrDefault();
            if (item == null)
            {
                return false;
            }
            return true;
        }

        public bool AddItem(string ItemName, int ItemPrefabID, int Stack, int Chance = 1)
        {
            if (!GetItem(ItemPrefabID, out ItemEncounterModel item))
            {
                item = new ItemEncounterModel();
                item.name = ItemName;
                item.ItemID = ItemPrefabID;
                item.Stack = Stack;
                item.Chance = Chance;
                items.Add(item);
                Database.saveDatabase();
                return true;
            }

            throw new ProductExistException();

        }

        public bool RemoveItem(string ItemName)
        {
            if (GetItemFromName(ItemName, out ItemEncounterModel item))
            {
                items.Remove(item);
                Database.saveDatabase();
                return true;
            }

            throw new ProductDontExistException();
        }

        public bool SpawnWithLocation(Entity sender, float3 pos)
        {

            UnitSpawnerService.UnitSpawner.SpawnWithCallback(sender, new PrefabGUID(PrefabGUID), new(pos.x, pos.z), Lifetime, (Entity e) => {
                npcModel = GameData.Npcs.FromEntity(e);
                ModifyBoss(sender, e);
            });
            return true;
        }

        public bool Spawn(Entity sender)
        {
            UnitSpawnerService.UnitSpawner.SpawnWithCallback(sender, new PrefabGUID(PrefabGUID), new(x, z), Lifetime, (Entity e) => {
                npcModel = GameData.Npcs.FromEntity(e);
                ModifyBoss(sender, e);
            });
            var _message = PluginConfig.SpawnMessageBossTemplate.Value;
            _message = _message.Replace("#time#", FontColorChatSystem.Yellow($"{Lifetime / 60}"));
            _message = _message.Replace("#worldbossname#", FontColorChatSystem.Yellow($"{name}"));

            ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChatSystem.Green($"{_message}"));
            return true;
        }

        public bool DropItems(string vblood)
        {
            foreach (var item in items)
            {
                if (probabilityGeneratingReward(item.Chance))
                {
                    var users = WorldBossSystem.GetKillers(vblood);
                    foreach (var user in users)
                    {
                        var itemGuid = new PrefabGUID(item.ItemID);
                        var quantity = item.Stack;
                        var userModel = GameData.Users.GetUserByCharacterName(user);
                        if (!userModel.TryGiveItem(itemGuid, quantity, out _))
                        {
                            userModel.DropItemNearby(itemGuid, quantity);
                        }
                    }
                }
            }
            bossEntity = null;
            HourDespawn = DateTime.Parse(Hour).AddSeconds(Lifetime).ToString("HH:mm:ss");
            return true;
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

        public void ModifyBoss(Entity user, Entity boss)
        {
            var players = GameData.Users.Online.Count();

            var unit = boss.Read<UnitLevel>();
            unit.Level = level;
            boss.Write(unit);

            var health = boss.Read<Health>();
            health.MaxHealth.Value = (health.MaxHealth * (players * multiplier));
            health.Value = health.MaxHealth.Value;
            boss.Write(health);

            if(BuffSystem.BuffBoss(boss, user, new PrefabGUID(PluginConfig.BuffForWorldBoss.Value), 0))
            {
                //Plugin.Logger.LogInfo($"BUFF OK {PluginConfig.BuffForWorldBoss.Value}");
            } else
            {
                //Plugin.Logger.LogInfo($"BUFF KO {PluginConfig.BuffForWorldBoss.Value}");
            }

            // TODO: Under Investigate
            RenameBoss(boss, name);

            bossEntity = boss;

        }

        public void SetLocation(float3 position)
        {
            
            x = position.x;
            z = position.z;
            y = position.y;

            Database.saveDatabase();

        }

        private static void RenameBoss(Entity merchant, string nameMerchant)
        {
            /*var nameable = new NameableInteractableComponent();
            nameable.name = nameMerchant;
            Plugin.World.EntityManager.SetComponentData(merchant, nameable);
            merchant.WithComponentData((ref NameableInteractable nameable) =>
            {
                nameable.Name = nameMerchant;
                return;
            });*/

            //var nameable = merchant.Read<Name>;
        }

        internal void SetAssetName(string v)
        {
            AssetName = v;
            Database.saveDatabase();
        }

        internal void SetHour(string hour)
        {
            Hour = hour;
            HourDespawn = DateTime.Parse(hour).AddSeconds(Lifetime).ToString("HH:mm:ss");
            Database.saveDatabase();
        }

        internal void SetHourDespawn()
        {
            HourDespawn = DateTime.Now.AddSeconds(Lifetime).ToString("HH:mm:ss");
            Database.saveDatabase();
        }
    }
}

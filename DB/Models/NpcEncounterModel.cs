using BloodyEncounters.Exceptions;
using BloodyEncounters.Services;
using ProjectM;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using VRising.GameData;
using VRising.GameData.Models;

namespace BloodyEncounters.DB.Models
{
    internal class NpcEncounterModel
    {
        public string name { get; set; } = string.Empty;
        public int PrefabGUID { get; set; }
        public int levelAbove { get; set; }
        public List<ItemEncounterModel> items { get; set; } = new();
        public Entity npcEntity { get; set; } = new();
        public NpcModel npcModel { get; set; }
        public float Lifetime { get; set; }

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

        public bool AddItem(string ItemName, int ItemPrefabID, int Stack)
        {
            if (!GetItem(ItemPrefabID, out ItemEncounterModel item))
            {
                item = new ItemEncounterModel();
                item.name = ItemName;
                item.ItemID = ItemPrefabID;
                item.Stack = Stack;
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
                npcEntity = e;
                npcModel = GameData.Npcs.FromEntity(npcEntity);
                ModifyNPC(sender, e);
            });
            return true;
        }

        public void ModifyNPC(Entity user, Entity npc)
        {
            var playertLevel = GameData.Users.FromEntity(user).Character.Equipment.Level;

            var unitLevel = Plugin.World.EntityManager.GetComponentData<UnitLevel>(npc);
            unitLevel.Level = (int)(playertLevel + levelAbove);

            Plugin.World.EntityManager.SetComponentData(npc, unitLevel);

            // TODO: Under Investigate
            RenameNPCt(npc, name);

        }

        private static void RenameNPCt(Entity merchant, string nameMerchant)
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

    }
}

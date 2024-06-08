using BloodyEncounters.Exceptions;
using ProjectM;
using Stunlock.Core;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Bloody.Core.Models.v1;
using Bloody.Core;
using Bloody.Core.API;
using Unity.Collections;
using Bloody.Core.Helper.v1;
using Bloody.Core.API.v1;
using Bloody.Core.GameData.v1;
using ProjectM.Shared;

namespace BloodyEncounters.DB.Models
{
    internal class NpcEncounterModel
    {
        internal string nameHash;

        public string name { get; set; } = string.Empty;
        public int PrefabGUID { get; set; }
        public string AssetName { get; set; } = string.Empty;
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

            SpawnSystem.SpawnUnitWithCallback(sender, new PrefabGUID(PrefabGUID), new(pos.x, pos.z), Lifetime, (Entity e) => {
                npcEntity = e;
                if(npcEntity.Has<VBloodUnit>())
                {
                    var actionVblood = () =>
                    {
                        var vbloodunit = npcEntity.Read<VBloodUnit>();
                        vbloodunit.CanBeTracked = false;
                        vbloodunit.UnlocksTrophyOnFeed = Trophy.None;
                        vbloodunit.UnlocksTrophyOnFeedBrutal = Trophy.None;
                        npcEntity.Write(vbloodunit);

                        var vBloodUnlockTechBuffer = npcEntity.ReadBuffer<VBloodUnlockTechBuffer>();
                        vBloodUnlockTechBuffer.Clear();
                    };

                    CoroutineHandler.StartFrameCoroutine(actionVblood, 3, 1);
                    
                }

                var dropTableBuffer = npcEntity.ReadBuffer<DropTableBuffer>();
                dropTableBuffer.Clear();


                npcModel = GameData.Npcs.FromEntity(npcEntity);
                ModifyNPC(sender, e);
            });
            return true;
        }

        public void ModifyNPC(Entity user, Entity npc)
        {
            var playertLevel = GameData.Users.FromEntity(user).Character.Equipment.Level;

            var unitLevel = Core.World.EntityManager.GetComponentData<UnitLevel>(npc);
            int level = (int)(playertLevel + levelAbove);
            unitLevel.Level = new ModifiableInt(level);

            Core.World.EntityManager.SetComponentData(npc, unitLevel);

            RenameNPC(npc);

        }

        private void RenameNPC(Entity boss)
        {
            boss.Add<NameableInteractable>();
            NameableInteractable _nameableInteractable = boss.Read<NameableInteractable>();
            _nameableInteractable.Name = new FixedString64Bytes(nameHash + "be");
            boss.Write(_nameableInteractable);
        }

        public bool GetNPCEntity()
        {
            var entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable, LifeTime>(EntityQueryOptions.IncludeDisabledEntities);
            foreach (var entity in entities)
            {
                NameableInteractable _nameableInteractable = entity.Read<NameableInteractable>();
                if (_nameableInteractable.Name.Value == nameHash + "be")
                {
                    npcEntity = entity;
                    entities.Dispose();
                    return true;
                }
            }
            entities.Dispose();
            return false;
        }

    }
}

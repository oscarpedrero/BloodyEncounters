using BloodyEncounters.DB.Models;
using BloodyEncounters.DB;
using ProjectM;
using System.Linq;
using VampireCommandFramework;
using BloodyEncounters.Exceptions;
using System;

namespace BloodyEncounters.Commands
{

    [CommandGroup("encounter worldboss items")]
    internal class ItemsWorldBossommand
    {

        [Command("list", usage: "<NameOfWorldBoss>", description: "List items of WorldBoss drop", adminOnly: true)]
        public void ListWorldBossItems(ChatCommandContext ctx, string WorldBossName)
        {

            try
            {
                if (Database.GetBoss(WorldBossName, out BossEncounterModel boss))
                {
                    ctx.Reply($"{boss.name} Items List");
                    ctx.Reply($"----------------------------");
                    ctx.Reply($"--");
                    foreach (var item in boss.GetItems())
                    {
                        ctx.Reply($"Item {item.ItemID}");
                        ctx.Reply($"Stack {item.Stack}");
                        ctx.Reply($"Stack {item.Chance}");
                        ctx.Reply($"--");
                    }
                    ctx.Reply($"----------------------------");
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
            catch (ProductExistException)
            {
                throw ctx.Error($"This item configuration already exists at merchant '{WorldBossName}'");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }
            
        }

        // .encounter product add Test1 736318803 20
        [Command("add", usage: "<NameOfWorldBoss> <ItemName> <ItemPrefabID> <Stack> <Chance>", description: "Add a item to a WorldBoss drop. Chance is number between 0 to 1, Example 0.5 for 50% of drop", adminOnly: true)]
        public void CreateItem(ChatCommandContext ctx, string WorldBossName, string ItemName, int ItemPrefabID, int Stack, int Chance)
        {
            try
            {
                if(Database.GetBoss(WorldBossName, out BossEncounterModel worldBoss))
                {
                    worldBoss.AddItem(ItemName,ItemPrefabID, Stack, Chance);
                    ctx.Reply($"Item successfully added to WorldBoss '{WorldBossName}'");
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
            catch (ProductExistException)
            {
                throw ctx.Error($"This item configuration already exists at merchant '{WorldBossName}'");
            } 
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

            
        }

        // .encounter product remove Test1 736318803
        [Command("remove", usage: "<NameOfWorldBoss> <ItemName>", description: "Remove a item to a WorldBoss", adminOnly: true)]
        public void RemoveProduct(ChatCommandContext ctx, string WorldBossName, string ItemName)
        {
            try
            {
                if (Database.GetBoss(WorldBossName, out BossEncounterModel npc))
                {
                    npc.RemoveItem(ItemName);
                    ctx.Reply($"WorldBoss '{WorldBossName}'\'s item has been successfully removed");
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
            catch (ProductDontExistException)
            {
                throw ctx.Error($"This item does not exist at WorldBoss '{WorldBossName}'");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

            
        }
    }
}

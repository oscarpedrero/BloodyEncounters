using BloodyEncounters.DB;
using BloodyEncounters.Exceptions;
using System;
using VampireCommandFramework;

namespace BloodyEncounters.Commands
{
    [CommandGroup("encounter npc")]
    internal class NpcCommand
    {

        [Command("list", usage: "", description: "List of merchant", adminOnly: true)]
        public void ListMerchant(ChatCommandContext ctx)
        {

            var npcs = Database.NPCS;

            if (npcs.Count == 0)
            {
                throw ctx.Error($"There are no npcs created");
            }
            ctx.Reply($"NPCS List");
            ctx.Reply($"----------------------------");
            ctx.Reply($"--");
            foreach (var npc in npcs)
            {
                ctx.Reply($"Merchant {npc.name}");
                ctx.Reply($"--");
            }
            ctx.Reply($"----------------------------");
        }

        [Command("create", usage: "<NameOfNPC> <PrefabGUIDOfNPC> <LevelsAbovePlayer> <LifeTime>", description: "Create a NPC", adminOnly: true)]
        public void CreateNPC(ChatCommandContext ctx, string npcName, int prefabGUID, int levelAbove, int lifeTime)
        {
            try
            {
                if (Database.AddNPC(npcName, prefabGUID, levelAbove, lifeTime))
                {
                    ctx.Reply($"NPC '{npcName}' created successfully");
                }
            }
            catch (NPCExistException)
            {
                throw ctx.Error($"NPC with name '{npcName}' exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

        }

        [Command("remove", usage: "<NameOfNPC>", description: "Remove a NPC", adminOnly: true)]
        public void RemoveMerchant(ChatCommandContext ctx, string npcName)
        {

            try
            {
                if (Database.RemoveNPC(npcName))
                {
                    ctx.Reply($"NPC '{npcName}' remove successfully");
                }
            }
            catch (NPCDontExistException)
            {
                throw ctx.Error($"NPC with name '{npcName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

        }
    }
}

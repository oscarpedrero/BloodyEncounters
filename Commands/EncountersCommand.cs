using BloodyEncounters.Configuration;
using BloodyEncounters.Systems;
using VampireCommandFramework;
using VRising.GameData;

namespace BloodyEncounters.Commands
{

    [CommandGroup("encounter")]
    internal class EncountersCommand
    {

        [Command("start", usage: "", description: "Starts an encounter for a random online user.", adminOnly: true)]
        public static void StartCommand(ChatCommandContext ctx)
        {
            EncounterSystem.StartEncounter();
            ctx.Reply("The hunt has begun...");
            return;
        }

        [Command("me", usage: "", description: "Starts an encounter for the admin who sends the command.", adminOnly: true)]
        public static void MeCommand(ChatCommandContext ctx)
        {
            var senderModel = GameData.Users.FromEntity(ctx.Event.SenderUserEntity);
            EncounterSystem.StartEncounter(senderModel);
            ctx.Reply("Prepare for the fight...");
            return;
        }

        [Command("player", usage: "<PlayerName>", description: "Starts an encounter for the given player, for example.", adminOnly: true)]
        public static void PlayerCommand(ChatCommandContext ctx, string PlayerName)
        {
            var senderModel = GameData.Users.GetUserByCharacterName(PlayerName);
            if (senderModel == null)
            {
                throw ctx.Error($"Player not found");
            }
            if (!senderModel.IsConnected)
            {
                throw ctx.Error($"Could not find an online player with name {PlayerName}");
            }
            EncounterSystem.StartEncounter(senderModel);
            ctx.Reply($"Sending an ambush to {PlayerName}.");
        }

        [Command("enable", usage: "", description: "Enables the random encounter timer.", adminOnly: true)]
        public static void EnableCommand(ChatCommandContext ctx)
        {
            if (PluginConfig.Enabled.Value)
            {
                throw ctx.Error("Already enabled.");
            }
            PluginConfig.Enabled.Value = true;
            TimerSystem.StartEncounterTimer();
            ctx.Reply($"Enabled");
        }

        [Command("disable", usage: "", description: "Disables the random encounter timer.", adminOnly: true)]
        public static void DisableCommand(ChatCommandContext ctx)
        {
            if (!PluginConfig.Enabled.Value)
            {
                throw ctx.Error("Already disabled.");
            }
            PluginConfig.Enabled.Value = false;
            TimerSystem._encounterTimer.Stop();
            ctx.Reply("Disabled.");
        }
    }
}

namespace CavRn.Stargate
{
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Systems.Messaging.Chat.Commands;
    using Eco.Shared.Math;

    [ChatCommandHandler]
    public static class StargateCommands
    {
        [ChatCommand("Shows commands for Stargate manipulation.")]
        public static void Stargate(User user) { }

        [ChatSubCommand("Stargate", "SpawnStargate", ChatAuthorizationLevel.Admin)]
        public static void Spawn(User user)
        {
            StargateGenerator.GenerateStargate((Vector3i)user.Position);
        }
    }
}



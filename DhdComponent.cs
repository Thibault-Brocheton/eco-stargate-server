namespace CavRn.Stargate
{
    using Eco.Core.Controller;
    using Eco.Gameplay.Interactions.Interactors;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Shared.IoC;
    using Eco.Shared.Items;
    using Eco.Shared.Serialization;
    using Eco.Shared.SharedTypes;
    using Eco.Shared.Utils;
    using System.Linq;
    using System.Threading.Tasks;

    [Serialized, NoIcon]
    public class DhdComponent: WorldObjectComponent
    {
        private StargateComponent? StargateComponent
        {
            get
            {
                var stargate = ServiceHolder<IWorldObjectManager>.Obj.GetObjectsWithin(this.Parent.WorldPosXZ(), 15).FirstOrDefault(worldObject => worldObject.GetType() == typeof(StargateObject));

                if (stargate != null) return stargate.GetComponent<StargateComponent>();

                return null;

            }
        }

        [Interaction(InteractionTrigger.InteractKey, "Press", authRequired: AccessType.None, requiredEnvVars: new[] { "Press" })]
        public void Press(Player player, InteractionTriggerInfo trigger, InteractionTarget target)
        {
            if (this.StargateComponent is null)
            {
                player.MsgLocStr("Le DHD n'est pas à proximité d'une porte des étoiles !");
                return;
            }

            if (!this.Parent.Enabled)
            {
                player.MsgLocStr("Le DHD n'est pas alimenté en énergie !");
                return;
            }

            if (!target.TryGetParameter("Press", out var objParam)) return;

            var glyphId = (string)objParam;

            var response = this.StargateComponent?.AddGlyph(glyphId) ?? StargateComponent.Response.End;

            switch (response)
            {
                case StargateComponent.Response.Success:
                    this.Parent.SetAnimatedState($"Glyph{glyphId}", true);
                    break;
                case StargateComponent.Response.End:
                {
                    this.Deactivate();
                    break;
                }
                case StargateComponent.Response.NoAction:
                {
                    player.MsgLocStr("Attendez un instant !");
                    break;
                }
            }
        }

        [Interaction(InteractionTrigger.InteractKey, "Activate", authRequired: AccessType.None, requiredEnvVars: new[] { "Activate" })]
        public void Activate(Player player, InteractionTriggerInfo trigger, InteractionTarget target)
        {
            if (this.StargateComponent is null)
            {
                player.MsgLocStr("Le DHD n'est pas à proximité d'une porte des étoiles !");
                return;
            }

            if (!this.Parent.Enabled)
            {
                player.MsgLocStr("Le DHD n'est pas alimenté en énergie !");
                return;
            }

            var response = this.StargateComponent?.Activate(player) ?? StargateComponent.Response.End;

            switch (response)
            {
                case StargateComponent.Response.Success:
                    this.Parent.SetAnimatedState("Dome", true);
                    break;
                case StargateComponent.Response.End:
                {
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(500);
                        this.Deactivate();
                    });
                    break;
                }
                case StargateComponent.Response.NoAction:
                {
                    player.MsgLocStr("Attendez un instant !");
                    break;
                }
            }
        }

        public void Deactivate()
        {
            for (var i = 1; i <= 39; i++)
            {
                this.Parent.SetAnimatedState($"Glyph{i}", false);
            }
            this.Parent.SetAnimatedState("Dome", false);
        }
    }
}

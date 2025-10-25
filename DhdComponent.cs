using Eco.Shared.Localization;
using Eco.Shared.Math;

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
                var stargate = ServiceHolder<IWorldObjectManager>.Obj.All.OfType<StargateObject>()
                    .Where(s => WorldPosition3i.Distance((WorldPosition3i)this.Parent.Position3i, (WorldPosition3i)s.Position3i) < 8)
                    .OrderBy(s => WorldPosition3i.Distance((WorldPosition3i)this.Parent.Position3i, (WorldPosition3i)s.Position3i))
                    .FirstOrDefault();

                return stargate?.GetComponent<StargateComponent>();
            }
        }

        [Interaction(InteractionTrigger.InteractKey, "Press", authRequired: AccessType.None, requiredEnvVars: new[] { "Press" })]
        public void Press(Player player, InteractionTriggerInfo trigger, InteractionTarget target)
        {
            if (this.StargateComponent is null)
            {
                player.Msg(new LocString("The DHD is not near a Stargate (8 blocks)!"));
                return;
            }

            if (!this.Parent.Enabled)
            {
                player.Msg(new LocString("The DHD is not enabled!"));
                return;
            }

            if (!target.TryGetParameter("Press", out var objParam)) return;

            var glyphId = (string)objParam;

            var response = this.StargateComponent?.AddGlyph(glyphId) ?? StargateComponent.Response.End;

            switch (response)
            {
                case StargateComponent.Response.Success:
                    this.Parent.SetAnimatedState($"Glyph{glyphId}", true);
                    this.Parent.TriggerAnimatedEvent("PressSound");
                    break;
                case StargateComponent.Response.End:
                {
                    this.Deactivate();
                    break;
                }
                case StargateComponent.Response.AlreadyDone:
                {
                    player.Msg(new LocString("Glyph already engaged!"));
                    break;
                }
                case StargateComponent.Response.NotPossible:
                {
                    player.Msg(new LocString("You cannot add a new glyph! Press the central button!"));
                    break;
                }
                case StargateComponent.Response.NoAction:
                {
                    player.Msg(new LocString("Wait a moment!"));
                    break;
                }
            }
        }

        [Interaction(InteractionTrigger.InteractKey, "Activate", authRequired: AccessType.None, requiredEnvVars: new[] { "Activate" })]
        public void Activate(Player player, InteractionTriggerInfo trigger, InteractionTarget target)
        {
            if (this.StargateComponent is null)
            {
                player.Msg(new LocString("The DHD is not near a Stargate (8 blocks)!"));
                return;
            }

            if (!this.Parent.Enabled)
            {
                player.Msg(new LocString("The DHD is not enabled!"));
                return;
            }

            this.Parent.TriggerAnimatedEvent("DomeSound");

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
                    player.Msg(new LocString("Wait a moment!"));
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

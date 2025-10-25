using Eco.Shared.Localization;

namespace CavRn.Stargate
{
    using Eco.Core.Controller;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Interactions.Interactors;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Shared.IoC;
    using Eco.Shared.Items;
    using Eco.Shared.Math;
    using Eco.Shared.Serialization;
    using Eco.Shared.SharedTypes;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;
    using System;

    [Serialized, NoIcon]
    public class RingsComponent: WorldObjectComponent
    {
        private bool isTeleporting = false;

        [Interaction(InteractionTrigger.RightClick, "Activate", authRequired: AccessType.ConsumerAccess)]
        public void Activate(Player player, InteractionTriggerInfo trigger, InteractionTarget target)
        {
            var otherRings = ServiceHolder<IWorldObjectManager>.Obj.All
                .OfType<RingsObject>()
                .Where(w => Math.Abs(w.Position.X - this.Parent.Position.X) < 1
                            && Math.Abs(w.Position.Z - this.Parent.Position.Z) < 1
                            && w != this.Parent)
                .ToList();

            var otherRing = otherRings.Where(r => r.Position.Y > this.Parent.Position.Y).MinBy(r => r.Position.Y) ?? otherRings.MinBy(r => r.Position.Y);

            if (otherRing is null)
            {
                player.Error(new LocString("Can't find any other ring to teleport to. Make sure you add an other in the same X/Z position (less than 1 block), but in a different height"));
                return;
            }

            var otherRingComponent = otherRing.GetComponent<RingsComponent>();

            if (this.isTeleporting || otherRingComponent.isTeleporting)
            {
                player.Error(new LocString("Wait for the teleportation of this rings or the target rings to finish."));
                return;
            }

            this.isTeleporting = true;
            otherRingComponent.isTeleporting = true;

            this.Parent.TriggerAnimatedEvent("RingAction");
            otherRing.TriggerAnimatedEvent("RingAction");

            // wait 4 seconds
            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(4000);

                    var myPlayers = UserManager.Users
                        .Where(user => user.IsOnline
                            && !user.Player.MountManager.IsMounted
                            && Eco.Shared.Math.Vector2.Distance(user.Position.XZ(), this.Parent.Position.XZ()) < 2.25
                            && user.Position.Y > this.Parent.Position.Y - 1
                            && user.Position.Y < this.Parent.Position.Y + 4)
                        .ToList();

                    var myVehicles = ServiceHolder<IWorldObjectManager>.Obj.All
                        .OfType<PhysicsWorldObject>()
                        .Where(w => Eco.Shared.Math.Vector2.Distance(w.Position.XZ(), this.Parent.Position.XZ()) < 2.25
                            && w.Position.Y > this.Parent.Position.Y - 1
                            && w.Position.Y < this.Parent.Position.Y + 4)
                        .Where(w => w.GetComponent<VehicleComponent>() is not null)
                        .ToList();

                    var targetPlayers = UserManager.Users
                        .Where(user => user.IsOnline
                            && !user.Player.MountManager.IsMounted
                            && Eco.Shared.Math.Vector2.Distance(user.Position.XZ(), otherRing.Position.XZ()) < 2.25
                            && user.Position.Y > otherRing.Position.Y - 1
                            && user.Position.Y < otherRing.Position.Y + 4)
                        .ToList();

                    var targetVehicles = ServiceHolder<IWorldObjectManager>.Obj.All
                        .OfType<PhysicsWorldObject>()
                        .Where(w => Eco.Shared.Math.Vector2.Distance(w.Position.XZ(), otherRing.Position.XZ()) < 2.25
                            && w.Position.Y > otherRing.Position.Y - 1
                            && w.Position.Y < otherRing.Position.Y + 4)
                        .Where(w => w.GetComponent<VehicleComponent>() is not null)
                        .ToList();

                    var positionDiff = otherRing.Position - this.Parent.Position;
                    var offset = new Vector3(0, 0.5f, 0);

                    myPlayers.ForEach(user => user.Player.SetPosition(user.Position + positionDiff + offset));
                    myVehicles.ForEach(w =>
                    {
                        w.Position += positionDiff;

                        if (w.GetComponent<MountComponent>().MountedPlayers.Any())
                        {
                            w.Position += offset;
                        }

                        w.SyncPositionAndRotation();
                    });

                    targetPlayers.ForEach(user => user.Player.SetPosition(user.Position - positionDiff + offset));
                    targetVehicles.ForEach(w =>
                    {
                        w.Position -= positionDiff;

                        if (w.GetComponent<MountComponent>().MountedPlayers.Any())
                        {
                            w.Position += offset;
                        }

                        w.SyncPositionAndRotation();
                    });

                    await Task.Delay(4000);
                }
                finally
                {
                    this.isTeleporting = false;
                    otherRingComponent.isTeleporting = false;
                }
            });
        }
    }
}

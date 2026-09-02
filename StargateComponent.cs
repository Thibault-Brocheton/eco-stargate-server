using Eco.Shared.Localization;

namespace CavRn.Stargate
{
    using Eco.Core.Controller;
    using Eco.Core.Utils;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Mods.TechTree;
    using Eco.Shared.IoC;
    using Eco.Shared.Math;
    using Eco.Shared.Serialization;
    using Eco.Shared.Voxel;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;
    using System;

    [Serialized, CreateComponentTabLoc, HasIcon("PowerGridComponent")]
    public class StargateComponent : WorldObjectComponent, ITriggerVolumeListener
    {
        const string VortexVolumeName  = "Vortex";                //TriggerVolume name on the gate's client prefab.
        const string VortexOverlayName = "StargateVortexOverlay"; //Fullscreen overlay prefab in the mod bundle, shown to travellers during the transit.
        const float  VortexTravelSecs  = 4.0f;                    //Minimum transit duration so the overlay's vortex video plays out entirely before arrival.

        public override WorldObjectComponentClientAvailability Availability => WorldObjectComponentClientAvailability.Always;
        [SyncToView] public override string IconName => "PowerGridComponent";

        private static readonly Dictionary<int, string> NumberGlyphAssoc = new()
        {
            { 1, "GizehGlyphItem" },
            { 2, "CraterGlyphItem" },
            { 3, "VirgoGlyphItem" },
            { 4, "BootesGlyphItem" },
            { 5, "CentaurusGlyphItem" },
            { 6, "LibraGlyphItem" },
            { 7, "SerpensCaputGlyphItem" },
            { 8, "NormaGlyphItem" },
            { 9, "ScorpiusGlyphItem" },
            { 10, "CoronaAustralisGlyphItem" },
            { 11, "ScutumGlyphItem" },
            { 12, "SagittariusGlyphItem" },
            { 13, "AquilaGlyphItem" },
            { 14, "MicroscopiumGlyphItem" },
            { 15, "CapricornusGlyphItem" },
            { 16, "PiscisAustrinusGlyphItem" },
            { 17, "EquuleusGlyphItem" },
            { 18, "AquariusGlyphItem" },
            { 19, "PegasusGlyphItem" },
            { 20, "SculptorGlyphItem" },
            { 21, "PiscesGlyphItem" },
            { 22, "AndromedaGlyphItem" },
            { 23, "TriangulumGlyphItem" },
            { 24, "AriesGlyphItem" },
            { 25, "PerseusGlyphItem" },
            { 26, "CetusGlyphItem" },
            { 27, "TaurusGlyphItem" },
            { 28, "AurigaGlyphItem" },
            { 29, "EridanusGlyphItem" },
            { 30, "OrionGlyphItem" },
            { 31, "CanisMinorGlyphItem" },
            { 32, "MonocerosGlyphItem" },
            { 33, "GeminiGlyphItem" },
            { 34, "HydraGlyphItem" },
            { 35, "LynxGlyphItem" },
            { 36, "CancerGlyphItem" },
            { 37, "SextansGlyphItem" },
            { 38, "LeoMinorGlyphItem" },
            { 39, "LeoGlyphItem" },
        };

        public override InventoryMoveResult TryPickup(Player player, InventoryChangeSet playerInvChanges, Inventory targetInventory, bool force)
        {
            if (this.IsOpened)
            {
                Result.Fail(new LocString("You need to close the stargate before picking it!"));
            }

            return player.User.ToolbarSelected.Item is SteelHammerItem or ModernHammerItem
                ? Result.Succeeded
                : Result.Fail(new LocString("You need a Steel Hammer or a Modern Hammer to pickup this Stargate!"));
        }

        private DhdComponent? DhdComponent
        {
            get
            {
                var dhd = ServiceHolder<IWorldObjectManager>.Obj.All.OfType<DhdObject>()
                    .Where(s => WorldPosition3i.Distance((WorldPosition3i)this.Parent.Position3i, (WorldPosition3i)s.Position3i) < 8)
                    .OrderBy(s => WorldPosition3i.Distance((WorldPosition3i)this.Parent.Position3i, (WorldPosition3i)s.Position3i))
                    .FirstOrDefault();

                return dhd?.GetComponent<DhdComponent>();
            }
        }

        public enum Response
        {
            End,
            NoAction,
            NotPossible,
            AlreadyDone,
            Success,
        }

        public override void Initialize()
        {
            base.Initialize();

            if (this.OwnAddress.Count == 6 || this.OwnOrigin > 0) return;

            var xGlyphes = BuildAllOrderedPairs(new List<int>() { 2, 3, 4, 5, 6, 7, 8, 9,10,11,12,14,15,16,17,18 });
            var yGlyphes = BuildAllOrderedPairs(new List<int>() { 19,20,21,22,23 });
            var zGlyphes = BuildAllOrderedPairs(new List<int>() { 24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39 });

            var xg = GetCouple(xGlyphes, this.Parent.Position3i.X, World.WrappedVoxelSize.X);
            var yg = GetCouple(yGlyphes, this.Parent.Position3i.Y, World.WrappedVoxelSize.Y);
            var zg = GetCouple(zGlyphes, this.Parent.Position3i.Z, World.WrappedVoxelSize.Z);

            this.OwnAddress.Clear();
            this.OwnAddress.Add(xg.Item1);
            this.OwnAddress.Add(yg.Item1);
            this.OwnAddress.Add(zg.Item1);
            this.OwnAddress.Add(xg.Item2);
            this.OwnAddress.Add(yg.Item2);
            this.OwnAddress.Add(zg.Item2);

            this.OwnOrigin = 1;

            this.OwnAddressIcons = "Address: <size=400%>" + string.Join("", this.OwnAddress.Select(s => $"<link=\"Item:{NumberGlyphAssoc[s]}\"><icon name=\"{NumberGlyphAssoc[s]}\"></icon></link>")) + "</size>";
            this.OriginPointIcon = $"Origin: <size=400%><link=\"Item:{NumberGlyphAssoc[this.OwnOrigin]}\"><icon name=\"{NumberGlyphAssoc[this.OwnOrigin]}\"></icon></link></size>";
        }

        public override void Tick()
        {
            base.Tick();

            var config = StargatePlugin.Obj.Config;
            if      (this.openedAt  is not null && DateTime.Now.Subtract((DateTime)this.openedAt).TotalSeconds  > config.AutoCloseSeconds)                     this.Deactivate(true); //Wormhole lifetime, tracked here rather than by a delayed task so a closed-then-reopened gate never inherits the old timer.
            else if (!this.IsOpened && this.lastAction is not null && DateTime.Now.Subtract((DateTime)this.lastAction).TotalSeconds > config.DialTimeoutSeconds) this.Deactivate(true); //Abandoned dial.
        }

        //Fired by the Vortex TriggerVolume on the gate's client prefab when a traveller on foot or their vehicle crosses the event horizon.
        public void OnPlayerTriggerVolume(Player player, string volumeName, bool entered, bool vehicle)
        {
            if (entered && volumeName == VortexVolumeName) this.TravelThroughGate(player.User);
        }

        //Sends one traveller (on foot or driving) through the opened gate: hidden from other players while their client preloads the arrival area (same flow as the /tpp command),
        //a vortex overlay covering their own screen, then moved (or their vehicle) to the destination gate.
        private void TravelThroughGate(User user)
        {
            if (!this.IsOpened || this.IsCalledFromOutside || this.dialedStargate is null) return;

            var target    = this.dialedStargate.Parent;
            var targetRot = Eco.Shared.Math.Quaternion.LookRotation(-target.Rotation.Forward, target.Rotation.Up);
            Vector3 ArrivalPos(float behind) => new Vector3(target.Position.X, target.Position.Y + 0.25f, target.Position.Z) + target.Rotation.Back * behind;

            if (!user.Player.MountManager.IsMounted)
            {
                var targetPos = ArrivalPos(VehicleArrival.FootDistanceBehindGate);
                this.TeleportThroughGate(user, targetPos,
                    teleport: () =>
                    {
                        user.Player.SetPositionAndRotation(targetPos, targetRot);
                        user.Player.Msg(new LocString("You travelled through the Stargate!"));
                    },
                    setHidden: hidden => this.SetInvisible(user, hidden),
                    freezeMovement: true);
            }
            else if (user.Player.MountManager.Mount.Driver == user.Player)
            {
                if (!StargatePlugin.Obj.Config.AllowVehicleTravel) { user.Player.Msg(new LocString("Vehicles can't travel through the Stargate here, leave it behind and walk through.")); return; }

                var mount     = user.Player.MountManager.Mount;
                var targetPos = ArrivalPos(VehicleArrival.DistanceBehindGate(mount.Parent));
                this.TeleportThroughGate(user, targetPos,
                    teleport: () =>
                    {
                        if (user.Player.MountManager.Mount != mount) return; //Dismounted during the preload: they'll travel on foot instead.
                        TeleportWithOccupants(mount, targetPos, targetRot);
                    },
                    setHidden: hidden =>
                    {
                        this.SetInvisible(user, hidden);
                        mount.Parent.SetClientVisibility(!hidden);
                    },
                    freezeMovement: false); //The motor doesn't drive the vehicle, freezing it would do nothing.
            }
        }

        private void TeleportThroughGate(User user, Vector3 targetPos, Action teleport, Action<bool> setHidden, bool freezeMovement)
        {
            if (!this.teleportingUsers.TryAdd(user, 0)) return;

            var wasInvisible = user.IsInvisible;
            if (!wasInvisible) setHidden(true);

            var arrivalGate = this.dialedStargate?.Parent;
            _ = Task.Run(async () =>
            {
                try     { await user.Player.PreloadedTeleportAsync(targetPos, teleport, VortexOverlayName, freezeMovement, VortexTravelSecs, arrivalGate); }
                finally
                {
                    if (!wasInvisible) setHidden(false);
                    this.teleportingUsers.TryRemove(user, out _);
                }
            });
        }

        //Private in vanilla; without the reset a server running movement-hack detection would flag (or kick on) the occupants' jump.
        static readonly System.Reflection.MethodInfo? ResetMovementDetector = typeof(Player).GetMethod("ResetMovementDetector", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        //Moves the vehicle and the server-side position of everyone aboard. Their clients ride the seat, so a player teleport RPC would dismount them; but left at the origin
        //server-side, the driver gets the now-distant vehicle dropped from their client and the seat cleared as stale.
        static void TeleportWithOccupants(MountComponent mount, Vector3 pos, Eco.Shared.Math.Quaternion rot)
        {
            var vehicle = mount.Parent;
            vehicle.Position = pos;
            vehicle.Rotation = rot;
            vehicle.SyncPositionAndRotation();
            (vehicle as PhysicsWorldObject)?.MarkPoseUpdated();

            foreach (var occupant in mount.MountedPlayers)
            {
                occupant.User.Position = pos;
                occupant.User.MarkDirty();
                occupant.Position = pos;
                ResetMovementDetector?.Invoke(occupant, new object[] { pos });
                occupant.MinimapObject.UpdatePosition(pos);
            }
        }

        private void SetInvisible(User user, bool invisible)
        {
            user.IsInvisible = invisible;
            user.OnInvisible?.Invoke(user);
        }

        private static List<(int, int)> BuildAllOrderedPairs(List<int> glyphs)
        {
            var result = new List<(int, int)>();
            for (int i = 0; i < glyphs.Count; i++)
            {
                for (int j = 0; j < glyphs.Count; j++)
                {
                    if (i != j)
                    {
                        result.Add((glyphs[i], glyphs[j]));
                    }
                }
            }
            return result;
        }

        private static (int, int) GetCouple(List<(int, int)> couples, int x, int xMax)
        {
            double segmentLength = (double)xMax / couples.Count;
            int index = (int)Math.Floor(x / segmentLength);

            if (index < 0)
                index = 0;
            if (index >= couples.Count)
                index = couples.Count - 1;

            return couples[index];
        }

        private List<int> OwnAddress { get; set; } = new List<int>();
        private int OwnOrigin { get; set; } = 0;
        [SyncToView, Autogen, Serialized, PropReadOnly, UITypeName("StringDisplay")] public string OwnAddressIcons { get; set; } = "";
        [SyncToView, Autogen, Serialized, PropReadOnly, UITypeName("StringDisplay")] public string OriginPointIcon { get; set; } = "";

        private readonly List<string> dialAddress = new List<string>();

        //Travellers whose destination is preloading; Tick keeps detecting them in the vortex until the teleport lands.
        private readonly System.Collections.Concurrent.ConcurrentDictionary<User, byte> teleportingUsers = new();

        private DateTime rotatingUntil = DateTime.MinValue; //Ring spin end time; presses during a spin stack instead of being refused.
        private int dialSession = 0;                        //Bumped on Deactivate so pending chevron timers from a cancelled dial stay silent.
        private bool IsRotating => DateTime.Now < this.rotatingUntil;
        private bool IsOpened { get; set; }
        private bool IsCalledFromOutside { get; set; } = false;
        private DateTime? lastAction = null;
        private DateTime? openedAt   = null; //Set on the dialing gate only; the receiving gate follows it.
        private StargateComponent? dialedStargate;

        private void Deactivate(bool notifyDhd = false, bool notifyDialStargate = true)
        {
            if (!this.IsOpened && this.dialAddress.Count > 0)
            {
                this.Parent.TriggerAnimatedEvent("Fail");
            }

            this.lastAction = null;
            this.openedAt   = null;
            this.rotatingUntil = DateTime.MinValue;
            this.dialSession++;
            this.IsOpened = false;
            this.IsCalledFromOutside = false;

            this.Parent.SetAnimatedState("Vortex", this.IsOpened);
            this.Parent.SetAnimatedState("Blocked", false);

            this.Parent.SetAnimatedState("Chevron1", false);
            this.Parent.SetAnimatedState("Chevron2", false);
            this.Parent.SetAnimatedState("Chevron3", false);
            this.Parent.SetAnimatedState("Chevron4", false);
            this.Parent.SetAnimatedState("Chevron5", false);
            this.Parent.SetAnimatedState("Chevron6", false);
            this.Parent.SetAnimatedState("Chevron7", false);

            this.dialAddress.Clear();

            if (notifyDhd)
            {
                this.DhdComponent?.Deactivate();
            }

            if (notifyDialStargate && this.dialedStargate is not null)
            {
                this.dialedStargate.Deactivate(true, false);
            }

            this.dialedStargate = null;
        }

        public Response AddGlyph(string glyph)
        {
            this.lastAction = DateTime.Now;

            if (!this.Parent.Enabled || this.IsOpened)
            {
                return Response.NoAction;
            }

            if (this.dialAddress.Count >= 7)
            {
                return Response.NotPossible;
            }

            if (this.dialAddress.Contains(glyph))
            {
                return Response.AlreadyDone;
            }

            this.dialAddress.Add(glyph);
            var chevronIndex = this.dialAddress.Count; //Captured now: stacked presses each light their own chevron.
            var session      = this.dialSession;

            //Presses stack freely: only start a new spin when none is running (re-triggering would pile up ghost rotations in the animator).
            if (!this.IsRotating)
            {
                this.rotatingUntil = DateTime.Now.AddMilliseconds(8300);
                this.Parent.TriggerAnimatedEvent(chevronIndex == 7 ? "Rotate7" : "Rotate");
            }

            //The current spin's end catches up every press made during it: all their chevrons light together when it locks.
            var delay = (int)Math.Max(0, (this.rotatingUntil - DateTime.Now).TotalMilliseconds);
            _ = Task.Run(async () =>
            {
                await Task.Delay(delay);

                if (this.dialSession != session) return; //Dial was cancelled meanwhile: leave the chevron dark.
                this.Parent.SetAnimatedState($"Chevron{chevronIndex}", true);
            });

            return Response.Success;
        }

        private Response DialFromOutside(StargateComponent stargate)
        {
            if (this.IsOpened || this.IsCalledFromOutside)
            {
                return Response.End;
            }

            this.dialedStargate = stargate;
            this.lastAction = null;
            this.IsCalledFromOutside = true;
            this.IsOpened = true;
            this.rotatingUntil = DateTime.MinValue;
            this.Parent.SetAnimatedState("Rotate", false);
            this.Parent.SetAnimatedState("Blocked", true); //Arrival side: a solid wall blocks entering the vortex the wrong way.
            this.Parent.TriggerAnimatedEvent("OpenVortex");

            _ = Task.Run(async () =>
            {
                await Task.Delay(500);

                this.Parent.SetAnimatedState("Vortex" , this.IsOpened);
            });

            this.Parent.SetAnimatedState("Chevron1", true);
            this.Parent.SetAnimatedState("Chevron2", true);
            this.Parent.SetAnimatedState("Chevron3", true);
            this.Parent.SetAnimatedState("Chevron4", true);
            this.Parent.SetAnimatedState("Chevron5", true);
            this.Parent.SetAnimatedState("Chevron6", true);
            this.Parent.SetAnimatedState("Chevron7", true);

            this.DhdComponent?.Parent.SetAnimatedState("Dome", true);

            return Response.Success;
        }

        public Response Activate(Player player)
        {
            this.lastAction = DateTime.Now;

            if (!this.Parent.Enabled)
            {
                return Response.NoAction;
            }

            if (this.dialAddress.Count == 0)
            {
                this.Deactivate();
                return Response.End;
            }

            if (this.dialAddress.Count != 7 || this.IsOpened || this.OwnOrigin != int.Parse(this.dialAddress[6]))
            {
                this.Deactivate();

                if (this.IsOpened) player.Msg(new LocString("The destination Stargate is already opened!"));

                return Response.End;
            }

            var foundStargate = ServiceHolder<IWorldObjectManager>.Obj.All.OfType<StargateObject>()
                .Where(worldObject => worldObject != this.Parent)
                .Select(o =>
                {
                    return o.GetComponent<StargateComponent>();
                })
                .FirstOrDefault(o => string.Join(",", o.OwnAddress) == string.Join(",", this.dialAddress.Slice(0, 6)));

            if (foundStargate is null)
            {
                this.Deactivate();

                return Response.End;
            }

            var dialResponse = foundStargate.DialFromOutside(this);

            if (dialResponse == Response.End)
            {
                this.Deactivate();
                return Response.End;
            }

            this.dialedStargate = foundStargate;
            this.IsOpened = true;
            this.openedAt = DateTime.Now;
            this.Parent.TriggerAnimatedEvent("OpenVortex");

            _ = Task.Run(async () =>
            {
                await Task.Delay(500);

                this.Parent.SetAnimatedState("Vortex" , this.IsOpened);
            });

            return Response.Success;
        }
    }
}

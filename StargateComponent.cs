using Eco.Shared.Localization;

namespace CavRn.Stargate
{
    using Eco.Core.Controller;
    using Eco.Core.Utils;
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
    public class StargateComponent : WorldObjectComponent
    {
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

            if (this.IsOpened && !this.IsCalledFromOutside && this.dialedStargate is not null)
            {
                var usersInPosition = UserManager.Users
                    .Where(user => user.IsOnline
                                   && Vector3.Distance(user.Position, this.Parent.Position + Vector3i.Up) < 1.25f)
                    .ToList();

                var usersToTeleport = usersInPosition.Where(user => !user.Player.MountManager.IsMounted);
                var targetPos = new Vector3(this.dialedStargate.Parent.Position.X, this.dialedStargate.Parent.Position.Y + 0.25f, this.dialedStargate.Parent.Position.Z) + this.dialedStargate.Parent.Rotation.Back;
                var targetRot = Eco.Shared.Math.Quaternion.LookRotation(-this.dialedStargate.Parent.Rotation.Forward, this.dialedStargate.Parent.Rotation.Up);

                foreach (var user in usersToTeleport)
                {
                    user.Player.SetPositionAndRotation(targetPos, targetRot);
                    user.Player.Msg(new LocString("You travelled through the Stargate!"));
                }

                var vehiclesToTeleports = usersInPosition
                    .Where(user => user.Player.MountManager.IsMounted && user.Player.MountManager.Mount.Driver == user.Player)
                    .Select(user => user.Player.MountManager.Mount.Parent);

                foreach (var vehiclesToTeleport in vehiclesToTeleports)
                {
                    vehiclesToTeleport.Position = targetPos;
                    vehiclesToTeleport.Rotation = targetRot;
                    vehiclesToTeleport.SyncPositionAndRotation();
                }
            }

            if (!this.IsOpened && this.lastAction is not null && DateTime.Now.Subtract((DateTime)this.lastAction).TotalSeconds > 90)
            {
                this.Deactivate(true);
            }
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

        private bool isRotating = false;
        private bool IsOpened { get; set; }
        private bool IsCalledFromOutside { get; set; } = false;
        private DateTime? lastAction = null;
        private StargateComponent? dialedStargate;

        private void Deactivate(bool notifyDhd = false, bool notifyDialStargate = true)
        {
            if (!this.IsOpened && this.dialAddress.Count > 0)
            {
                this.Parent.TriggerAnimatedEvent("Fail");
            }

            this.lastAction = null;
            this.isRotating = false;
            this.IsOpened = false;
            this.IsCalledFromOutside = false;

            this.Parent.SetAnimatedState("Vortex", this.IsOpened);

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

            if (!this.Parent.Enabled || this.isRotating || this.IsOpened)
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

            this.isRotating = true;
            this.Parent.TriggerAnimatedEvent(this.dialAddress.Count == 7 ? "Rotate7" : "Rotate");

            _ = Task.Run(async () =>
            {
                await Task.Delay(8300);

                this.isRotating = false;
                this.Parent.SetAnimatedState($"Chevron{this.dialAddress.Count}", true);
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
            this.isRotating = false;
            this.Parent.SetAnimatedState("Rotate", this.isRotating);
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

            if (!this.Parent.Enabled || this.isRotating)
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
            this.Parent.TriggerAnimatedEvent("OpenVortex");

            _ = Task.Run(async () =>
            {
                await Task.Delay(500);

                this.Parent.SetAnimatedState("Vortex" , this.IsOpened);
            });


            _ = Task.Run(async () =>
            {
                await Task.Delay(3800 * 60);

                this.Deactivate(true);
            });

            return Response.Success;
        }

        /*[Interaction(InteractionTrigger.InteractKey, "EnterVortex", authRequired: AccessType.None, requiredEnvVars: new[] { "EnterVortex" })]
        public async Task EnterVortex(Player player, InteractionTriggerInfo trigger, InteractionTarget target)
        {
            if (this.dialedStargate is null || !this.IsOpened) return;

            if (this.IsCalledFromOutside)
            {
                player.MsgLocStr("Vortex is not opened in this way!");
                return;
            }

            if (await player.ConfirmBoxLoc($"Do you want to travel through the stargate?"))
            {
                var pos = new Vector3i(this.dialedStargate.Parent.Position3i.X, this.dialedStargate.Parent.Position3i.Y, this.dialedStargate.Parent.Position3i.Z) + (2 * this.dialedStargate.Parent.Rotation.Back);
                player.SetPosition(pos);
                player.MsgLocStr("You travelled through the Stargate!");
            }
        }*/
    }
}

namespace CavRn.Stargate
{
    using Eco.Core.Controller;
    using Eco.Gameplay.Interactions.Interactors;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Shared.IoC;
    using Eco.Shared.Items;
    using Eco.Shared.Math;
    using Eco.Shared.Serialization;
    using Eco.Shared.SharedTypes;
    using Eco.Shared.Utils;
    using Eco.Shared.Voxel;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System;

    [Serialized, CreateComponentTabLoc, NoIcon]
    public class StargateComponent : WorldObjectComponent
    {
        public override WorldObjectComponentClientAvailability Availability => WorldObjectComponentClientAvailability.Always;

        private DhdComponent? DhdComponent
        {
            get
            {
                var dhd = ServiceHolder<IWorldObjectManager>.Obj.GetObjectsWithin(this.Parent.WorldPosXZ(), 15).FirstOrDefault(worldObject => worldObject.GetType() == typeof(DhdObject));

                if (dhd != null) return dhd.GetComponent<DhdComponent>();

                return null;
            }
        }

        public enum Response
        {
            End,
            NoAction,
            Success,
        }

        public override void Initialize()
        {
            base.Initialize();

            if (this.OwnAddress.Count == 6 || this.OwnOrigin > 0) return;

            var xGlyphes = BuildAllOrderedPairs([ 2, 3, 4, 5, 6, 7, 8, 9,10,11,12,14,15,16,17,18,19]);
            var yGlyphes = BuildAllOrderedPairs([20,21,22]);
            var zGlyphes = BuildAllOrderedPairs([23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39]);

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

            this.OwnAddressIcons = "Address: <size=500%>" + string.Join("", this.OwnAddress.Select(s => $"<link=\"Item:Glyph{s}Item\"><icon name=\"Glyph{s}Item\"></icon></link>")) + "</size>";
            this.OriginPointIcon = $"Origin: <size=500%><link=\"Item:Glyph{this.OwnOrigin}Item\"><icon name=\"Glyph{this.OwnOrigin}Item\"></icon></link></size>";
        }

        public override void Tick()
        {
            base.Tick();

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
        public int OwnOrigin { get; private set; } = 0;
        [SyncToView, Autogen, Serialized, PropReadOnly, UITypeName("StringDisplay")] public string OwnAddressIcons { get; set; } = "";
        [SyncToView, Autogen, Serialized, PropReadOnly, UITypeName("StringDisplay")] public string OriginPointIcon { get; set; } = "";

        private readonly List<string> dialAddress = [];

        private bool isRotating = false;
        public bool IsOpened { get; private set; }
        public bool IsCalledFromOutside { get; set; } = false;
        private DateTime? lastAction = null;
        private StargateComponent? dialedStargate;

        private void Deactivate(bool notifyDhd = false, bool notifyDialStargate = true)
        {
            this.lastAction = null;
            this.isRotating = false;
            this.IsOpened = false;
            this.IsCalledFromOutside = false;

            this.Parent.SetAnimatedState("Rotate", this.isRotating);
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
                this.Deactivate();

                return Response.End;
            }

            this.dialAddress.Add(glyph);

            this.isRotating = true;
            this.Parent.SetAnimatedState("Rotate", this.isRotating);

            _ = Task.Run(async () =>
            {
                await Task.Delay(8300);

                this.isRotating = false;
                this.Parent.SetAnimatedState("Rotate", this.isRotating);
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
            this.Parent.SetAnimatedState("Rotate" , this.isRotating);
            this.Parent.SetAnimatedState("Vortex" , this.IsOpened);

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

            if (this.dialAddress.Count != 7 || this.IsOpened || this.OwnOrigin != int.Parse(this.dialAddress[6]))
            {
                this.Deactivate();

                if (this.IsOpened) player.MsgLocStr("La porte des étoiles de destination est déjà ouverte.");

                return Response.End;
            }

            var foundStargate = ServiceHolder<IWorldObjectManager>.Obj.All
                .Where(worldObject => worldObject.GetType() == typeof(StargateObject) && worldObject != this.Parent)
                .Select(o => o.GetComponent<StargateComponent>())
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
            this.Parent.SetAnimatedState("Vortex" , this.IsOpened);

            _ = Task.Run(async () =>
            {
                await Task.Delay(3800 * 60);

                this.Deactivate(true);
            });

            return Response.Success;
        }

        [Interaction(InteractionTrigger.InteractKey, "EnterVortex", authRequired: AccessType.None, requiredEnvVars: new[] { "EnterVortex" })]
        public async Task EnterVortex(Player player, InteractionTriggerInfo trigger, InteractionTarget target)
        {
            if (this.dialedStargate is null || !this.IsOpened) return;

            if (this.IsCalledFromOutside)
            {
                player.MsgLocStr("Le vortex n'est pas ouvert dans ce sens !");
                return;
            }

            if (await player.ConfirmBoxLoc($"Souhaitez-vous voyager à travers la porte ?"))
            {
                var pos = new Vector3i(this.dialedStargate.Parent.Position3i.X, this.dialedStargate.Parent.Position3i.Y, this.dialedStargate.Parent.Position3i.Z) + (2 * this.dialedStargate.Parent.Rotation.Back);
                player.SetPosition(pos);
                player.MsgLocStr("Vous avez voyagé à travers la Porte des Etoiles !");
            }
        }
    }
}

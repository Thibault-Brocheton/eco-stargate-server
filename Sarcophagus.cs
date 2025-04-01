namespace CavRn.Stargate
{
    using Eco.Core.Items;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Occupancy;
    using Eco.Gameplay.Systems.NewTooltip;
    using Eco.Shared.Items;
    using Eco.Shared.Localization;
    using Eco.Shared.Math;
    using Eco.Shared.Serialization;
    using Eco.Shared.Utils;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System;

    [Serialized]
    [RequireComponent(typeof(OnOffComponent))]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(LinkComponent))]
    [RequireComponent(typeof(CraftingComponent))]
    [RequireComponent(typeof(PowerGridComponent))]
    [RequireComponent(typeof(PowerConsumptionComponent))]
    [Tag("Usable")]
    public class SarcophagusObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(SarcophagusItem);
        public override LocString DisplayName => Localizer.DoStr("Sarcophagus");
        public override TableTextureMode TableTexture => TableTextureMode.Metal;

        static SarcophagusObject()
		{
			WorldObject.AddOccupancy<SarcophagusObject>(new List
                <BlockOccupancy>(){
				new BlockOccupancy(new Vector3i(-1, 0, 0)),
				new BlockOccupancy(new Vector3i( 0, 0, 0)),
				new BlockOccupancy(new Vector3i( 1, 0, 0)),
				new BlockOccupancy(new Vector3i(-1, 1, 0)),
				new BlockOccupancy(new Vector3i( 0, 1, 0)),
				new BlockOccupancy(new Vector3i( 1, 1, 0)),

				new BlockOccupancy(new Vector3i(-1, 0, 1)),
				new BlockOccupancy(new Vector3i( 0, 0, 1)),
				new BlockOccupancy(new Vector3i( 1, 0, 1)),
				new BlockOccupancy(new Vector3i(-1, 1, 1)),
				new BlockOccupancy(new Vector3i( 0, 1, 1)),
				new BlockOccupancy(new Vector3i( 1, 1, 1)),

				new BlockOccupancy(new Vector3i(-1, 0, 2)),
				new BlockOccupancy(new Vector3i( 0, 0, 2)),
				new BlockOccupancy(new Vector3i( 1, 0, 2)),
				new BlockOccupancy(new Vector3i(-1, 1, 2)),
				new BlockOccupancy(new Vector3i( 0, 1, 2)),
				new BlockOccupancy(new Vector3i( 1, 1, 2)),

				new BlockOccupancy(new Vector3i(-1, 0, -1)),
				new BlockOccupancy(new Vector3i( 0, 0, -1)),
				new BlockOccupancy(new Vector3i( 1, 0, -1)),
				new BlockOccupancy(new Vector3i(-1, 1, -1)),
				new BlockOccupancy(new Vector3i( 0, 1, -1)),
				new BlockOccupancy(new Vector3i( 1, 1, -1)),

				new BlockOccupancy(new Vector3i(-1, 0, -2)),
				new BlockOccupancy(new Vector3i( 0, 0, -2)),
				new BlockOccupancy(new Vector3i( 1, 0, -2)),
				new BlockOccupancy(new Vector3i(-1, 1, -2)),
				new BlockOccupancy(new Vector3i( 0, 1, -2)),
				new BlockOccupancy(new Vector3i( 1, 1, -2)),

            });
		}

        protected override void Initialize()
        {
            this.GetComponent<PowerConsumptionComponent>().Initialize(500000);
            this.GetComponent<PowerGridComponent>().Initialize(50, new ElectricPower());
        }
    }

    [Serialized]
    [LocDisplayName("Sarcophagus")]
    [Category("Hidden"), Tag("NotInBrowser")]
    [Weight(35000)]
    public class SarcophagusItem : WorldObjectItem<SarcophagusObject>
    {
        public override bool ShowLocationsInWorld => false;
        [NewTooltip(CacheAs.SubType, 7)] public static LocString PowerConsumptionTooltip() => Localizer.Do($"Consumes: {Text.Info(500000)}w of {new ElectricPower().Name} power.");
    }
}

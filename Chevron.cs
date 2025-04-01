namespace CavRn.Stargate
{
    using Eco.Core.Items;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Housing.PropertyValues;
    using Eco.Gameplay.Housing;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Occupancy;
    using Eco.Gameplay.Systems.TextLinks;
    using Eco.Shared.Items;
    using Eco.Shared.Localization;
    using Eco.Shared.Math;
    using Eco.Shared.Serialization;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System;

    [Serialized]
    [RequireComponent(typeof(HousingComponent))]
    [RequireComponent(typeof(ForSaleComponent))]
    [Tag("Usable")]
    public class ChevronObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(ChevronItem);
        public override LocString DisplayName => Localizer.DoStr("Chevron");
        public override TableTextureMode TableTexture => TableTextureMode.Stone;

        static ChevronObject()
        {
            WorldObject.AddOccupancy<ChevronObject>(new List<BlockOccupancy>(){
                new BlockOccupancy(new Vector3i(0, 0, 0)),
                new BlockOccupancy(new Vector3i(1, 0, 0)),
            });
        }

        protected override void Initialize()
        {
            this.GetComponent<HousingComponent>().HomeValue = ChevronItem.homeValue;
        }
    }

    [Serialized]
    [LocDisplayName("Chevron")]
    [Category("Hidden"), Tag("NotInBrowser")]
    [Weight(25000)]
    public class ChevronItem : WorldObjectItem<ChevronObject>
    {
        public override bool ShowLocationsInWorld => false;
        public override HomeFurnishingValue HomeValue => homeValue;
        public static readonly HomeFurnishingValue homeValue = new HomeFurnishingValue()
        {
            ObjectName                              = typeof(ChevronObject).UILink(),
            Category                                = HousingConfig.GetRoomCategory("Decoration"),
            BaseValue                               = 10,
            TypeForRoomLimit                        = Localizer.DoStr("Artifact"),
            DiminishingReturnMultiplier             = 0.1f
        };
    }
}

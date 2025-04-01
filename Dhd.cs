namespace CavRn.Stargate
{
    using Eco.Core.Controller;
    using Eco.Core.Items;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Occupancy;
    using Eco.Shared.Items;
    using Eco.Shared.Localization;
    using Eco.Shared.Math;
    using Eco.Shared.Serialization;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System;

    [Serialized]
    [RequireComponent(typeof(DhdComponent))]
    [Tag("Usable")]
    public class DhdObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(DhdItem);
        public override LocString DisplayName => Localizer.DoStr("Dhd");
        public override TableTextureMode TableTexture => TableTextureMode.Stone;

        static DhdObject()
		{
			WorldObject.AddOccupancy<DhdObject>(new List<BlockOccupancy>(){
				new BlockOccupancy(new Vector3i(0, 0, 0)),
				new BlockOccupancy(new Vector3i(0, 1, 0)),
            });
		}
    }

    [Serialized]
    [LocDisplayName("Dhd")]
    [Category("Hidden"), Tag("NotInBrowser"), NoIcon]
    [Weight(10000)]
    public class DhdItem : WorldObjectItem<DhdObject>
    {
        public override bool ShowLocationsInWorld => false;
    }
}

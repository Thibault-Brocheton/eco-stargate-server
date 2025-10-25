
using Eco.Core.Items;
using Eco.Gameplay.Components.Auth;

namespace CavRn.Stargate
{
	using Eco.Gameplay.Components;
	using Eco.Mods.TechTree;
	using System.Collections.Generic;
    using Eco.Gameplay.Items.Recipes;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Occupancy;
    using Eco.Gameplay.Skills;
    using Eco.Shared.Items;
    using Eco.Shared.Localization;
    using Eco.Shared.Math;
    using Eco.Shared.Serialization;
    using System;
    using Eco.Core.Utils;

    [Serialized]
	[RequireComponent(typeof(RingsComponent))]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(MinimapComponent))]
    [RequireComponent(typeof(OccupancyRequirementComponent))]
    [RequireComponent(typeof(ForSaleComponent))]
    [Tag("Usable")]
    [Ecopedia("Stargate", "Stargate", subPageName: "Rings")]
    public class RingsObject : WorldObject, IRepresentsItem, IPickupConfirmationComponent
    {
        public override LocString DisplayName { get { return Localizer.DoStr("Rings"); } }
        public override LocString DisplayDescription
        {
            get { return new LocString("Allow teleportation on Y level"); }
        }
        public override TableTextureMode TableTexture => TableTextureMode.Stone;
        public virtual Type RepresentedItemType { get { return typeof(RingsItem); } }

        LocString GetComponentPickupConfirmation() => new LocString("Are you sure you want to pickup these Rings?");
        public Result CanPickup()                  => Result.Succeeded;

		static RingsObject()
		{
            WorldObject.AddOccupancy<RingsObject>(new List<BlockOccupancy>(){
            new BlockOccupancy(new Vector3i(-2, 0, -2)),
            new BlockOccupancy(new Vector3i(-2, 0, -1)),
            new BlockOccupancy(new Vector3i(-2, 0, 0)),
            new BlockOccupancy(new Vector3i(-2, 0, 1)),
            new BlockOccupancy(new Vector3i(-2, 0, 2)),
            new BlockOccupancy(new Vector3i(-2, 1, -2)),
            new BlockOccupancy(new Vector3i(-2, 1, -1)),
            new BlockOccupancy(new Vector3i(-2, 1, 0)),
            new BlockOccupancy(new Vector3i(-2, 1, 1)),
            new BlockOccupancy(new Vector3i(-2, 1, 2)),
            new BlockOccupancy(new Vector3i(-2, 2, -2)),
            new BlockOccupancy(new Vector3i(-2, 2, -1)),
            new BlockOccupancy(new Vector3i(-2, 2, 0)),
            new BlockOccupancy(new Vector3i(-2, 2, 1)),
            new BlockOccupancy(new Vector3i(-2, 2, 2)),
            new BlockOccupancy(new Vector3i(-2, 3, -2)),
            new BlockOccupancy(new Vector3i(-2, 3, -1)),
            new BlockOccupancy(new Vector3i(-2, 3, 0)),
            new BlockOccupancy(new Vector3i(-2, 3, 1)),
            new BlockOccupancy(new Vector3i(-2, 3, 2)),
            new BlockOccupancy(new Vector3i(-2, 4, -2)),
            new BlockOccupancy(new Vector3i(-2, 4, -1)),
            new BlockOccupancy(new Vector3i(-2, 4, 0)),
            new BlockOccupancy(new Vector3i(-2, 4, 1)),
            new BlockOccupancy(new Vector3i(-2, 4, 2)),
            new BlockOccupancy(new Vector3i(-1, 0, -2)),
            new BlockOccupancy(new Vector3i(-1, 0, -1)),
            new BlockOccupancy(new Vector3i(-1, 0, 0)),
            new BlockOccupancy(new Vector3i(-1, 0, 1)),
            new BlockOccupancy(new Vector3i(-1, 0, 2)),
            new BlockOccupancy(new Vector3i(-1, 1, -2)),
            new BlockOccupancy(new Vector3i(-1, 1, -1)),
            new BlockOccupancy(new Vector3i(-1, 1, 0)),
            new BlockOccupancy(new Vector3i(-1, 1, 1)),
            new BlockOccupancy(new Vector3i(-1, 1, 2)),
            new BlockOccupancy(new Vector3i(-1, 2, -2)),
            new BlockOccupancy(new Vector3i(-1, 2, -1)),
            new BlockOccupancy(new Vector3i(-1, 2, 0)),
            new BlockOccupancy(new Vector3i(-1, 2, 1)),
            new BlockOccupancy(new Vector3i(-1, 2, 2)),
            new BlockOccupancy(new Vector3i(-1, 3, -2)),
            new BlockOccupancy(new Vector3i(-1, 3, -1)),
            new BlockOccupancy(new Vector3i(-1, 3, 0)),
            new BlockOccupancy(new Vector3i(-1, 3, 1)),
            new BlockOccupancy(new Vector3i(-1, 3, 2)),
            new BlockOccupancy(new Vector3i(-1, 4, -2)),
            new BlockOccupancy(new Vector3i(-1, 4, -1)),
            new BlockOccupancy(new Vector3i(-1, 4, 0)),
            new BlockOccupancy(new Vector3i(-1, 4, 1)),
            new BlockOccupancy(new Vector3i(-1, 4, 2)),
            new BlockOccupancy(new Vector3i(0, 0, -2)),
            new BlockOccupancy(new Vector3i(0, 0, -1)),
            new BlockOccupancy(new Vector3i(0, 0, 0)),
            new BlockOccupancy(new Vector3i(0, 0, 1)),
            new BlockOccupancy(new Vector3i(0, 0, 2)),
            new BlockOccupancy(new Vector3i(0, 1, -2)),
            new BlockOccupancy(new Vector3i(0, 1, -1)),
            new BlockOccupancy(new Vector3i(0, 1, 0)),
            new BlockOccupancy(new Vector3i(0, 1, 1)),
            new BlockOccupancy(new Vector3i(0, 1, 2)),
            new BlockOccupancy(new Vector3i(0, 2, -2)),
            new BlockOccupancy(new Vector3i(0, 2, -1)),
            new BlockOccupancy(new Vector3i(0, 2, 0)),
            new BlockOccupancy(new Vector3i(0, 2, 1)),
            new BlockOccupancy(new Vector3i(0, 2, 2)),
            new BlockOccupancy(new Vector3i(0, 3, -2)),
            new BlockOccupancy(new Vector3i(0, 3, -1)),
            new BlockOccupancy(new Vector3i(0, 3, 0)),
            new BlockOccupancy(new Vector3i(0, 3, 1)),
            new BlockOccupancy(new Vector3i(0, 3, 2)),
            new BlockOccupancy(new Vector3i(0, 4, -2)),
            new BlockOccupancy(new Vector3i(0, 4, -1)),
            new BlockOccupancy(new Vector3i(0, 4, 0)),
            new BlockOccupancy(new Vector3i(0, 4, 1)),
            new BlockOccupancy(new Vector3i(0, 4, 2)),
            new BlockOccupancy(new Vector3i(1, 0, -2)),
            new BlockOccupancy(new Vector3i(1, 0, -1)),
            new BlockOccupancy(new Vector3i(1, 0, 0)),
            new BlockOccupancy(new Vector3i(1, 0, 1)),
            new BlockOccupancy(new Vector3i(1, 0, 2)),
            new BlockOccupancy(new Vector3i(1, 1, -2)),
            new BlockOccupancy(new Vector3i(1, 1, -1)),
            new BlockOccupancy(new Vector3i(1, 1, 0)),
            new BlockOccupancy(new Vector3i(1, 1, 1)),
            new BlockOccupancy(new Vector3i(1, 1, 2)),
            new BlockOccupancy(new Vector3i(1, 2, -2)),
            new BlockOccupancy(new Vector3i(1, 2, -1)),
            new BlockOccupancy(new Vector3i(1, 2, 0)),
            new BlockOccupancy(new Vector3i(1, 2, 1)),
            new BlockOccupancy(new Vector3i(1, 2, 2)),
            new BlockOccupancy(new Vector3i(1, 3, -2)),
            new BlockOccupancy(new Vector3i(1, 3, -1)),
            new BlockOccupancy(new Vector3i(1, 3, 0)),
            new BlockOccupancy(new Vector3i(1, 3, 1)),
            new BlockOccupancy(new Vector3i(1, 3, 2)),
            new BlockOccupancy(new Vector3i(1, 4, -2)),
            new BlockOccupancy(new Vector3i(1, 4, -1)),
            new BlockOccupancy(new Vector3i(1, 4, 0)),
            new BlockOccupancy(new Vector3i(1, 4, 1)),
            new BlockOccupancy(new Vector3i(1, 4, 2)),
            new BlockOccupancy(new Vector3i(2, 0, -2)),
            new BlockOccupancy(new Vector3i(2, 0, -1)),
            new BlockOccupancy(new Vector3i(2, 0, 0)),
            new BlockOccupancy(new Vector3i(2, 0, 1)),
            new BlockOccupancy(new Vector3i(2, 0, 2)),
            new BlockOccupancy(new Vector3i(2, 1, -2)),
            new BlockOccupancy(new Vector3i(2, 1, -1)),
            new BlockOccupancy(new Vector3i(2, 1, 0)),
            new BlockOccupancy(new Vector3i(2, 1, 1)),
            new BlockOccupancy(new Vector3i(2, 1, 2)),
            new BlockOccupancy(new Vector3i(2, 2, -2)),
            new BlockOccupancy(new Vector3i(2, 2, -1)),
            new BlockOccupancy(new Vector3i(2, 2, 0)),
            new BlockOccupancy(new Vector3i(2, 2, 1)),
            new BlockOccupancy(new Vector3i(2, 2, 2)),
            new BlockOccupancy(new Vector3i(2, 3, -2)),
            new BlockOccupancy(new Vector3i(2, 3, -1)),
            new BlockOccupancy(new Vector3i(2, 3, 0)),
            new BlockOccupancy(new Vector3i(2, 3, 1)),
            new BlockOccupancy(new Vector3i(2, 3, 2)),
            new BlockOccupancy(new Vector3i(2, 4, -2)),
            new BlockOccupancy(new Vector3i(2, 4, -1)),
            new BlockOccupancy(new Vector3i(2, 4, 0)),
            new BlockOccupancy(new Vector3i(2, 4, 1)),
            new BlockOccupancy(new Vector3i(2, 4, 2)),
            });
		}
    }

    [Serialized]
    [LocDisplayName("Rings")]
    [Weight(12000)]
    public class RingsItem : WorldObjectItem<RingsObject>
    {
        protected override OccupancyContext GetOccupancyContext => new SideAttachedContext( 0  | DirectionAxisFlags.Down , WorldObject.GetOccupancyInfo(this.WorldObjectType));
    }

    [RequiresSkill(typeof(BlacksmithSkill), 5)]
    public class RingsRecipe : RecipeFamily
    {
        public RingsRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                "Rings",  //noloc
                Localizer.DoStr("Rings"),
                new List<IngredientElement>
                {
                    new IngredientElement(typeof(RebarItem), 120),
                    new IngredientElement(typeof(HeatSinkItem), 20),
                    new IngredientElement(typeof(LubricantItem), 30),
                },
                new List<CraftingElement>
                {
                    new CraftingElement<RingsItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 15;
            this.LaborInCalories = CreateLaborInCaloriesValue(100_000);
            this.CraftMinutes = CreateCraftTimeValue(240);
            this.Initialize(Localizer.DoStr("Rings"), typeof(RingsRecipe));
            CraftingComponent.AddRecipe(typeof(BlacksmithTableObject), this);
        }
    }
}

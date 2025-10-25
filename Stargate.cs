using Eco.Gameplay.Components.Auth;

namespace CavRn.Stargate
{
	using Eco.Gameplay.Components;
	using Eco.Mods.TechTree;
	using System.Collections.Generic;
    using Eco.Core.Controller;
    using Eco.Core.Items;
    using Eco.Core.Utils;
    using Eco.Gameplay.Items.Recipes;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Occupancy;
    using Eco.Gameplay.Skills;
    using Eco.Shared.Items;
    using Eco.Shared.Localization;
    using Eco.Shared.Math;
    using Eco.Shared.Serialization;
    using System.ComponentModel;
    using System;

    [Serialized]
	[RequireComponent(typeof(StargateComponent))]
    [RequireComponent(typeof(OccupancyRequirementComponent))]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [Tag("Usable")]
    [Ecopedia("Stargate", "Stargate", subPageName: "Stargate")]
    public class StargateObject : WorldObject, IRepresentsItem, IPickupConfirmationComponent
    {
        public override LocString DisplayName { get { return Localizer.DoStr("Stargate"); } }
        public override LocString DisplayDescription
        {
            get { return new LocString(this.GetComponent<StargateComponent>()?.OwnAddressIcons ?? ""); }
        }
        public override TableTextureMode TableTexture => TableTextureMode.Stone;
        public virtual Type RepresentedItemType { get { return typeof(StargateItem); } }

        LocString GetComponentPickupConfirmation() => new LocString("Are you sure you want to pickup this stargate? Its coordinates will change if you change its location.");
        public Result CanPickup()                  => Result.Succeeded;

		static StargateObject()
		{
			WorldObject.AddOccupancy<StargateObject>(new List<BlockOccupancy>(){
				new BlockOccupancy(new Vector3i(-3, 1, 0)),
				new BlockOccupancy(new Vector3i(-2, 1, 0)),
				new BlockOccupancy(new Vector3i(-1, 1, 0)),
				new BlockOccupancy(new Vector3i( 0, 1, 0)),
				new BlockOccupancy(new Vector3i( 1, 1, 0)),
				new BlockOccupancy(new Vector3i( 2, 1, 0)),
				new BlockOccupancy(new Vector3i( 3, 1, 0)),

				new BlockOccupancy(new Vector3i(-3, 2, 0)),
				new BlockOccupancy(new Vector3i(-2, 2, 0)),
				new BlockOccupancy(new Vector3i(-1, 2, 0)),
				new BlockOccupancy(new Vector3i( 0, 2, 0)),
				new BlockOccupancy(new Vector3i( 1, 2, 0)),
				new BlockOccupancy(new Vector3i( 2, 2, 0)),
				new BlockOccupancy(new Vector3i( 3, 2, 0)),

				new BlockOccupancy(new Vector3i(-3, 3, 0)),
				new BlockOccupancy(new Vector3i(-2, 3, 0)),
				new BlockOccupancy(new Vector3i(-1, 3, 0)),
				new BlockOccupancy(new Vector3i( 0, 3, 0)),
				new BlockOccupancy(new Vector3i( 1, 3, 0)),
				new BlockOccupancy(new Vector3i( 2, 3, 0)),
				new BlockOccupancy(new Vector3i( 3, 3, 0)),

				new BlockOccupancy(new Vector3i(-3, 4, 0)),
				new BlockOccupancy(new Vector3i(-2, 4, 0)),
				new BlockOccupancy(new Vector3i(-1, 4, 0)),
				new BlockOccupancy(new Vector3i( 0, 4, 0)),
				new BlockOccupancy(new Vector3i( 1, 4, 0)),
				new BlockOccupancy(new Vector3i( 2, 4, 0)),
				new BlockOccupancy(new Vector3i( 3, 4, 0)),

				new BlockOccupancy(new Vector3i(-3, 5, 0)),
				new BlockOccupancy(new Vector3i(-2, 5, 0)),
				new BlockOccupancy(new Vector3i(-1, 5, 0)),
				new BlockOccupancy(new Vector3i( 0, 5, 0)),
				new BlockOccupancy(new Vector3i( 1, 5, 0)),
				new BlockOccupancy(new Vector3i( 2, 5, 0)),
				new BlockOccupancy(new Vector3i( 3, 5, 0)),

				new BlockOccupancy(new Vector3i(-3, 6, 0)),
				new BlockOccupancy(new Vector3i(-2, 6, 0)),
				new BlockOccupancy(new Vector3i(-1, 6, 0)),
				new BlockOccupancy(new Vector3i( 0, 6, 0)),
				new BlockOccupancy(new Vector3i( 1, 6, 0)),
				new BlockOccupancy(new Vector3i( 2, 6, 0)),
				new BlockOccupancy(new Vector3i( 3, 6, 0)),
            });
		}
    }

    [Serialized]
    [LocDisplayName("Stargate")]
    [Weight(25000)]
    public class StargateItem : WorldObjectItem<StargateObject>
    {
        public override bool ShowLocationsInWorld => false;
        protected override OccupancyContext GetOccupancyContext => new SideAttachedContext( 0  | DirectionAxisFlags.Down , WorldObject.GetOccupancyInfo(this.WorldObjectType));
    }

    [RequiresSkill(typeof(BlacksmithSkill), 6)]
    public class StargateRecipe : RecipeFamily
    {
        public StargateRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                "Stargate",  //noloc
                Localizer.DoStr("Stargate"),
                new List<IngredientElement>
                {
                    new IngredientElement(typeof(RebarItem), 1000),
                    new IngredientElement(typeof(HeatSinkItem), 100),
                    new IngredientElement(typeof(LubricantItem), 100),
                    new IngredientElement(typeof(ChevronItem), 9, true),
                },
                new List<CraftingElement>
                {
                    new CraftingElement<StargateItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 50;
            this.LaborInCalories = CreateLaborInCaloriesValue(1_000_000);
            this.CraftMinutes = CreateCraftTimeValue(2880);
            this.Initialize(Localizer.DoStr("Stargate"), typeof(StargateRecipe));
            CraftingComponent.AddRecipe(typeof(BlacksmithTableObject), this);
        }
    }
}

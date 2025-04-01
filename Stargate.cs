namespace CavRn.Stargate
{
	using Eco.Gameplay.Components;
	using Eco.Mods.TechTree;
	using System.Collections.Generic;
    using Eco.Core.Controller;
    using Eco.Core.Items;
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
    public class StargateObject : WorldObject, IRepresentsItem
    {
        public override LocString DisplayName { get { return Localizer.DoStr("Stargate"); } }
        public override LocString DisplayDescription
        {
            get { return new LocString(this.GetComponent<StargateComponent>()?.OwnAddressIcons ?? ""); }
        }
        public override TableTextureMode TableTexture => TableTextureMode.Stone;
        public virtual Type RepresentedItemType { get { return typeof(StargateItem); } }

		static StargateObject()
		{
			WorldObject.AddOccupancy<StargateObject>(new List<BlockOccupancy>(){
				new BlockOccupancy(new Vector3i(-3, 0, 0)),
				new BlockOccupancy(new Vector3i(-2, 0, 0)),
				new BlockOccupancy(new Vector3i(-1, 0, 0)),
				new BlockOccupancy(new Vector3i( 0, 0, 0)),
				new BlockOccupancy(new Vector3i( 1, 0, 0)),
				new BlockOccupancy(new Vector3i( 2, 0, 0)),
				new BlockOccupancy(new Vector3i( 3, 0, 0)),

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
    [Category("Hidden"), Tag("NotInBrowser"), NoIcon]
    [Weight(35000)]
    public class StargateItem : WorldObjectItem<StargateObject>
    {
        public override bool ShowLocationsInWorld => false;
    }

    [RequiresSkill(typeof(SelfImprovementSkill), 6)]
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
                    new IngredientElement(typeof(RebarItem), 1500, true),
                    new IngredientElement(typeof(HeatSinkItem), 200, true),
                    new IngredientElement(typeof(WhetstoneItem), 3000, true),
                    new IngredientElement(typeof(IronWheelItem), 200, true),
                    new IngredientElement(typeof(LightBulbItem), 100, true),
                    new IngredientElement(typeof(PlasticItem), 500, true),
                    new IngredientElement(typeof(ElevatorCallPostItem), 100, true),
                    new IngredientElement(typeof(CeramicMoldItem), 800, true),
                    new IngredientElement(typeof(SteelSawBladeItem), 200, true),
                    new IngredientElement(typeof(MetalRudderItem), 200, true),
                    new IngredientElement(typeof(LubricantItem), 1000, true),
                    new IngredientElement(typeof(PrintingSuppliesItem), 300, true),
                    new IngredientElement(typeof(ChevronItem), 9, true),
                    new IngredientElement("Glyph", 39, true),
                },
                new List<CraftingElement>
                {
                    new CraftingElement<StargateItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 500;
            this.LaborInCalories = CreateLaborInCaloriesValue(5_000_000);
            this.CraftMinutes = CreateCraftTimeValue(120);
            this.Initialize(Localizer.DoStr("Stargate"), typeof(StargateRecipe));
            CraftingComponent.AddRecipe(typeof(SarcophagusObject), this);
        }
    }
}

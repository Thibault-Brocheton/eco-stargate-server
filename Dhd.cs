namespace CavRn.Stargate
{
    using Eco.Core.Items;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Items.Recipes;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Occupancy;
    using Eco.Gameplay.Skills;
    using Eco.Mods.TechTree;
    using Eco.Shared.Items;
    using Eco.Shared.Localization;
    using Eco.Shared.Math;
    using Eco.Shared.Serialization;
    using System.Collections.Generic;
    using System;

    [Serialized]
    [RequireComponent(typeof(DhdComponent))]
    [RequireComponent(typeof(OccupancyRequirementComponent))]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [Tag("Usable")]
    [Ecopedia("Stargate", "Stargate", subPageName: "Dhd")]
    public class DhdObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(DhdItem);
        public override LocString DisplayName => Localizer.DoStr("Dhd");
        public override TableTextureMode TableTexture => TableTextureMode.Metal;

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
    [LocDescription("Remote control of a Stargate.")]
    [Weight(6000)]
    public class DhdItem : WorldObjectItem<DhdObject>
    {
        public override bool ShowLocationsInWorld => false;
        protected override OccupancyContext GetOccupancyContext => new SideAttachedContext( 0  | DirectionAxisFlags.Down , WorldObject.GetOccupancyInfo(this.WorldObjectType));
    }

    [RequiresSkill(typeof(BlacksmithSkill), 3)]
    public class DhdRecipe : RecipeFamily
    {
        public DhdRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                "Dhd",  //noloc
                Localizer.DoStr("Dhd"),
                new List<IngredientElement>
                {
                    new IngredientElement(typeof(IronBarItem), 200),
                    new IngredientElement(typeof(CopperBarItem), 100),
                    new IngredientElement(typeof(GoldBarItem), 50),
                    new IngredientElement(typeof(GlassItem), 30),
                },
                new List<CraftingElement>
                {
                    new CraftingElement<DhdItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 10;
            this.LaborInCalories = CreateLaborInCaloriesValue(100_000);
            this.CraftMinutes = CreateCraftTimeValue(480);
            this.Initialize(Localizer.DoStr("Dhd"), typeof(DhdRecipe));
            CraftingComponent.AddRecipe(typeof(BlacksmithTableObject), this);
        }
    }
}

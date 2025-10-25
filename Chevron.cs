namespace CavRn.Stargate
{
    using Eco.Core.Items;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Housing.PropertyValues;
    using Eco.Gameplay.Housing;
    using Eco.Gameplay.Items.Recipes;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Occupancy;
    using Eco.Gameplay.Skills;
    using Eco.Gameplay.Systems.TextLinks;
    using Eco.Mods.TechTree;
    using Eco.Shared.Items;
    using Eco.Shared.Localization;
    using Eco.Shared.Math;
    using Eco.Shared.Serialization;
    using System.Collections.Generic;
    using System;

    [Serialized]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(HousingComponent))]
    [RequireComponent(typeof(ForSaleComponent))]
    [Tag("Usable")]
    public class ChevronObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(ChevronItem);
        public override LocString DisplayName => Localizer.DoStr("Chevron");
        public override TableTextureMode TableTexture => TableTextureMode.Metal;

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
    [Weight(2000)]
    public class ChevronItem : WorldObjectItem<ChevronObject>
    {
        protected override OccupancyContext GetOccupancyContext => new SideAttachedContext( 0  | DirectionAxisFlags.Down | DirectionAxisFlags.Backward | DirectionAxisFlags.Forward, WorldObject.GetOccupancyInfo(this.WorldObjectType));

        public override HomeFurnishingValue HomeValue => homeValue;
        public static readonly HomeFurnishingValue homeValue = new HomeFurnishingValue()
        {
            ObjectName                              = typeof(ChevronObject).UILink(),
            Category                                = HousingConfig.GetRoomCategory("Decoration"),
            BaseValue                               = 8,
            TypeForRoomLimit                        = Localizer.DoStr("Artifact"),
            DiminishingReturnMultiplier             = 0.1f
        };
    }

    [RequiresSkill(typeof(AdvancedSmeltingSkill), 6)]
    public class ChevronRecipe : RecipeFamily
    {
        public ChevronRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                "Chevron",  //noloc
                Localizer.DoStr("Chevron"),
                new List<IngredientElement>
                {
                    new IngredientElement(typeof(RebarItem), 200),
                    new IngredientElement(typeof(CopperWiringItem), 50),
                    new IngredientElement(typeof(LubricantItem), 20),
                    new IngredientElement(typeof(LightBulbItem), 20),
                },
                new List<CraftingElement>
                {
                    new CraftingElement<ChevronItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 5;
            this.LaborInCalories = CreateLaborInCaloriesValue(100_000);
            this.CraftMinutes = CreateCraftTimeValue(60);
            this.Initialize(Localizer.DoStr("Chevron"), typeof(ChevronRecipe));
            CraftingComponent.AddRecipe(typeof(BlastFurnaceObject), this);
        }
    }
}

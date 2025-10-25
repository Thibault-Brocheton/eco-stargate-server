namespace CavRn.Stargate
{
    using Eco.Core.Plugins.Interfaces;
    using Eco.Core.Plugins;
    using Eco.Core.Utils;
    using Eco.Gameplay;
    using Eco.Shared.Logging;
    using Eco.Shared.Math;
    using Eco.Shared.Utils;
    using Eco.World.Blocks;
    using Eco.World;
    using Eco.WorldGenerator;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System;

    public class StargateMod: IModInit
    {
        public static ModRegistration Register() => new()
        {
            ModName = "Stargate",
            ModDescription = "Discover or craft a Stargate to travel fast through the map.",
            ModDisplayName = "Stargate"
        };
    }

    public class StargateConfig: Singleton<StargateConfig>
    {
        public int NumberToSpawnAtWorldCreation { get; set; } = 3;
    }

    public class StargatePlugin: Singleton<StargatePlugin>, IModKitPlugin, IConfigurablePlugin
    {
        public static ThreadSafeAction OnSettingsChanged = new();
        public IPluginConfig PluginConfig => this.config;
        private readonly PluginConfig<StargateConfig> config;
        public StargateConfig Config => this.config.Config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new();

        public StargatePlugin()
        {
            this.config = new PluginConfig<StargateConfig>("Stargate");
            this.SaveConfig();
        }

        public string GetStatus()
        {
            return "OK";
        }

        public string GetCategory()
        {
            return "Mods";
        }

        public object GetEditObject() => this.config.Config;
        public void OnEditObjectChanged(object o, string param) { this.SaveConfig(); }
    }

    public class StargateGenerator : IWorldGenFeature
    {
        public static void GenerateStargate(Vector3i location)
        {
            Log.WriteLineLoc($"Generate Stargate at position {location}");

            var wo = WorldObjectDebugUtil.Spawn("StargateObject", null, location);
            wo.Rotation = Eco.Shared.Math.Quaternion.LookRotation(Vector3i.Back);

            var floor = BlockManager.FromTypeName("AdobeFloorBlock");

            var stairs = BlockManager.FromTypeName("AdobeStairsMidBlock");
            var stairs90 = BlockManager.FromTypeName("AdobeStairsMid90Block");
            var stairs180 = BlockManager.FromTypeName("AdobeStairsMid180Block");
            var stairs270 = BlockManager.FromTypeName("AdobeStairsMid270Block");

            var stairsCorner = BlockManager.FromTypeName("AdobeStairsCornerBlock");
            var stairsCorner90 = BlockManager.FromTypeName("AdobeStairsCorner90Block");
            var stairsCorner180 = BlockManager.FromTypeName("AdobeStairsCorner180Block");
            var stairsCorner270 = BlockManager.FromTypeName("AdobeStairsCorner270Block");

            var stairsTurn = BlockManager.FromTypeName("AdobeStairsTurnBlock");
            var stairsTurn90 = BlockManager.FromTypeName("AdobeStairsTurn90Block");
            var stairsTurn180 = BlockManager.FromTypeName("AdobeStairsTurn180Block");
            var stairsTurn270 = BlockManager.FromTypeName("AdobeStairsTurn270Block");

            World.SetBlock(stairsCorner180, location + Vector3i.Back + Vector3i.Left * 3);
            World.SetBlock(stairs90       , location + Vector3i.Back + Vector3i.Left * 2);
            World.SetBlock(stairs90       , location + Vector3i.Back + Vector3i.Left * 1);
            World.SetBlock(stairs90       , location + Vector3i.Back);
            World.SetBlock(stairs90       , location + Vector3i.Back + Vector3i.Right * 1);
            World.SetBlock(stairs90       , location + Vector3i.Back + Vector3i.Right * 2);
            World.SetBlock(stairsCorner90 , location + Vector3i.Back + Vector3i.Right * 3);

            World.SetBlock(stairs180, location + Vector3i.Left * 3);
            World.SetBlock(floor    , location + Vector3i.Left * 2);
            World.SetBlock(floor    , location + Vector3i.Left * 1);
            World.SetBlock(floor    , location);
            World.SetBlock(floor    , location + Vector3i.Right * 1);
            World.SetBlock(floor    , location + Vector3i.Right * 2);
            World.SetBlock(stairs   , location + Vector3i.Right * 3);

            World.SetBlock(stairsCorner270, location + Vector3i.Forward + Vector3i.Left * 3);
            World.SetBlock(stairsTurn270  , location + Vector3i.Forward + Vector3i.Left * 2);
            World.SetBlock(floor          , location + Vector3i.Forward + Vector3i.Left * 1);
            World.SetBlock(floor          , location + Vector3i.Forward);
            World.SetBlock(floor          , location + Vector3i.Forward + Vector3i.Right * 1);
            World.SetBlock(stairsTurn     , location + Vector3i.Forward + Vector3i.Right * 2);
            World.SetBlock(stairsCorner   , location + Vector3i.Forward + Vector3i.Right * 3);

            World.SetBlock(stairs180, location + Vector3i.Forward * 2 + Vector3i.Left * 2);
            World.SetBlock(floor    , location + Vector3i.Forward * 2 + Vector3i.Left * 1);
            World.SetBlock(floor    , location + Vector3i.Forward * 2);
            World.SetBlock(floor    , location + Vector3i.Forward * 2 + Vector3i.Right * 1);
            World.SetBlock(stairs   , location + Vector3i.Forward * 2 + Vector3i.Right * 2);

            World.SetBlock(stairs180, location + Vector3i.Forward * 3 + Vector3i.Left * 2);
            World.SetBlock(floor    , location + Vector3i.Forward * 3 + Vector3i.Left * 1);
            World.SetBlock(floor    , location + Vector3i.Forward * 3);
            World.SetBlock(floor    , location + Vector3i.Forward * 3 + Vector3i.Right * 1);
            World.SetBlock(stairs   , location + Vector3i.Forward * 3 + Vector3i.Right * 2);

            World.SetBlock(stairs180, location + Vector3i.Forward * 4 + Vector3i.Left * 2);
            World.SetBlock(floor    , location + Vector3i.Forward * 4 + Vector3i.Left * 1);
            World.SetBlock(floor    , location + Vector3i.Forward * 4);
            World.SetBlock(floor    , location + Vector3i.Forward * 4 + Vector3i.Right * 1);
            World.SetBlock(stairs   , location + Vector3i.Forward * 4 + Vector3i.Right * 2);

            World.SetBlock(stairs180, location + Vector3i.Forward * 5 + Vector3i.Left * 2);
            World.SetBlock(floor    , location + Vector3i.Forward * 5 + Vector3i.Left * 1);
            World.SetBlock(floor    , location + Vector3i.Forward * 5);
            World.SetBlock(floor    , location + Vector3i.Forward * 5 + Vector3i.Right * 1);
            World.SetBlock(stairs   , location + Vector3i.Forward * 5 + Vector3i.Right * 2);

            World.SetBlock(stairsCorner270, location + Vector3i.Forward * 6 + Vector3i.Left * 2);
            World.SetBlock(stairs270      , location + Vector3i.Forward * 6 + Vector3i.Left * 1);
            World.SetBlock(stairs270      , location + Vector3i.Forward * 6);
            World.SetBlock(stairs270      , location + Vector3i.Forward * 6 + Vector3i.Right * 1);
            World.SetBlock(stairsCorner   , location + Vector3i.Forward * 6 + Vector3i.Right * 2);
        }

        public void Generate(Random seed, Vector3 voxelSize, WorldSettings settings)
        {
            List<Vector3i> previousLocations = new List<Vector3i>();

            for (var i = 0; i < StargatePlugin.Obj.Config.NumberToSpawnAtWorldCreation; ++i)
            {
                List<Vector3i> locations = new List<Vector3i>();
                while (locations.Count < 20)
                {
                    var potentialPosition = (Vector3i)WrappedWorldPosition3i.Clamp((WrappedPosition3i)World.GetRandomLandPos());

                    var allPositions = new List<Vector3i>()
                    {
                        potentialPosition + Vector3i.Back + Vector3i.Left * 3,
                        potentialPosition + Vector3i.Back + Vector3i.Left * 2,
                        potentialPosition + Vector3i.Back + Vector3i.Left * 1,
                        potentialPosition + Vector3i.Back,
                        potentialPosition + Vector3i.Back + Vector3i.Right * 1,
                        potentialPosition + Vector3i.Back + Vector3i.Right * 2,
                        potentialPosition + Vector3i.Back + Vector3i.Right * 3,

                        potentialPosition + Vector3i.Left * 3,
                        potentialPosition + Vector3i.Left * 2,
                        potentialPosition + Vector3i.Left * 1,
                        potentialPosition,
                        potentialPosition + Vector3i.Right * 1,
                        potentialPosition + Vector3i.Right * 2,
                        potentialPosition + Vector3i.Right * 3,

                        potentialPosition + Vector3i.Forward + Vector3i.Left * 3,
                        potentialPosition + Vector3i.Forward + Vector3i.Left * 2,
                        potentialPosition + Vector3i.Forward + Vector3i.Left * 1,
                        potentialPosition + Vector3i.Forward,
                        potentialPosition + Vector3i.Forward + Vector3i.Right * 1,
                        potentialPosition + Vector3i.Forward + Vector3i.Right * 2,
                        potentialPosition + Vector3i.Forward + Vector3i.Right * 3,

                        potentialPosition + Vector3i.Forward * 2 + Vector3i.Left * 2,
                        potentialPosition + Vector3i.Forward * 2 + Vector3i.Left * 1,
                        potentialPosition + Vector3i.Forward * 2,
                        potentialPosition + Vector3i.Forward * 2 + Vector3i.Right * 1,
                        potentialPosition + Vector3i.Forward * 2 + Vector3i.Right * 2,

                        potentialPosition + Vector3i.Forward * 3 + Vector3i.Left * 2,
                        potentialPosition + Vector3i.Forward * 3 + Vector3i.Left * 1,
                        potentialPosition + Vector3i.Forward * 3,
                        potentialPosition + Vector3i.Forward * 3 + Vector3i.Right * 1,
                        potentialPosition + Vector3i.Forward * 3 + Vector3i.Right * 2,

                        potentialPosition + Vector3i.Forward * 4 + Vector3i.Left * 2,
                        potentialPosition + Vector3i.Forward * 4 + Vector3i.Left * 1,
                        potentialPosition + Vector3i.Forward * 4,
                        potentialPosition + Vector3i.Forward * 4 + Vector3i.Right * 1,
                        potentialPosition + Vector3i.Forward * 4 + Vector3i.Right * 2,

                        potentialPosition + Vector3i.Forward * 5 + Vector3i.Left * 2,
                        potentialPosition + Vector3i.Forward * 5 + Vector3i.Left * 1,
                        potentialPosition + Vector3i.Forward * 5,
                        potentialPosition + Vector3i.Forward * 5 + Vector3i.Right * 1,
                        potentialPosition + Vector3i.Forward * 5 + Vector3i.Right * 2,

                        potentialPosition + Vector3i.Forward * 6 + Vector3i.Left * 2,
                        potentialPosition + Vector3i.Forward * 6 + Vector3i.Left * 1,
                        potentialPosition + Vector3i.Forward * 6,
                        potentialPosition + Vector3i.Forward * 6 + Vector3i.Right * 1,
                        potentialPosition + Vector3i.Forward * 6 + Vector3i.Right * 2,
                    };

                    if (allPositions.Any(p => World.GetBlock(p) == Block.Empty || World.GetBlock(p) is WaterBlock || World.GetBlock(p + Vector3i.Up) != Block.Empty)) { continue; }

                    locations.Add(potentialPosition);
                }

                var location = (previousLocations.Count == 0
                    ? locations.First()
                    : locations.MaxBy(l => previousLocations.Min(m => Vector3i.Distance(m, l)))) + Vector3i.Up;

                GenerateStargate(location);

                previousLocations.Add(location);
            }
        }
    }
}

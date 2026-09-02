namespace CavRn.Stargate
{
    using System.Collections.Generic;
    using Eco.Gameplay.Objects;

    //Where a vehicle lands behind the destination gate: the server knows no vehicle footprint (their occupancy is empty), so the rear overhang of each vanilla prefab
    //(collider extent behind the pivot, along -Z) is tabulated here and the pivot is pushed back so the tail clears the ring like a traveller on foot does.
    public static class VehicleArrival
    {
        const float FootClearance     = 1f;   //Travellers on foot appear this far behind the gate; a vehicle's tail gets the same clearance.
        const float DefaultRearExtent = 1f;   //Unknown (modded) vehicles: generous enough for a cart, keeps trucks with a rear pivot close.

        static readonly Dictionary<string, float> RearExtents = new()
        {
            ["WheelbarrowObject"]           = 0.91f,
            ["HandPlowObject"]              = 1.48f,
            ["SmallWoodCartObject"]         = 0.63f,
            ["WoodCartObject"]              = 0.56f,
            ["WoodShopCartObject"]          = 0.54f,
            ["PoweredCartObject"]           = 0f,
            ["SteamTractorObject"]          = 0.57f,
            ["SteamTruckObject"]            = 0.84f,
            ["TruckObject"]                 = 0.12f,
            ["TrailerTruckObject"]          = 0.49f,
            ["SkidSteerObject"]             = 0.56f,
            ["ExcavatorObject"]             = 0.55f,
            ["ScorpionObject"]              = 0.48f,
            ["CraneObject"]                 = 4.01f,
            ["WoodenElevatorObject"]        = 0.76f,
            ["IndustrialElevatorObject"]    = 11.5f,
            ["SmallCanoeObject"]            = 1.36f,
            ["LargeCanoeObject"]            = 1.75f,
            ["SmallWoodenBoatObject"]       = 2.51f,
            ["WoodenTransportShipObject"]   = 3.5f,
            ["WoodenBargeObject"]           = 0.07f,
            ["MediumFishingTrawlerObject"]  = 8.88f,
            ["IndustrialBargeObject"]       = 15.32f,
        };

        /// <summary>Distance behind the gate at which to place the pivot of <paramref name="vehicle"/> so its tail sits one foot-traveller's clearance past the ring.</summary>
        public static float DistanceBehindGate(WorldObject vehicle) => FootClearance + (RearExtents.TryGetValue(vehicle.GetType().Name, out var rear) ? rear : DefaultRearExtent);

        public static float FootDistanceBehindGate => FootClearance;
    }
}

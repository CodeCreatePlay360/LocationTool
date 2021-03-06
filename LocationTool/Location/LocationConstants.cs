using UnityEngine;


namespace CodeCreatePlay
{
    namespace LocationTool
    {
        [System.Serializable]
        public enum LocationCategory
        {
            None,
            Town,
            Travern,
            Woods,
            Inn,
            PointOfInterest,
            Bakery,
            Home,
            Area,
            WoodPiles,
            RedZone,
            RainShelter,
            BaseCamp,
            CampFire,
            Farm,
            Mill,
            Garden,
            Forest,
        }

        public enum GoTo
        {
            FixedLocation_And_FixedDestination,
            FixedLocation_And_RandomDestination,
            RandomLocation_And_FixedDestination,
            RandomLocation_And_RandomDestination,
            RandomLocationInLocation_And_RandomDestination,
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deimos
{
    internal class Stuff
    {
        public static void Run()
        {
            foreach (var cl in Enum.GetValues<UnitClass>())
            {
                Console.WriteLine($"#define UNITCLASS_{cl.ToString().ToUpperInvariant()} {(int)cl}");
            }
        }

        private enum UnitClass
        {
            Archer = 900,
            Artifact,
            TradeBoat,
            Building,
            Civilian,
            OceanFish,
            Infantry,
            BerryBush,
            StoneMine,
            PreyAnimal,
            PredatorAnimal,
            Miscellaneous,
            Cavalry,
            SiegeWeapon,
            Terrain,
            Tree,
            TreeStump,
            Healer,
            Monk,
            TradeCart,
            TransportBoat,
            FishingBoat,
            Warship,
            Conquistador,
            WarElephant,
            Hero,
            ElephantArcher,
            Wall,
            Phalanx,
            DomesticAnimal,
            Flag,
            DeepSeaFish,
            GoldMine,
            ShoreFish,
            Cliff,
            Petard,
            CavalryArcher,
            Doppelganger,
            Bird,
            Gate,
            SalvagePile,
            ResourcePile,
            Relic,
            MonkWithRelic,
            HandCannoneer,
            TwoHandedSwordsMan,
            Pikeman,
            Scout,
            OreMine,
            Farm,
            Spearman,
            PackedUnit,
            Tower,
            BoardingBoat,
            UnpackedSiegeUnit,
            Ballista,
            Raider,
            CavalryRaider,
            Livestock,
            King,
            MiscBuilding,
            ControlledAnimal
        }
    }
}

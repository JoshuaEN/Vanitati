using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public class DestroyedAirfield : DestroyedLandStructure
    {
        private DestroyedAirfield() : base("destroyed_airfield") { }
        public static DestroyedAirfield Instance { get; } = new DestroyedAirfield();
    }
}

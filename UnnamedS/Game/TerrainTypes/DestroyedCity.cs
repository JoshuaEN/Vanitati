using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public class DestroyedCity : DestroyedLandStructure
    {
        private DestroyedCity() : base("destroyed_city") { }
        public static DestroyedCity Instance { get; } = new DestroyedCity();
    }
}

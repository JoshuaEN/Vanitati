using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public class DestroyedRunway : DestroyedLandStructure
    {
        private DestroyedRunway() : base("destroyed_runway") { }
        public static DestroyedRunway Instance { get; } = new DestroyedRunway();
    }
}

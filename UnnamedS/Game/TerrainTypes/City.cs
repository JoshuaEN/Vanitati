using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public sealed class City : LandStructure
    {
        private City() : base("city") { }
        public static City Instance { get; } = new City();
    }
}

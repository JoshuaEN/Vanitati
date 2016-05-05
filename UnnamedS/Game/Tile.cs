using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    public class Tile
    {
        public Unit Unit { get; }
        public Terrain Terrain { get; }
        public Location Location
        {
            get { return Terrain.Location; }
        }

        public Tile(Terrain terrain, Unit unit)
        {
            Contract.Requires<ArgumentNullException>(null != terrain);

            Terrain = terrain;
            Unit = unit;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Terrain);
        }
    }
}

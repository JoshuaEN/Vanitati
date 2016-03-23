using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    public class TerrainStateChange : StateChange
    {
        public Location ChangedTerrainLocation { get; }
        public TerrainStateChange(Location changedTerrainLocation, List<IAttribute> updatedAttributes) : base(updatedAttributes)
        {
            ChangedTerrainLocation = changedTerrainLocation;
        }
    }
}

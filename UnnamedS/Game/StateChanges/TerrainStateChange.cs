using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    public class TerrainStateChange : UpdateStateChange
    {
        public Location ChangedTerrainLocation { get; }
        public TerrainStateChange(Location changedTerrainLocation, IDictionary<string, object> updatedProperties) : base(updatedProperties)
        {
            ChangedTerrainLocation = changedTerrainLocation;
        }
    }
}

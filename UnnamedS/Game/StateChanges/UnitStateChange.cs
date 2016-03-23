using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    public class UnitStateChange : StateChange
    {
        public Location ChangedUnitLocation { get; }
        public UnitStateChange(Location changedUnitLocation, List<IAttribute> updatedAttributes) : base(updatedAttributes)
        {
            ChangedUnitLocation = changedUnitLocation;
        }
    }
}

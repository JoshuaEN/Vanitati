using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    public class UnitStateChange : StateChange
    {
        public int UnitId { get; }
        public Cause ChangeCause { get; }
        public UnitStateChange(int unitId, List<IAttribute> updatedAttributes, Cause cause = Cause.Changed) : base(updatedAttributes)
        {
            UnitId = unitId;
            ChangeCause = cause;
        }

        public enum Cause { Created, Destroyed, Changed }
    }
}

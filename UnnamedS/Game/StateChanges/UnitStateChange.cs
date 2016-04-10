using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    public class UnitStateChange : UpdateStateChange
    {
        public int UnitID { get; }
        public Cause ChangeCause { get; }
        public Location Location { get; }
        public Location PreviousLocation { get; }
        public bool LocationChanged
        {
            get { return Location != PreviousLocation; }
        }

        public UnitStateChange(int unitID, IDictionary<string, object> updatedProperties, Location location, Cause changeCause = Cause.Changed, Location previousLocation = null) : base(updatedProperties)
        {
            UnitID = unitID;
            ChangeCause = changeCause;
            Location = location;
            PreviousLocation = previousLocation ?? location;
        }

        public enum Cause { Created, Destroyed, Changed }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
            Contract.Requires<ArgumentNullException>(null != location);

            UnitID = unitID;
            ChangeCause = changeCause;
            Location = location;
            PreviousLocation = previousLocation ?? location;
        }

        public enum Cause { Created, Destroyed, Changed, Added, Removed }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Location);
            Contract.Invariant(null != PreviousLocation);
        }
    }
}

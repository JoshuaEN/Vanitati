using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Event
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class UserAddedEventArgs : EventArgs
    {
        public User User { get; }

        public UserAddedEventArgs(User user)
        {
            Contract.Requires<ArgumentNullException>(null != user);
            User = user;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != User);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ActionInfo
    {
        public ActionType Type { get; }
        public ActionContext Context { get; }

        public ActionInfo(ActionType type, ActionContext context)
        {
            Contract.Requires<ArgumentNullException>(null != type);
            Contract.Requires<ArgumentNullException>(null != context);

            Type = type;
            Context = context;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Type);
            Contract.Invariant(null != Context);
        }
    }
}

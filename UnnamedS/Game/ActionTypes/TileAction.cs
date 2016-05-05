using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes
{
    [ContractClass(typeof(ContractClassForTileAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class TileAction : ActionType
    {
        public TileAction(string key) : base(key) { }
    }

    [ContractClassFor(typeof(TileAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForTileAction : TileAction
    {
        private ContractClassForTileAction() : base(null) { }

    }
}

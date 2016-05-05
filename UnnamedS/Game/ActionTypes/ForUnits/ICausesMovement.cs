using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    /// <summary>
    /// Required interface for any Action that can cause the source unit to move.
    /// </summary>
    [ContractClass(typeof(ContractClassForICausesMovement))]
    public interface ICausesMovement
    {
        IReadOnlyDictionary<Location, Move.MovementRemaining> GetRemainingMovement(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile);
    }

    [ContractClassFor(typeof(ICausesMovement))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForICausesMovement : ICausesMovement
    {
        public IReadOnlyDictionary<Location, Move.MovementRemaining> GetRemainingMovement(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Ensures(Contract.Result<IReadOnlyDictionary<Location, Move.MovementRemaining>>() != null);

            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes
{
    public sealed class ReplenishUnitTurnResources : ActionType
    {
        public ReplenishUnitTurnResources() : base("replenish_unit_turn_resources", ActionTarget.Self, ActionTriggers.TurnStart, false) { }
        public static ReplenishUnitTurnResources Instance { get; } = new ReplenishUnitTurnResources();

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context, Tile sourceTile, Tile targetTile)
        {
            var changes = new List<StateChange>();

            changes.Add(
                new StateChanges.UnitStateChange(
                    sourceTile.Unit.UnitID,
                    new Dictionary<string, object>()
                    {
                        {"Attacks", sourceTile.Unit.UnitType.MaxAttacks },
                        {"Movement", sourceTile.Unit.UnitType.MaxMovement }
                    },
                    sourceTile.Location
                )
            );

            return changes;
        }

        public override IReadOnlyDictionary<Location, ActionChain> ActionableLocations(IReadOnlyBattleGameState state, ActionContext context, Tile sourceTile)
        {
            throw new NotSupportedException();
        }
    }
}

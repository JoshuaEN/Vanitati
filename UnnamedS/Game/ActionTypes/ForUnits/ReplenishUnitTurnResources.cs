using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class ReplenishUnitTurnResources : UnitTargetOtherAction
    {
        public override ActionTriggers Triggers { get; } = ActionTriggers.OnTurnStart;

        public ReplenishUnitTurnResources() : base("replenish_unit_turn_resources") { }
        public static ReplenishUnitTurnResources Instance { get; } = new ReplenishUnitTurnResources();

        public override bool CanPerformOn(IReadOnlyBattleGameState state, UnitTargetOtherContext context, Tile sourceTile)
        {
            return true;
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, UnitTargetOtherContext context, Tile sourceTile)
        {
            var changes = new List<StateChange>();

            changes.Add(
                new StateChanges.UnitStateChange(
                    sourceTile.Unit.UnitID,
                    new Dictionary<string, object>()
                    {
                        {"Actions", sourceTile.Unit.UnitType.MaxActions },
                        {"Movement", sourceTile.Unit.UnitType.MaxMovement }
                    },
                    sourceTile.Location
                )
            );

            return changes;
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, UnitTargetOtherContext context, Tile sourceTile)
        {
            return new List<Modifier>()
            {
                new Modifier("replenished_actions", sourceTile.Unit.UnitType.MaxActions),
                new Modifier("replenished_movement", sourceTile.Unit.UnitType.MaxMovement)
            };
        }
    }
}

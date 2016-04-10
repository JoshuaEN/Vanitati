using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes
{
    public sealed class AttackRifle : AttackBase
    {
        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, Action.ActionContext context, Tile sourceTile, Tile targetTile)
        {
            return new List<StateChange>()
            {
                new StateChanges.UnitStateChange(targetTile.Unit.UnitID, new Dictionary<string, object>(0), targetTile.Location, StateChanges.UnitStateChange.Cause.Destroyed)
            };
        }

        private AttackRifle() : 
            base(
                "rifle",
                targetableMovementTypes:
                    new HashSet<MovementType>()
                    {
                        MovementTypes.Boots.Instance
                    },
                maximumRange: 1,
                suppliesNeeded:
                    new Dictionary<SupplyType, int>()
                    {
                        { SupplyTypes.RifleRounds.Instance, 1 }
                    }
            )
        { }

        public static AttackRifle Instance { get; } = new AttackRifle();
    }
}

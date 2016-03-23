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
        public override bool CanPerformOn(BattleGameState state, Action.ActionContext context, Tile sourceTile, Tile targetTile = null)
        {
            if (targetTile != null)
                return true;
            else
                return false;
        }

        public override IReadOnlyList<StateChange> PerformOn(BattleGameState state, Action.ActionContext context, Tile sourceTile, Tile targetTile = null)
        {
            return new List<StateChange>();
        }

        private AttackRifle(string key) : base(key) { }

        public static AttackRifle Instance { get; } = new AttackRifle("rifle");

        static AttackRifle()
        {
            Instance = new AttackRifle("attack_rifle");
        }
    }
}

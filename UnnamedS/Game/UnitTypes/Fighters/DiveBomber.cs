using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public sealed class DiveBomber : FighterChassis
    {
        public override int MaxMovement { get; }
        public override int BuildCost { get; } = 26000;

        public override IReadOnlyList<UnitAction> Actions { get; } = new List<UnitAction>()
        {
            ActionTypes.ForUnits.Move.Instance,
            ActionTypes.ForUnits.AttackDiveBomb.Instance
        }.Concat(DEFAULT_ACTIONS).ToList();

        public override IReadOnlyDictionary<SupplyType, int> SupplyLimits { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Bombs.Instance, 4 },
            { SupplyTypes.Kerosene.Instance, 70 }
        };

        private DiveBomber() : base("dive_bomber")
        {
            MaxMovement = base.MaxMovement - 1;
        }
        public static DiveBomber Instance { get; } = new DiveBomber();
    }
}

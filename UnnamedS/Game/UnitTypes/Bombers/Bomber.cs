using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public sealed class Bomber : BomberChassis
    {
        public override int BuildCost { get; } = 28000;

        public override IReadOnlyList<UnitAction> Actions { get; } = new List<UnitAction>()
        {
            ActionTypes.ForUnits.Move.Instance,
            ActionTypes.ForUnits.AttackSaturationBombing.Instance
        }.Concat(DEFAULT_ACTIONS).ToList();

        public override IReadOnlyDictionary<SupplyType, int> SupplyLimits { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Bombs.Instance, 1 },
            { SupplyTypes.Kerosene.Instance, 120 }
        };

        private Bomber() : base("bomber") { }
        public static Bomber Instance { get; } = new Bomber();
    }
}

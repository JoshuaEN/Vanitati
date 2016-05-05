using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public sealed class Fighter : FighterChassis
    {
        public override int BuildCost { get; } = 24000;

        public override IReadOnlyList<UnitAction> Actions { get; } = new List<UnitAction>()
        {
            ActionTypes.ForUnits.Move.Instance,
            ActionTypes.ForUnits.AttackFighterHeavyMachineGun.Instance
        }.Concat(DEFAULT_ACTIONS).ToList();

        public override IReadOnlyDictionary<SupplyType, int> SupplyLimits { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Bullets.Instance, 4 },
            { SupplyTypes.Kerosene.Instance, 70 }
        };

        private Fighter() : base("fighter") { }
        public static Fighter Instance { get; } = new Fighter();
    }
}

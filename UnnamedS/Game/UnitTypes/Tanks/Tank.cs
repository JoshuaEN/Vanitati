using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public sealed class Tank : TankChassis
    {
        public override int BuildCost { get; } = 14000;

        public override IReadOnlyList<UnitAction> Actions { get; } = new List<UnitAction>()
        {
            ActionTypes.ForUnits.Move.Instance,
            ActionTypes.ForUnits.AttackTankMainGun.Instance,
            ActionTypes.ForUnits.AttackHeavyMachineGun.Instance,
            ActionTypes.ForUnits.DigIn.Instance
        }.Concat(DEFAULT_ACTIONS).ToList();

        public override IReadOnlyDictionary<SupplyType, int> SupplyLimits { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Shells.Instance, 7 },
            { SupplyTypes.Bullets.Instance, 20 },
            { SupplyTypes.Diesel.Instance, 40 },
            { SupplyTypes.Rations.Instance, 60 }
        };

        private Tank() : base("tank") { }
        public static Tank Instance { get; } = new Tank();
    }
}

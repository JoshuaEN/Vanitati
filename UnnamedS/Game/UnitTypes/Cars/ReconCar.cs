using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public sealed class ReconCar : CarChassis
    {
        public override int BuildCost { get; } = 6000;

        public override IReadOnlyList<UnitAction> Actions { get; } = new List<UnitAction>()
        {
            ActionTypes.ForUnits.Move.Instance,
            ActionTypes.ForUnits.AttackLightMachineGun.Instance,
            ActionTypes.ForUnits.DigIn.Instance
        }.Concat(DEFAULT_ACTIONS).ToList();

        public override IReadOnlyDictionary<SupplyType, int> SupplyLimits { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Bullets.Instance, 12 },
            { SupplyTypes.Diesel.Instance, 80 },
            { SupplyTypes.Rations.Instance, 40 }
        };

        private ReconCar() : base("recon_car") { }
        public static ReconCar Instance { get; } = new ReconCar();
    }
}

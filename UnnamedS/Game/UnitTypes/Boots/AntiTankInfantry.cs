using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public sealed class AntiTankInfantry : BootsChassis
    {
        public override int MaxMovement { get; }

        public override int Concealment { get; }

        public override int BuildCost { get; } = 3000;

        public override IReadOnlyList<UnitAction> Actions { get; } = new List<UnitAction>()
        {
            ActionTypes.ForUnits.Move.Instance,
            ActionTypes.ForUnits.AttackAntiTankRocket.Instance,
            ActionTypes.ForUnits.AttackRifle.Instance,
            ActionTypes.ForUnits.Capture.Instance,
            ActionTypes.ForUnits.DigIn.Instance
        }.Concat(DEFAULT_ACTIONS).ToList();

        public override IReadOnlyDictionary<SupplyType, int> SupplyLimits { get; } = new Dictionary<SupplyType, int>()
        {
            {SupplyTypes.Rations.Instance, 20 },
            {SupplyTypes.Rockets.Instance, 4 },
            {SupplyTypes.Bullets.Instance, 8 }
        };

        private AntiTankInfantry() : base("anti_tank_infantry")
        {
            Concealment = base.Concealment;
            MaxMovement = base.MaxMovement - 1;
        }
        public static AntiTankInfantry Instance { get; } = new AntiTankInfantry();
    }
}

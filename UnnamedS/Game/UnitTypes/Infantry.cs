using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public sealed class Infantry : UnitType
    {
        private Infantry() : base(
            key:
                "infantry",

            movementType:
                MovementTypes.Boots.Instance,

            maxMovement:
                3,

            actions:
                new List<ActionType>()
                {
                    ActionTypes.AttackRifle.Instance
                },

            supplyLimits:
                new Dictionary<SupplyType, int>()
                {
                    {SupplyTypes.Rations.Instance, 14 },
                    {SupplyTypes.RifleRounds.Instance, 10 }
                }
        )
        { }

        public static Infantry Instance { get; } = new Infantry();
    }
}

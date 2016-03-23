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

            attributeDefaults:
                new Dictionary<string, object>()
                {
                    { MOVEMENT, 2 },
                },

            movementType:
                MovementTypes.Boots.Instance,

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
        { /*SetDefaultType(this);*/ }

        public static Infantry Instance { get; } = new Infantry();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.SupplyTypes
{
    public sealed class RifleRounds : SupplyType
    {
        private RifleRounds() : base("rifle_rounds") { }
        public static RifleRounds Instance { get; } = new RifleRounds();
    }
}

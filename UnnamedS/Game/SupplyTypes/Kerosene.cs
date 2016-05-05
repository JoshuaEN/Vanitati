using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.SupplyTypes
{
    public sealed class Kerosene : SupplyType
    {
        public override bool CriticalResource { get; } = true;

        private Kerosene() : base("kerosene") { }
        public static Kerosene Instance { get; } = new Kerosene();
    }
}

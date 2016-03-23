using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.SupplyTypes
{
    public sealed class Rations : SupplyType
    {
        private Rations() : base("rations") { }
        public static Rations Instance { get; } = new Rations();
    }
}

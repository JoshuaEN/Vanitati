using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.SupplyTypes
{
    public sealed class Bullets : SupplyType
    {
        private Bullets() : base("bullets") { }
        public static Bullets Instance { get; } = new Bullets();
    }
}

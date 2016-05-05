using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.SupplyTypes
{
    public sealed class Diesel : SupplyType
    {
        private Diesel() : base("diesel") { }
        public static Diesel Instance { get; } = new Diesel();
    }
}

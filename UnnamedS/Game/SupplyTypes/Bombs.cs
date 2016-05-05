using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.SupplyTypes
{
    public sealed class Bombs : SupplyType
    {
        private Bombs() : base("bombs") { }
        public static Bombs Instance { get; } = new Bombs();
    }
}

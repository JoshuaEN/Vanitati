using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    public abstract class SupplyType : BaseType
    {
        protected SupplyType(string key) : base("supply_" + key) { }

        public static IReadOnlyDictionary<string, SupplyType> TYPES { get; }

        static SupplyType()
        {
            TYPES = BuildTypeListing<SupplyType>("UnnamedStrategyGame.Game.SupplyTypes");
        }
    }
}

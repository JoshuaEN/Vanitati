using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class SupplyType : BaseType
    {
        protected SupplyType(string key) : base("supply_" + key) { }

        /// <summary>
        /// Critical resources automatically destroy the unit if they reach zero.
        /// </summary>
        public virtual bool CriticalResource { get; } = false;

        public static IReadOnlyDictionary<string, SupplyType> TYPES { get; }

        static SupplyType()
        {
            TYPES = BuildTypeListing<SupplyType>("UnnamedStrategyGame.Game.SupplyTypes");
        }
    }
}

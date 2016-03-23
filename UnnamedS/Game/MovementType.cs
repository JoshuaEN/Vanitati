using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    public abstract class MovementType : BaseType
    {
        protected MovementType(string key) : base("movement_" + key) { }

        public static IReadOnlyDictionary<string, MovementType> TYPES { get; }

        static MovementType()
        {
            TYPES = BuildTypeListing<MovementType>("UnnamedStrategyGame.Game.MovementTypes");
        }
    }
}

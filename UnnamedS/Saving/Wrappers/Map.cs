using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Saving.Wrappers
{
    public class Map : BaseWrapper
    {
        public override SaveType Type { get; } = SaveType.Map;
        public string Author { get; }
        public override string Extension { get; } = ".VanitatiMap";

        public Map(Game.BattleGameState.Fields fields, string author) : base(fields)
        {
            Contract.Requires<ArgumentNullException>(null != author);

            Author = author;
        }

        private Map() : base() { }
        public static Map Instance { get; } = new Map();
    }
}

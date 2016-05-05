using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class GameStarted : StateChange
    {
        public int Height { get; }
        public int Width { get; }
        public Terrain[] Terrain { get; }
        public Unit[] Units { get; }
        public Commander[] Commanders { get; }
        public IDictionary<string, object> GameStateAttributes { get; }

        public GameStarted(
            int height, int width, 
            Terrain[] terrain, Unit[] units, Commander[] commanders, 
            IDictionary<string, object> gameStateAttributes
        ) : base()
        {
            Contract.Requires<ArgumentNullException>(null != terrain);
            Contract.Requires<ArgumentNullException>(null != units);
            Contract.Requires<ArgumentNullException>(null != commanders);
            Contract.Requires<ArgumentNullException>(null != gameStateAttributes);

            Height = height;
            Width = width;
            Terrain = terrain;
            Units = units;
            Commanders = commanders;
            GameStateAttributes = gameStateAttributes;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Terrain);
            Contract.Invariant(null != Units);
            Contract.Invariant(null != Commanders);
            Contract.Invariant(null != GameStateAttributes);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
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
            Height = height;
            Width = width;
            Terrain = terrain;
            Units = units;
            Commanders = commanders;
            GameStateAttributes = gameStateAttributes;
        }
    }
}

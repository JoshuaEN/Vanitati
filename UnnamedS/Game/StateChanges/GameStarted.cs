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
        public Player[] Players { get; }
        public Dictionary<string, object> GameStateAttributes { get; }

        public GameStarted(
            int height, int width, 
            Terrain[] terrain, Unit[] units, Player[] players, 
            Dictionary<string, object> gameStateAttributes
        ) : base(new List<IAttribute>())
        {
            Height = height;
            Width = width;
            Terrain = terrain;
            Units = units;
            Players = players;
            GameStateAttributes = gameStateAttributes;
        }
    }
}

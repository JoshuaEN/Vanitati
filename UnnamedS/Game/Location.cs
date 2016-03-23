using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    public class Location
    {
        public int X { get; }
        public int Y { get; }

        [Newtonsoft.Json.JsonConstructor]
        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Location()
        {
            X = 0;
            Y = 0;
        }
    }
}

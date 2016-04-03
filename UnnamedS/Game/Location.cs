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

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is Location))
                return false;

            var otherLoc = (obj as Location);

            return otherLoc.X == X && otherLoc.Y == Y;
        }

        public bool Equals(Location loc)
        {
            if (System.Object.ReferenceEquals(this, loc))
                return true;

            if (loc == null)
                return false;

            return loc.X == X && loc.Y == Y;
        }

        public static bool operator ==(Location a, Location b)
        {
            if (System.Object.ReferenceEquals(a, b))
                return true;

            if (a == null && b != null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Location a, Location b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() * 17 + Y.GetHashCode();
        }
    }
}

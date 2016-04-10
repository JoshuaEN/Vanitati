using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
            if (object.ReferenceEquals(obj, null))
                return false;

            if (!(obj is Location))
                return false;

            var otherLoc = (obj as Location);

            return this == otherLoc;
        }

        public bool Equals(Location loc)
        {
            return this == loc;
        }

        public static bool operator ==(Location a, Location b)
        {
            if (object.ReferenceEquals(a, b))
                return true;

            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
                return false;

            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Location a, Location b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() * 17 + Y.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", X, Y);
        }
    }
}

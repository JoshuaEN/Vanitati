using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class User
    {
        public int UserID { get; }
        public string Name { get; set; }
        public bool IsHost { get; set; }

        public User(int userID, string name, bool isHost = false)
        {
            UserID = userID;
            Name = name;
            IsHost = isHost;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null))
                return false;

            if (!(obj is User))
                return false;

            var otherLoc = (obj as User);

            return this == otherLoc;
        }

        public bool Equals(User loc)
        {
            return this == loc;
        }

        public static bool operator ==(User a, User b)
        {
            if (object.ReferenceEquals(a, b))
                return true;

            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
                return false;

            return a.UserID == b.UserID;
        }

        public static bool operator !=(User a, User b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return UserID.GetHashCode();
        }
    }
}

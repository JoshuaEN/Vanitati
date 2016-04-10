using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
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
    }
}

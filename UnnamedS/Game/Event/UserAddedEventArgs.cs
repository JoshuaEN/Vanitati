using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Event
{
    public class UserAddedEventArgs : EventArgs
    {
        public User User { get; }

        public UserAddedEventArgs(User user)
        {
            User = user;
        }
    }
}

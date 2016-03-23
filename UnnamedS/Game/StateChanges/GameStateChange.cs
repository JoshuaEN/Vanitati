using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    public class GameStateChange : StateChange
    {
        public GameStateChange(List<IAttribute> updatedAttributes) : base(updatedAttributes) { }
    }
}

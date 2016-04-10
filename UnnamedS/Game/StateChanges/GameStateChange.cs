using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    public class GameStateChange : UpdateStateChange
    {
        public GameStateChange(IDictionary<string, object> updatedProperties) : base(updatedProperties) { }
    }
}

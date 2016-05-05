using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class GameStateChange : UpdateStateChange
    {
        public GameStateChange(IDictionary<string, object> updatedProperties) : base(updatedProperties) { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    public class PlayerStateChange : StateChange
    {
        public uint PlayerId { get; }
        public PlayerStateChange(uint playerId, List<IAttribute> updatedAttributes) : base(updatedAttributes)
        {
            PlayerId = playerId;
        }
    }
}

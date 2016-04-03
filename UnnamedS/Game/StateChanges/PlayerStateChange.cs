using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    public class PlayerStateChange : StateChange
    {
        public int PlayerId { get; }
        public Cause ChangeCause { get; }
        public PlayerStateChange(int playerId, List<IAttribute> updatedAttributes, Cause cause = Cause.Changed) : base(updatedAttributes)
        {
            PlayerId = playerId;
            ChangeCause = cause;
        }

        public enum Cause { Added, Removed, Changed }
    }
}

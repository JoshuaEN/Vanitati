using System;

namespace UnnamedStrategyGame.Game.Event
{
    public class PlayerChangedEventArgs : EventArgs
    {
        public StateChanges.PlayerStateChange ChangeInfo { get; }

        public PlayerChangedEventArgs(StateChanges.PlayerStateChange changeInfo)
        {
            ChangeInfo = changeInfo;
        }
    }
}
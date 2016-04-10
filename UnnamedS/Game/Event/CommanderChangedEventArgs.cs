using System;

namespace UnnamedStrategyGame.Game.Event
{
    public class CommanderChangedEventArgs : EventArgs
    {
        public StateChanges.CommanderStateChange ChangeInfo { get; }

        public CommanderChangedEventArgs(StateChanges.CommanderStateChange changeInfo)
        {
            ChangeInfo = changeInfo;
        }
    }
}
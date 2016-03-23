namespace UnnamedStrategyGame.Game.Event
{
    public class UnitChangedEventArgs : System.EventArgs
    {
        public StateChanges.UnitStateChange ChangeInfo { get; }

        public UnitChangedEventArgs(StateChanges.UnitStateChange changeInfo)
        {
            ChangeInfo = changeInfo;
        }
    }
}
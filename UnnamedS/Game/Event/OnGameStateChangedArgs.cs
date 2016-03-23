namespace UnnamedStrategyGame.Game.Event
{
    public class OnGameStateChangedArgs : System.EventArgs
    {
        public StateChanges.GameStateChange ChangeInfo { get; }

        public OnGameStateChangedArgs(StateChanges.GameStateChange changeInfo)
        {
            ChangeInfo = changeInfo;
        }
    }
}
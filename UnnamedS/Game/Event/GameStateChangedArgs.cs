namespace UnnamedStrategyGame.Game.Event
{
    public class GameStateChangedArgs : System.EventArgs
    {
        public StateChanges.GameStateChange ChangeInfo { get; }

        public GameStateChangedArgs(StateChanges.GameStateChange changeInfo)
        {
            ChangeInfo = changeInfo;
        }
    }
}
namespace UnnamedStrategyGame.Game.Event
{
    public class TerrainChangedEventArgs : System.EventArgs
    {
        public StateChanges.TerrainStateChange ChangeInfo { get; }

        public TerrainChangedEventArgs(StateChanges.TerrainStateChange changeInfo)
        {
            ChangeInfo = changeInfo;
        }
    }
}
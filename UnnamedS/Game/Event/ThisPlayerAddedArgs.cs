namespace UnnamedStrategyGame.Game.Event
{
    public class OnThisPlayerAddedArgs
    {
        public int YourPlayerId { get; }

        public OnThisPlayerAddedArgs(int yourPlayerId)
        {
            YourPlayerId = yourPlayerId;
        }
    }
}
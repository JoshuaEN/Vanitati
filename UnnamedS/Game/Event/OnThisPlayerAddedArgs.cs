namespace UnnamedStrategyGame.Game.Event
{
    public class OnThisPlayerAddedArgs
    {
        public uint YourPlayerId { get; }

        public OnThisPlayerAddedArgs(uint yourPlayerId)
        {
            YourPlayerId = yourPlayerId;
        }
    }
}
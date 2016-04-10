
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.Game
{
    /// <summary>
    /// Event handling cannot be used by the GameLogic code to signal to Players
    /// because it needs to be able to control which messages get sent to which players
    /// for a number of use cases, including:
    /// * Player Chat
    /// * Fog of war
    /// * Invisible units (e.g. subs)
    /// This interface represents the possible events.
    /// </summary>
    public interface IUserLogic
    {
        /// <summary>
        /// When one or more actions are taken.
        /// </summary>
        void OnActionsTaken(object sender, Event.ActionsTakenEventArgs e);

        /// <summary>
        /// When a player's state changes.
        /// </summary>
        void OnCommanderChanged(object sender, Event.CommanderChangedEventArgs e);

        /// <summary>
        /// When a unit's state changes.
        /// </summary>
        void OnUnitChanged(object sender, Event.UnitChangedEventArgs e);

        /// <summary>
        /// When a terrain's state changes.
        /// </summary>
        void OnTerrainChanged(object sender, Event.TerrainChangedEventArgs e);

        /// <summary>
        /// When the game's state changes.
        /// </summary>
        void OnGameStateChanged(object sender, Event.GameStateChangedArgs e);

        void OnUserAdded(object sender, Event.UserAddedEventArgs e);

        void OnUserRemoved(object sender, Event.UserRemovedEventArgs e);

        void OnUserAssignedToCommander(object sender, Event.UserAssignedToCommanderEventArgs e);

        void OnGameStart(object sender, Event.GameStartEventArgs e);

        void OnTurnEnded(object sender, Event.TurnEndedEventArgs args);
        void OnSync(object sender, SyncEventArgs args);
    }
}

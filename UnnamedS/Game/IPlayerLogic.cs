using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    /// <summary>
    /// Event handling cannot be used by the GameLogic code to siginal to Players
    /// because it needs to be able to control which messages get sent to which players
    /// for a number of use cases, including:
    /// * Player Chat
    /// * Fog of war
    /// * Invisible units (e.g. subs)
    /// This interface represents the possible events.
    /// </summary>
    public interface IPlayerLogic
    {
        /// <summary>
        /// When one or more actions are taken.
        /// </summary>
        void OnActionsTaken(object sender, Event.ActionsTakenEventArgs e);

        /// <summary>
        /// When a player's state changes.
        /// </summary>
        void OnPlayerChanged(object sender, Event.PlayerChangedEventArgs e);

        /// <summary>
        /// When a unit's state changes.
        /// </summary>
        void OnUnitChanged(object sender, Event.UnitChangedEventArgs e);

        /// <summary>
        /// When a terrian's state changes.
        /// </summary>
        void OnTerrainChanged(object sender, Event.TerrainChangedEventArgs e);

        /// <summary>
        /// When the game's state changes.
        /// </summary>
        void OnGameStateChanged(object sender, Event.OnGameStateChangedArgs e);

        /// <summary>
        /// 
        /// </summary>
        void OnThisPlayerAdded(object sender, Event.OnThisPlayerAddedArgs e);
    }
}

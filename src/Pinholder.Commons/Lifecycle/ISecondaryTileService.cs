using System;

namespace PinHolder.Lifecycle
{
    public interface ISecondaryTileService
    {
        /// <summary>
        /// Checks if secondary tile for card already exists
        /// </summary>
        /// <param name="cardId">Card id</param>
        /// <returns>true if secondary tile can be created</returns>
        bool CanCreate(int cardId);

        /// <summary>
        /// Tryes to create new secondary tile
        /// </summary>
        /// <param name="cardName">Card name</param>
        /// <param name="cardDescription">Card description</param>
        /// <param name="cardId">Card Id</param>
        /// <param name="exists">Delegate to execute if tile already exists</param>
        void TryCreate(string cardName, string cardDescription, int cardId, Action exists);

        /// <summary>
        /// Deletes secondary tile by card Id
        /// </summary>
        /// <param name="cardId">card Id</param>
        void DeleteTile(int cardId);
    }
}
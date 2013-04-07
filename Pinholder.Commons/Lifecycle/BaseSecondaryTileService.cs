using System;
using PinHolder.Navigation;

namespace PinHolder.Lifecycle
{
    public abstract class BaseSecondaryTileService : ISecondaryTileService
    {
        /// <summary>
        /// Checks if secondary tile for card already exists
        /// </summary>
        /// <param name="cardId">Card id</param>
        /// <returns>true if secondary tile can be created</returns>
        public bool CanCreate(int cardId)
        {
            return CheckCreationState(FormatNavigationUri(cardId)) == CreationState.CanCreate;
        }

        /// <summary>
        /// Tries to create new secondary tile
        /// </summary>
        /// <param name="cardName">Card name</param>
        /// <param name="cardDescription">Card description</param>
        /// <param name="cardId">Card Id</param>
        /// <param name="exists">Delegate to execute if tile already exists</param>
        public void TryCreate(string cardName, string cardDescription, int cardId, Action exists)
        {
            if (exists  == null) throw new ArgumentNullException("exists");

            var uri = FormatNavigationUri(cardId);

            var createStatus = CheckCreationState(uri);
            switch (createStatus)
            {
                    case CreationState.CanCreate:
                                CreateTile(cardName,cardDescription, uri);
                    break;
                    case CreationState.AlreadyExcists:
                    exists();
                    break;
            }
        }

        /// <summary>
        /// Deletes secondary tile by card Id
        /// </summary>
        /// <param name="cardId">card Id</param>
        public void DeleteTile(int cardId)
        {
            var uri = FormatNavigationUri(cardId);
            DeleteTileIfExists(uri);
        }

        private static string FormatNavigationUri(int cardId)
        {
            return string.Format("{0}?{1}={2}", Pages.ViewPage, Keys.Id, cardId);
        }

        private CreationState CheckCreationState(string uri)
        {
            return IsTileExists(uri) ? CreationState.AlreadyExcists : CreationState.CanCreate;
        }

        protected abstract bool IsTileExists(string partOfUri);

        protected abstract void DeleteTileIfExists(string uri);

        protected abstract void CreateTile(string cardName, string cardDescription, string uri);

    }
}

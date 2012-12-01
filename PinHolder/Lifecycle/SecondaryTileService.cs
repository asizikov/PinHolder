using System;
using System.Linq;
using Microsoft.Phone.Shell;
using PinHolder.Navigation;

namespace PinHolder.Lifecycle
{
    public sealed class SecondaryTileService : ISecondaryTileService
    {
        private const string IMAGE_PATH = "ApplicationIcon.png";

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
        /// <param name="cardId">Card Id</param>
        /// <param name="exists">Delegate to execute if tile already exists</param>
        public void TryCreate(string cardName, int cardId, Action exists)
        {
            if (exists  == null) throw new ArgumentNullException("exists");

            var uri = FormatNavigationUri(cardId);

            var createStatus = CheckCreationState(uri);
            switch (createStatus)
            {
                    case CreationState.CanCreate:
                                var tile = GetSecondaryTileData(cardName);                              
                                ShellTile.Create(new Uri(uri,UriKind.Relative), tile);
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
            var tile = FindTile(uri);
            if (tile != null)
            {
                tile.Delete();
            }

        }

        private static string FormatNavigationUri(int cardId)
        {
            return string.Format("{0}?{1}={2}", Pages.ViewPage, Keys.Id, cardId);
        }

        private CreationState CheckCreationState(string uri)
        {
            var tile = FindTile(uri);
            if(tile != null) return CreationState.AlreadyExcists;
            return CreationState.CanCreate;
        }

        private static ShellTile FindTile(string partOfUri)
        {
            var shellTile = ShellTile.ActiveTiles.FirstOrDefault(
                tile => tile.NavigationUri.ToString().Contains(partOfUri));

            return shellTile;
        }


        private  StandardTileData GetSecondaryTileData(string cardName)
        {
            var tileData = new StandardTileData
            {
                Title = cardName,
                BackgroundImage = new Uri(IMAGE_PATH, UriKind.Relative),
            };
            return tileData;
        }
    }
}

using System;
using System.Linq;
using Mangopollo;
using Microsoft.Phone.Shell;
using PinHolder.Navigation;
using Mangopollo.Tiles;

namespace PinHolder.Lifecycle
{
    public sealed class SecondaryTileService : ISecondaryTileService
    {
        private const string BACKGROUND_IMAGE_PATH = "Background.png";
        private const string BACKGROUND_WIDE_IMAGE_PATH = "Background-Wide.png";
        private const string BACKGROUND_SMALL_IMAGE_PATH = "ApplicationIcon.png";

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

        private static void CreateTile(string cardName, string cardDescription, string uri)
        {
            if (Utils.CanUseLiveTiles)
            {
              var tiledata =  TilesCreator.CreateFlipTile(cardName, cardName, 
                    cardDescription, 
                    cardDescription,
                    null,
                    backBackgroundImage: new Uri(BACKGROUND_IMAGE_PATH, UriKind.Relative),
                    smallBackgroundImage: new Uri(BACKGROUND_SMALL_IMAGE_PATH, UriKind.Relative),
                    backgroundImage: new Uri(BACKGROUND_IMAGE_PATH, UriKind.Relative),
                    wideBackgroundImage: new Uri(BACKGROUND_WIDE_IMAGE_PATH, UriKind.Relative),
                    wideBackBackgroundImage: new Uri(BACKGROUND_WIDE_IMAGE_PATH, UriKind.Relative));
              ShellTileExt.Create(new Uri(uri, UriKind.Relative), tiledata, true);
            }
            else
            {
                var tile = GetSecondaryTileData(cardName);
                ShellTile.Create(new Uri(uri, UriKind.Relative), tile);
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

        private static CreationState CheckCreationState(string uri)
        {
            var tile = FindTile(uri);
            return tile != null ? CreationState.AlreadyExcists : CreationState.CanCreate;
        }

        private static ShellTile FindTile(string partOfUri)
        {
            var shellTile = ShellTile.ActiveTiles.FirstOrDefault(
                tile => tile.NavigationUri.ToString().Contains(partOfUri));

            return shellTile;
        }


        private static StandardTileData GetSecondaryTileData(string cardName)
        {
            var tileData = new StandardTileData
            {
                Title = cardName,
                BackgroundImage = new Uri(BACKGROUND_IMAGE_PATH, UriKind.Relative),
            };
            return tileData;
        }
    }
}

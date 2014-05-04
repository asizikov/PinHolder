using System;
using System.Linq;
using Mangopollo;
using Mangopollo.Tiles;
using Microsoft.Phone.Shell;

namespace PinHolder.Lifecycle
{
    public sealed class SecondaryTileService : BaseSecondaryTileService
    {
        private const string BACKGROUND_IMAGE_PATH = "Background.png";
        private const string BACKGROUND_WIDE_IMAGE_PATH = "Background-Wide.png";
        private const string BACKGROUND_SMALL_IMAGE_PATH = "ApplicationIcon.png";

        protected override bool IsTileExists(string partOfUri)
        {
            return ShellTile.ActiveTiles.FirstOrDefault(
                tile => tile.NavigationUri.ToString().Contains(partOfUri)) != null;
        }

        protected override void DeleteTileIfExists(string uri)
        {
            var tile = ShellTile.ActiveTiles.FirstOrDefault(
                t => t.NavigationUri.ToString().Contains(uri));
            if (tile != null)
            {
                tile.Delete();
            }
        }


        private static StandardTileData GetSecondaryTileData(string cardName, string cardDescription)
        {
            var tileData = new StandardTileData
                {
                    Title = cardName,
                    BackTitle = cardName,
                    BackContent = cardDescription,
                    BackBackgroundImage = new Uri(BACKGROUND_IMAGE_PATH, UriKind.Relative),
                    BackgroundImage = new Uri(BACKGROUND_IMAGE_PATH, UriKind.Relative),
                };
            return tileData;
        }



        protected override void CreateTile(string cardName, string cardDescription, string uri)
        {
            if (Utils.CanUseLiveTiles)
            {
                var tiledata = TilesCreator.CreateFlipTile(cardName, cardName,
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
                var tile = GetSecondaryTileData(cardName, cardDescription);
                ShellTile.Create(new Uri(uri, UriKind.Relative), tile);
            }

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using _112FrieslandLogic.Data;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using _112FrieslandLogic;
using WRCHelperLibrary;

namespace _112FrieslandBackgroundWP
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            IList<NewsLink> Content = await NotificationDataHandler.GenerateNotifications();

            if (Content.Count > 0)
            {
                CreateTiles(Content.Cast<INewsLink>().ToList(), Content.Count);
                BadgeHandler.CreateBadge(Content.Count());
            }

            deferral.Complete();
        }

        private void CreateTiles(IList<INewsLink> Content, int Counter)
        {
            XmlDocument RectangleTile = TileXmlHandler.CreateRectangleTile(TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150IconWithBadgeAndText), Content, Counter, "ms-appx:///assets/BadgeLogo.scale-240.png", "112Fryslân.nl");
            XmlDocument SquareTile = TileXmlHandler.CreateSquareTile(TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150IconWithBadge), Content, "ms-appx:///assets/BadgeLogo.scale-240.png", "112Fryslân");
            XmlDocument SmallTile = TileXmlHandler.CreateSmallSquareTile(TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare71x71IconWithBadge), "ms-appx:///assets/BadgeLogo.scale-240.png", "112Fryslân");

            TileXmlHandler.CreateTileUpdate(new XmlDocument[] { RectangleTile, SquareTile, SmallTile });
        }
    }
}

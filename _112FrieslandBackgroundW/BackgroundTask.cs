using _112FrieslandLogic;
using _112FrieslandLogic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;
using WRCHelperLibrary;

namespace _112FrieslandBackgroundW
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            IList<NewsLink> Content = await NotificationDataHandler.GenerateNotifications();

            if (Content.Count > 0)
            {
                ToastHandler.CreateToast(Content);
                CreateTiles(Content.Cast<INewsLink>().ToList(), Content.Count);
                BadgeHandler.CreateBadge(Content.Count());
            }

            deferral.Complete();
        }

        private void CreateTiles(IList<INewsLink> Content, int Counter)
        {
            XmlDocument RectangleTile = TileXmlHandler.CreateRectangleTile(TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text01), Content, Counter, string.Empty, string.Empty, "112Fryslân");
            XmlDocument SquareTile = TileXmlHandler.CreateLargeSquareTile(TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare310x310Text01), Content);
            XmlDocument SmallTile = TileXmlHandler.CreateSquareTile(TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText03), Content, "ms-appx:///assets/Logo.scale-100.png", "112Fryslân", "112Fryslân");

            TileXmlHandler.CreateTileUpdate(new XmlDocument[] { RectangleTile, SquareTile, SmallTile });
        }
    }
}

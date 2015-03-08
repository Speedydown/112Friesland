using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;

namespace _112Friesland
{
    public static class ArticleCounter
    {
        public static void AddArticleCount()
        {
            int Counter = GetCurrentCount() + 1;

            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;

            try
            {
                localSettings.Values["NumberOfArticles"] = Counter;
            }
            catch
            {

            }

            if (Counter == 25)
            {
                ShowRateDialog();
            }
        }

        private static int GetCurrentCount()
        {
            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;

            try
            {
                return (int)localSettings.Values["NumberOfArticles"];
            }
            catch
            {
                return 0;
            }
        }

        private static async Task ShowRateDialog()
        {
            var messageDialog = new Windows.UI.Popups.MessageDialog("Wij bieden 112Fryslân kostenloos aan en we zouden het op prijs stellen als u de 112Fryslân app een positieve review geeft.", "Bedankt");
            messageDialog.Commands.Add(
            new Windows.UI.Popups.UICommand("Review", CommandInvokedHandler));
            messageDialog.Commands.Add(
            new Windows.UI.Popups.UICommand("Annuleren", CommandInvokedHandler));
            await messageDialog.ShowAsync();
        }


        private static void CommandInvokedHandler(IUICommand command)
        {
            if (command.Label == "Review")
            {
                Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + "a18b206e-3c35-487c-993e-1a2dc7eb4238"));
            }
        }
    }
}

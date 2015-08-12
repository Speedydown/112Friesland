using _112FrieslandLogic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace _112FrieslandLogic
{
    public static class NotificationDataHandler
    {
        public static IAsyncOperation<IList<NewsLink>> GenerateNotifications()
        {
            return GenerateNotificationsHelper().AsAsyncOperation();
        }

        private static async Task<IList<NewsLink>> GenerateNotificationsHelper()
        {
            try
            {
                ApplicationData applicationData = ApplicationData.Current;
                ApplicationDataContainer localSettings = applicationData.LocalSettings;
                IList<NewsLink> News = await DataHandler.GetNewsLinksByPage(1);
                IList<NewsLink> newNews = new List<NewsLink>();


                string LastURL = string.Empty;

                if (localSettings.Values["LastNewsItem"] != null)
                {
                    LastURL = localSettings.Values["LastNewsItem"].ToString();
                }
                else
                {
                    return newNews;
                }

#if DEBUG
                //Test data
                //LastURL = News[3].URL;
#endif
                int NotificationCounter = 0;

                foreach (NewsLink n in News)
                {
                    if (n.URL == LastURL)
                    {
                        if (NotificationCounter > 0)
                        {
                            return (newNews);
                        }
                    }

                    newNews.Add(n);
                    NotificationCounter++;

                }
            }
            catch (Exception)
            {

            }

            return new List<NewsLink>();
        }
    }
}

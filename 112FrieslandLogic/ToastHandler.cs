using _112FrieslandLogic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;

namespace _112FrieslandLogic
{
    public static class ToastHandler
    {
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public static void CreateToast(IList<NewsLink> Content)
        {
            if (Content == null || Content.Count == 0)
            {
                return;
            }

            string LastToast = string.Empty;

            if (localSettings.Values["LastToast"] != null)
            {
                LastToast = localSettings.Values["LastToast"].ToString();
            }
            else
            {
                localSettings.Values["LastToast"] = Content.First().URL;
                return;
            }

            localSettings.Values["LastToast"] = Content.First().URL;
            int ToastCounter = 0;

#if DEBUG
            //TestData
           // LastToast = Content.Last().URL;
#endif

            foreach (NewsLink a in Content)
            {
                if (a.URL == LastToast)
                {
                    break;
                }

                CreateActualToast(a.Title, a.Content, a.URL);
                ToastCounter++;

                if (ToastCounter > 3)
                {
                    break;
                }
            }
        }

        private static void CreateActualToast(string TileContent, string SecondaryContent, string ContentURL)
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(TileContent));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(SecondaryContent));

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.IM");

            toastNode.AppendChild(audio);

            ((XmlElement)toastNode).SetAttribute("launch", ContentURL);
            ToastNotification toast = new ToastNotification(toastXml);

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}

using _112Friesland.Common;
using _112FrieslandBackgroundW;
using _112FrieslandLogic;
using _112FrieslandLogic.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WRCHelperLibrary;

namespace _112Friesland
{
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private static IList<NewsLink> newsLinks = null;
        public static DateTime TimeLoaded = DateTime.Now.AddDays(-1);

        private string CurrentURL = string.Empty;
        private bool StopRefresh = false;

        public MainPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

         public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public static void ClearCachedData()
        {
            newsLinks = null;
            TimeLoaded = DateTime.Now.AddDays(-1);
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();

            if (newsLinks == null || DateTime.Now.Subtract(TimeLoaded).TotalMinutes > 5)
            {
                await LoadData();
            }

            if (e.NavigationParameter != null && e.NavigationParameter.ToString() != "")
            {
                try
                {
                    await this.OpenNewsItem(e.NavigationParameter.ToString());
                   
                    return;
                }
                catch
                {

                }
            }
        }

        private async Task OpenNewsItem(string URL)
        {
            try
            {
                NewsItemControl.DataContext = null;
                NewsItemLoadingControl.DisplayLoadingError(false);
                NewsItemLoadingControl.SetLoadingStatus(true);

                if (URL != null)
                {
                    CurrentURL = URL;
                    NewsPage newsItem = await DataHandler.GetNewsPageFromURL(URL);
                    NewsItemControl.DataContext = newsItem;
                }
            }
            catch
            {
                NewsItemLoadingControl.DisplayLoadingError(true);
            }
            finally
            {
                NewsItemLoadingControl.SetLoadingStatus(false);
            }

            ArticleCounter.AddArticleCount();
            Task t = Task.Run(() => DataHandler.GetDataFromURL("http://speedydown-001-site2.smarterasp.net/api.ashx?Fryslan=" + URL));
        }

        private async Task<IList<NewsLink>> GetNewsLinksOperationAsTask()
        {
            try
            {
                List<NewsLink> NewsLinks = (List<NewsLink>)await DataHandler.GetNewsLinksByPage(1);
                NewsLinks.AddRange((List<NewsLink>)await DataHandler.GetNewsLinksByPage(2));
                NewsLinks.AddRange((List<NewsLink>)await DataHandler.GetNewsLinksByPage(3));
                return NewsLinks;
            }
            catch
            {
                LoadingControl.DisplayLoadingError(true);
                return new List<NewsLink>();
            }
        }

  
        private async Task LoadData()
        {
            LoadingControl.DisplayLoadingError(false);
            LoadingControl.SetLoadingStatus(true, NewsLV.Items.Count == 0);
            NewsLV.ItemsSource = null;

            Task<IList<NewsLink>> GetNewsLinksTask = GetNewsLinksOperationAsTask();
            newsLinks = await GetNewsLinksTask;
            NewsLV.ItemsSource = newsLinks;
           
            LoadingControl.SetLoadingStatus(false);
            TimeLoaded = DateTime.Now;

            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;

            try
            {
                localSettings.Values["LastNewsItem"] = newsLinks.First().URL;
                NotificationHandler.Run("_112FrieslandBackgroundW.BackgroundTask", "_112FryslânBackGroundWorker",15);
            }
            catch
            {

            }

            if (newsLinks.Count > 0)
            {
                await this.OpenNewsItem(newsLinks.First().URL);
            }

            Task RefreshTask = Task.Run(() => this.RefreshData());
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            StopRefresh = true;
        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void NewsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            await this.OpenNewsItem((e.ClickedItem as NewsLink).URL);
        }

        private async Task RefreshData()
        {
            while (!StopRefresh)
            {
                await Task.Delay(150000);

                IList<NewsLink> NewsLinks = await GetNewsLinksOperationAsTask();

                try
                {
                    Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        if ((NewsLinks.Count > 0 && NewsLinks.First().URL != newsLinks.First().URL) || newsLinks.Count == 0)
                        {
                            newsLinks = NewsLinks;
                            NewsLV.ItemsSource = newsLinks;
                        }
                    });
                }
                catch
                {

                }
            }
        }
    }
}

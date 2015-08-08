using _112Friesland.Common;
using _112FrieslandBackgroundWP;
using _112FrieslandLogic;
using _112FrieslandLogic.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
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
        private static DateTime? LastLoadedDT = null;
        public static MainPage Instance { get; private set; }
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private List<NewsLink> NewsLinks;

        public MainPage()
        {
            Instance = this;
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.LoadData();
        }

        public async void LoadData(bool OverrideTimer = false)
        {
            LoadingControl.DisplayLoadingError(false);

            if (OverrideTimer)
            {
                LastLoadedDT = DateTime.Now.AddHours(-1);
            }

            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
            LoadingControl.SetLoadingStatus(true);

            if (LastLoadedDT == null || NewsLinks == null || DateTime.Now.Subtract((DateTime)LastLoadedDT).TotalMinutes > 5)
            {
                try
                {
                    NewsLinks = (List<NewsLink>)await DataHandler.GetNewsLinksByPage(1);
                    NewsLinks.AddRange((List<NewsLink>)await DataHandler.GetNewsLinksByPage(2));

                    this.ContentListview.ItemsSource = NewsLinks;

                    if (LastLoadedDT == null)
                    {
                        NotificationHandler.Run("_112FrieslandBackgroundWP.BackgroundTask", "_112FryslânBackGroundWorker");
                    }

                    ApplicationData applicationData = ApplicationData.Current;
                    ApplicationDataContainer localSettings = applicationData.LocalSettings;

                    try
                    {
                        localSettings.Values["LastNewsItem"] = NewsLinks.First().URL;
                    }
                    catch
                    {

                    }

                    LastLoadedDT = DateTime.Now;
                }
                catch (Exception)
                {
                    NewsLinks = null;
                    LoadingControl.DisplayLoadingError(true);
                }
            }
            else
            {
                try
                {
                    this.ContentListview.ItemsSource = NewsLinks;
                }
                catch
                {


                }
            }

            LoadingControl.SetLoadingStatus(false);
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void ContentListview_ItemClick(object sender, ItemClickEventArgs e)
        {
            NewsLink NL = (NewsLink)e.ClickedItem;

            if (!Frame.Navigate(typeof(ItemPage), (NL).URL))
            {

            }
        }

        private async void PrivacyPolicyButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://wiezitwaarvandaag.nl/privacypolicy.aspx"));
        }

        private async void _112FryslanButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://www.112fryslan.nl/"));
        }
    }
}

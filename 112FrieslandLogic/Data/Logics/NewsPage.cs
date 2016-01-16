using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;
using WebCrawlerTools;
using XamlControlLibrary.Interfaces;

namespace _112FrieslandLogic.Data
{
    public sealed class NewsPage : INewsItem
    {
        public string Title { get; private set; }
        public string ContentSummary { get; private set; }
        public IList<string> Body { get; private set; }
        public IList<string> ImageList { get; private set; }
        public string RealAuthor { get; private set; }
        public string Author
        {
            get
            {
                return this.RealAuthor + " op " + this.Date;
            }
        }
        public string Date { get; private set; }
        public string Added { get; private set; }
        public string Updated { get; private set; }

        public string TimeStamp
        {
            get
            {
                return string.Empty;
            }
        }

        public Brush TitleColor
        {
            get { return new SolidColorBrush(Colors.Black); }
        }

        public Brush TitleColorWindows
        {
            get { return new SolidColorBrush(Colors.DarkGray); }
        }

        public Visibility MediaVisibilty
        {
            get
            {
                return MediaFile == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility SummaryVisibilty
        {
            get { return ContentSummary.Length > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility TimeStampVisibilty
        {
            get
            {
                return Visibility.Collapsed;
            }
        }

        private Thickness _ContentMargins = new Thickness(4, 15, 4, 5);
        public Thickness ContentMargins
        {
            get
            {
                return _ContentMargins;
            }
        }

        public Uri MediaFile { get; private set; }

        public Uri YoutubeURL
        {
            get { return null; }
        }

        public Visibility DisplayWebView
        {
            get { return Visibility.Collapsed; }
        }

        public NewsPage(string Title, string ContentSummary, IList<string> Content, IList<string> ImageList, string Author, string Date)
        {
            this.Title = WebUtility.HtmlDecode(Title);

            if (ContentSummary.Length > 0 && Content.Count == 0)
            {
                Content.Add(ContentSummary);
                ContentSummary = string.Empty;
            }

            this.Body = Content;
            this.ContentSummary = ContentSummary;

            for (int i = 0; i < this.Body.Count; i++)
            {
                if (Body[i].Contains("&#x2013;"))
                {
                    string Out = string.Empty;
                    Body[i] = "<start>" + Body[i];
                    this.ContentSummary = HTMLParserUtil.GetContentAndSubstringInput("<start>", "<br />&#xD;", Body[i], out Out);
                    Body[i] = Out;
                }

                this.Body[i] = this.CleanString(this.Body[i]);
            }

            this.ContentSummary = CleanString(this.ContentSummary.Replace("&#8211;", ""));

            this.ImageList = ImageList;
            this.RealAuthor = Author;

            if (Date.Contains("Gepubliceerd op:"))
            {
                this.Date = WebUtility.HtmlDecode(Date.Substring("Gepubliceerd op:".Length)).Trim();
            }
            else
            {
                this.Date = WebUtility.HtmlDecode(Date).Trim();
            }
        }

        private string CleanString(string Input)
        {
            return HTMLParserUtil.CleanHTMLTagsFromString(WebUtility.HtmlDecode(Input).Trim()).Replace("\r", "\n").Replace("\n ", "\n");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WRCHelperLibrary;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;
using WebCrawlerTools;

namespace _112FrieslandLogic.Data
{
    public sealed class NewsPage : INewsItem
    {
        private static readonly string[] RemoveFilter = new string[] { "<br>", "<br />", "<br/>", "<BR>", "<p>", "<P>", "<em>", "</em>", "<strong>", "</strong>", "<u>", "</u>", "<b>", "</b>" };

        public string Title { get; private set; }
        public string ContentSummary { get; private set; }
        public string Content { get; private set; }
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
            get { return Visibility.Visible; }
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

        public Uri MediaFile { get; private set;}
        private IList<string> _Body;
        public IList<string> Body
        {
            get
            {
                if (_Body == null)
                {
                    _Body = new string[] { this.Content }.ToList();
                }

                return _Body;
            }
        }

        public Uri YoutubeURL
        {
            get { return null; }
        }

        public Visibility DisplayWebView
        {
            get { return Visibility.Collapsed; }
        }

        public NewsPage(string Title, string ContentSummary, string Content, IList<string> ImageList, string Author, string Date)
        {
            this.Title = WebUtility.HtmlDecode(Title);
            this.ContentSummary = this.CleanHTMLTags(ContentSummary);

            Content =  Content.Replace("<br />", "");

            while (true)
            {
                string temp = Content.Substring(Content.Length - 1);

                if (temp == "\n")
                {
                    Content = Content.Substring(0, Content.Length - 1);
                }
                else
                {
                    break;
                }
            }

            this.Content = HTMLParserUtil.CleanHTMLTagsFromString(this.CleanHTMLTags(Content));

            this.ImageList = ImageList;
            this.RealAuthor = Author;
            this.Date = Date;
        }

        private string CleanHTMLTags(string Input)
        {
            foreach (string s in RemoveFilter)
            {
                Input = Input.Replace(s, "");
            }

            Input = Input.Replace("&nbsp", " ");
            Input = WebUtility.HtmlDecode(Input);

            return Input;
        }
    }
}

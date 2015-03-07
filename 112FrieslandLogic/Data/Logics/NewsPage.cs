using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _112FrieslandLogic.Data
{
    public sealed class NewsPage
    {
        private static readonly string[] RemoveFilter = new string[] { "<br>", "<br />", "<br/>", "<BR>", "<p>", "<P>", "<em>", "</em>", "<strong>", "</strong>", "<u>", "</u>", "<b>", "</b>" };

        public string Title { get; private set; }
        public string ContentSummary { get; private set; }
        public string Content { get; private set; }
        public IList<string> ImageList { get; private set; }
        public string Author { get; private set; }
        public string Date { get; private set; }
        public string AuthorDate
        {
            get
            {
                return this.Author + " op " + this.Date;
            }
        }

        public NewsPage(string Title, string ContentSummary, string Content, IList<string> ImageList, string Author, string Date)
        {
            this.Title = WebUtility.HtmlDecode(Title);
            this.ContentSummary = this.CleanHTMLTags(ContentSummary);
            this.Content = this.CleanHTMLTags(Content);
            this.ImageList = ImageList;
            this.Author = Author;
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

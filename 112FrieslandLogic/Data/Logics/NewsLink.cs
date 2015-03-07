using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _112FrieslandLogic.Data
{
    public sealed class NewsLink
    {
        public string URL { get; private set; }
        public string ImageURL { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public string Author { get; private set; }
        public string Date { get; private set; }
        public string AuthorDate
        {
            get
            {
                return this.ToString();
            }
        }

        public NewsLink(string URL, string ImageURL, string Title, string Content, string Author, string Date)
        {
            this.URL = "http://www.112fryslan.nl/news/" + URL;
            this.ImageURL = "http://www.112fryslan.nl/" + ImageURL;
            this.Title = WebUtility.HtmlDecode(Title);
            this.Content = WebUtility.HtmlDecode(Content);
            this.Author = WebUtility.HtmlDecode(Author);
            this.Date = WebUtility.HtmlDecode(Date);
        }

        public override string ToString()
        {
            return this.Author + " op " + this.Date;
        }
    }
}

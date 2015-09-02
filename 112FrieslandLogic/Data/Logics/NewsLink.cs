using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WRCHelperLibrary;

namespace _112FrieslandLogic.Data
{
    public sealed class NewsLink : INewsLink
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

        public string CommentCount
        {
            get { return string.Empty; }
        }

        public string Time
        {
            get { return Date; }
        }

        public NewsLink(string URL, string ImageURL, string Title, string Content, string Author, string Date, string Location, string Region)
        {
            this.URL = URL;
            this.ImageURL = ImageURL;
            this.Title = WebUtility.HtmlDecode(Title).Trim();

            Location = Location.Replace("/r/n", "").Trim(); ;

            this.Content = WebUtility.HtmlDecode(Location + Content.Replace("-->\r\n", "").Replace("&#8211;", "-")).Trim().Replace("“", "").Replace("\r", "");
            this.Author = WebUtility.HtmlDecode(Author).Trim();
            this.Date = WebUtility.HtmlDecode(Date.Split('>')[1].Trim());
        }

        public override string ToString()
        {
            return this.Author + " op " + this.Date;
        }


       
    }
}

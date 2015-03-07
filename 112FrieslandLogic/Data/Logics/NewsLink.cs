using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _112FrieslandLogic.Data
{
    public sealed class NewsLink
    {
        public string URL { get; private set; }
        public string ImageURL { get; private set; }
        public string Content { get; private set; }
        public string Author { get; private set; }
        public string Date { get; private set; }

        public NewsLink(string URL, string ImageURL, string Content, string Author, string Date)
        {
            this.URL = "http://www.112fryslan.nl/news/" + URL;
            this.ImageURL = ImageURL;
            this.Content = Content;
            this.Author = Author;
            this.Date = Date;
        }

        public override string ToString()
        {
            return this.Author + " op " + this.Date;
        }
    }
}

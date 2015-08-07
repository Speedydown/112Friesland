using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerTools;

namespace _112FrieslandLogic.Data
{
    public static class NewsLinkParser
    {
        public static IList<NewsLink> GetNewsLinksFromSource(string Source)
        {
            //Remove unusable HeaderData
            Source = RemoveHeader(Source, "<div class='newsitems'>");

            //Parse Items
            return ParseContent(Source);
        }

        private static string RemoveHeader(string Input, string HTMLTextToCut)
        {
            try
            {
                return Input.Substring(HTMLParserUtil.GetPositionOfStringInHTMLSource(HTMLTextToCut, Input, false));

            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static List<NewsLink> ParseContent(string Input)
        {
            List<string> NewsLinksAsHTML = new List<string>();
            List<NewsLink> NewsLinks = new List<NewsLink>();

            while (Input.Length > 0)
            {
                string ContentSummary = string.Empty;

                try
                {
                    NewsLinksAsHTML.Add(HTMLParserUtil.GetContentAndSubstringInput("<div class='newsitem newsitem_published'><div class='newsitem_head'>", "</div><div class='spacer'></div></div>", Input, out Input));
                }
                catch
                {
                    break;
                }
            }

            if (NewsLinksAsHTML.Count == 0)
            {
                throw new Exception("No Items");
            }

            foreach (string s in NewsLinksAsHTML)
            {
                try
                {
                    NewsLinks.Add(GetNewsLinkFromHTMLSource(s));
                }
                catch (Exception)
                {

                }
            }

            return NewsLinks;
        }

        private static NewsLink GetNewsLinkFromHTMLSource(string Source)
        {
            string URL = HTMLParserUtil.GetContentAndSubstringInput("='news/", "' class='image_container'>", Source, out Source);
            string ImageURL = HTMLParserUtil.GetContentAndSubstringInput("' src='", "' alt=''", Source, out Source);

            string Title = string.Empty;

            try
            {
                Title = HTMLParserUtil.GetContentAndSubstringInput("class='newsitem_title'>", " <img align='top'", Source, out Source);
            }
            catch
            {
                Title = HTMLParserUtil.GetContentAndSubstringInput("class='newsitem_title'>", "</div>\n<div class='admin-icons'>", Source, out Source);
            }

            string Content = HTMLParserUtil.GetContentAndSubstringInput("<div class='newsitem_content'>", "<div class='spacer'></div><div class='newsmore'>", Source, out Source);
            string Author = HTMLParserUtil.GetContentAndSubstringInput("<div class='newsitem_author'>", "<div class='newsitem_date'>", Source, out Source, "", false);
            string Date = HTMLParserUtil.GetContentAndSubstringInput("<div class='newsitem_date'>", "</div>", Source, out Source);

            return new NewsLink(URL, ImageURL,Title, Content, Author, Date);
        }
    }
}

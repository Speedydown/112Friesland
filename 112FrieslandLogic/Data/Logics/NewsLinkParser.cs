using BaseLogic.HtmlUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _112FrieslandLogic.Data
{
    public static class NewsLinkParser
    {
        public static IList<NewsLink> GetNewsLinksFromSource(string Source)
        {
            //Remove unusable HeaderData
            Source = RemoveHeader(Source, "class=\"recent__item\"");

            //Parse Items
            return ParseContent(Source);
        }

        private static string RemoveHeader(string Input, string HTMLTextToCut)
        {
            try
            {
                return Input.Substring(HTMLParserUtil.GetPositionOfStringInHTMLSource(HTMLTextToCut, Input, true));

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
                    NewsLinksAsHTML.Add(HTMLParserUtil.GetContentAndSubstringInput("class=\"recent__details group\"", "class=\"recent__item\"", Input, out Input, "<div class=\"pagination group\">", true));
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
            Source = RemoveHeader(Source, "<ul class=\"recent__info\">");
            string Date = HTMLParserUtil.GetContentAndSubstringInput("<li class=\"recent__info-item\" itemprop=\"datePublished\" datetime=", "</li>", Source, out Source);
            string Author = HTMLParserUtil.GetContentAndSubstringInput("rel=\"author\">", "</a></u>", Source, out Source);
            //string Region = HTMLParserUtil.GetContentAndSubstringInput("<li class=\"recent__info-item\">", "</li>", Source, out Source);
            Source = RemoveHeader(Source, "<figure class=\"recent__figure\">");
            string URL = HTMLParserUtil.GetContentAndSubstringInput("<a href=\"", "\" title=", Source, out Source, "", false);
            string Title = HTMLParserUtil.GetContentAndSubstringInput("title=\"", "\"><img", Source, out Source);
            string ImageURL = HTMLParserUtil.GetContentAndSubstringInput("src=\"", "\" class", Source, out Source);

            string Location = string.Empty;
            string Content = string.Empty;

            try
            {
                Location = HTMLParserUtil.GetContentAndSubstringInput("<strong>", "</strong>", Source, out Source, "", false);
                Content = HTMLParserUtil.GetContentAndSubstringInput("</strong>", "<a href=", Source, out Source);
            }
            catch
            {
                string HTMLChar = Source.Contains("&#8211;") ? "&#8211;" : "&#x2013;";

                try
                {
                    Location = HTMLParserUtil.GetContentAndSubstringInput("<p>\r\n", HTMLChar, Source, out Source, "", false) + "-";
                }
                catch
                {
                    try
                    {
                        Location = HTMLParserUtil.GetContentAndSubstringInput("<p>\r\n", "- ", Source, out Source, "", false) + "-";
                    }
                    catch
                    {
                        try
                        {
                            Source = Source.Substring(HTMLParserUtil.GetPositionOfStringInHTMLSource("<div class=\"recent__body \" itemprop=\"text\">", Source, true));
                            Location = HTMLParserUtil.GetContentAndSubstringInput("<p>", HTMLChar, Source, out Source, "", false) + "-";
                        }
                        catch
                        {
                            try
                            {
                                Location = HTMLParserUtil.GetContentAndSubstringInput("<p>", "–", Source, out Source, "– ", false).Trim() + " -";
                            }
                            catch
                            {
                                try
                                {
                                    Location = HTMLParserUtil.GetContentAndSubstringInput("<p>", "- ", Source, out Source, "– ", false).Trim() + " -";
                                }
                                catch
                                {
                                    // Source = Source.Substring(HTMLParserUtil.GetPositionOfStringInHTMLSource("<div class=\"recent__body \" itemprop=\"text\">\n", Source, true));
                                    Location = HTMLParserUtil.GetContentAndSubstringInput("<p>", HTMLChar, Source, out Source, "– ", false).Trim() + " -";


                                }
                            }

                        }
                    }

                }

                try
                {
                    Content = HTMLParserUtil.GetContentAndSubstringInput(HTMLChar, "<a href=", Source, out Source);
                }
                catch
                {
                    try
                    {
                        Content = HTMLParserUtil.GetContentAndSubstringInput("- ", "<a href=", Source, out Source);
                    }
                    catch
                    {
                        Content = HTMLParserUtil.GetContentAndSubstringInput("– ", "<a href=", Source, out Source);
                    }
                }
            }

            return new NewsLink(URL, ImageURL, Title, Content, Author, Date, Location);
        }
    }
}

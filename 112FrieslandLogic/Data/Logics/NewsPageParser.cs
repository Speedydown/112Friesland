using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerTools;

namespace _112FrieslandLogic.Data
{
    public static class NewsPageParser
    {
        public static NewsPage GetNewsPageFromSource(string Source)
        {
            //Remove unusable HeaderData
            Source = RemoveHeader(Source, "<div class=\"content__header\">");

            //Parse Items
            return GetNewsPageFromHTML(Source);
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

        private static NewsPage GetNewsPageFromHTML(string Source)
        {
            string Header = HTMLParserUtil.GetContentAndSubstringInput("<h1 class=\"content__title content__title--bottom\">", "</h1>", Source, out Source);


            string ContentSummary = HTMLParserUtil.GetContentAndSubstringInput("<p>", "</p>", Source, out Source);
            List<string> Content = new List<string>();

            while (true)
            {
                try
                {
                    Content.Add(HTMLParserUtil.GetContentAndSubstringInput("<p>", "</p>", Source, out Source));
                }
                catch
                {
                    try
                    {
                        Content.Add(HTMLParserUtil.GetContentAndSubstringInput("<p style=\"text-align: left;\">", "</p>", Source, out Source));
                    }
                    catch
                    {
                        break;
                    }
                }
            }

            Source = RemoveHeader(Source, "<ul class=\"recent__info\">"); 
            string Date = HTMLParserUtil.GetContentAndSubstringInput("<li class=\"recent__info-item\" itemprop=\"datePublished\" datetime=", "</li>", Source, out Source, "");
            string Region = HTMLParserUtil.GetContentAndSubstringInput("<li class=\"recent__info-item\">", "</li>", Source, out Source);
            string Author = HTMLParserUtil.GetContentAndSubstringInput("<li class=\"recent__info-item\">", "</li>", Source, out Source);

            List<string> Images = GetImagesFromSource(Source);

            return new NewsPage(Header, ContentSummary, Content, Images, Author, Date);
        }

        private static List<string> GetImagesFromSource(string Source)
        {
            List<string> ImageList = new List<string>();

            //Clean garbage html
            Source = RemoveHeader(Source, "<ul class=\"content__gallery\"");

            while (Source.Length > 0)
            {
                try
                {
                    string ImageURL = HTMLParserUtil.GetContentAndSubstringInput("<a href=\"", "\" data-lightbox=\"gallery\"", Source, out Source, "\"  rel=\"lightbox[gal]");
                    ImageList.Add(ImageURL);
                }
                catch
                {
                    break;
                }
            }

            return ImageList;
        }
    }
}

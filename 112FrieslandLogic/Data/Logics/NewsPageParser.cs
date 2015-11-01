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
            string BackupSource = Source;
            string Header = HTMLParserUtil.GetContentAndSubstringInput("<h1 class=\"content__title content__title--bottom\">", "</h1>", Source, out Source);
            Source = RemoveHeader(Source, "itemprop=\"datePublished\"");
            string Date = HTMLParserUtil.GetContentAndSubstringInput(">", "</li>", Source, out Source, "");

            string ContentSummary = string.Empty;

            try
            {
                ContentSummary = HTMLParserUtil.GetContentAndSubstringInput("<p>", "</p>", Source, out Source);
            }
            catch
            {
                try
                {
                    ContentSummary = HTMLParserUtil.GetContentAndSubstringInput("<p class=\"western\">", "</p>", Source, out Source);
                }
                catch
                {
                    ContentSummary = HTMLParserUtil.GetContentAndSubstringInput("<p class=\"introductie\">", "</p>", Source, out Source);
                }
            }


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
                        try
                        {
                            Content.Add(HTMLParserUtil.GetContentAndSubstringInput("<p class=\"western\">", "</p>", Source, out Source));
                        }
                        catch
                        {
                            try
                            {
                                Content.Add(HTMLParserUtil.GetContentAndSubstringInput("<p class=\"introductie\">", "</p>", Source, out Source));
                                
                            }
                            catch
                            {
                                break;
                            }
                        }
                    }
                }
            }

            string Region = HTMLParserUtil.GetContentAndSubstringInput("<li class=\"recent__info-item\">", "</li>", Source, out Source);
            string Author = HTMLParserUtil.GetContentAndSubstringInput("<li class=\"recent__info-item\">", "</li>", Source, out Source);

            List<string> Images = GetImagesFromSource(BackupSource);

            return new NewsPage(Header, ContentSummary, Content, Images, Author, Date);
        }

        private static List<string> GetImagesFromSource(string Source)
        {
            List<string> ImageList = new List<string>();

            //Clean garbage html
            Source = RemoveHeader(Source, "<ul class=\"content__gallery\">");

            while (Source.Length > 0)
            {
                try
                {
                    string ImageURL = HTMLParserUtil.GetContentAndSubstringInput("\" href=\"", "\" data-rel", Source, out Source, string.Empty);
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

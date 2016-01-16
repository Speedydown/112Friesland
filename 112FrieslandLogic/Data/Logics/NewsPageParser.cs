using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerTools;

namespace _112FrieslandLogic.Data
{
    internal static class NewsPageParser
    {
        public async static Task<NewsPage> GetNewsPageFromSource(string Source)
        {
            //Remove unusable HeaderData
            Source = RemoveHeader(Source, "<div class=\"content__header\">");

            //Parse Items
            return await GetNewsPageFromHTML(Source);
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

        private static readonly string[] Paragraphs = new string[] { "<p>", "<p class=\"western\">", "<p class=\"introductie\">" };
        private static readonly string[] Content = new string[] { "<p>", "<p style=\"text-align: left;\">", "<p class=\"western\">", "<p class=\"introductie\">" };

        private async static Task<NewsPage> GetNewsPageFromHTML(string Source)
        {
            Task<List<string>> ImageTask = Task.Run(() => GetImagesFromSource(Source));

            string Header = HTMLParserUtil.GetContentAndSubstringInput("<h1 class=\"content__title content__title--bottom\">", "</h1>", Source, out Source);
            Source = RemoveHeader(Source, "itemprop=\"datePublished\"");
            string Date = HTMLParserUtil.GetContentAndSubstringInput(">", "</li>", Source, out Source, "");

            string ContentSummary = string.Empty;

            foreach (string p in Paragraphs)
            {

                try
                {
                    string backupSource = Source;
                    ContentSummary = HTMLParserUtil.GetContentAndSubstringInput(p, "</p>", Source, out Source);

                    if (ContentSummary == string.Empty)
                    {
                        Source = backupSource;
                        continue;
                    }

                    break;
                }
                catch
                {
                    continue;
                }
            }

            //Fix when divs are used
            if (ContentSummary == string.Empty)
            {
                Source = Source.Substring(HTMLParserUtil.GetPositionOfStringInHTMLSource("<div>", Source, false));
                ContentSummary = HTMLParserUtil.GetContentAndSubstringInput("<div>", "</div>", Source, out Source);
            }

            List<string> Content = new List<string>();

            bool Running = true;

            string BackupSourceForWhenDivsAreUsed = Source;

            while (Running)
            {
                string TempContent = null;

                foreach (string p in Paragraphs)
                {

                    try
                    {
                        TempContent = HTMLParserUtil.GetContentAndSubstringInput(p, "</p>", Source, out Source);
                        break;
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (TempContent == null)
                {
                    Running = false;
                }
                else if (TempContent != string.Empty)
                {
                    Content.Add(TempContent);
                }
            }

            //Fix when divs are used
            if (Content.Count == 0)
            {
                while (true)
                {
                    try
                    {

                        BackupSourceForWhenDivsAreUsed = BackupSourceForWhenDivsAreUsed.Substring(HTMLParserUtil.GetPositionOfStringInHTMLSource("<div>", BackupSourceForWhenDivsAreUsed, false));
                        string TempContent = HTMLParserUtil.GetContentAndSubstringInput("<div>", "</div>", BackupSourceForWhenDivsAreUsed, out BackupSourceForWhenDivsAreUsed);

                        if (TempContent.Contains("<"))
                        {
                            break;
                        }

                        Content.Add(TempContent);
                    }
                    catch
                    {
                        break;
                    }
                }
            }


            //string Region = HTMLParserUtil.GetContentAndSubstringInput("<li class=\"recent__info-item\">", "</li>", Source, out Source);
            Source = Source.Substring(HTMLParserUtil.GetPositionOfStringInHTMLSource("rel=\"author\">", Source, false));
            string Author = HTMLParserUtil.GetContentAndSubstringInput("rel=\"author\">", "</a></u>", Source, out Source);

            List<string> Images = await ImageTask;

            return new NewsPage(Header, ContentSummary, Content, Images, Author, Date);
        }

        private async static Task<List<string>> GetImagesFromSource(string Source)
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

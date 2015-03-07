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
            Source = RemoveHeader(Source);

            //Parse Items
            return ParseContent(Source);
        }

        private static string RemoveHeader(string Input)
        {
            try
            {
                return Input.Substring(GetPositionOFStringInSource("<div class='newsitems'>", Input));
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
                int StartIndex = GetPositionOFStringInSource("<div class='newsitem newsitem_published'><div class='newsitem_head'>", Input);

                if (StartIndex == -1)
                {
                    break;
                }

                int EndIndex = GetPositionOFStringInSource("</div><div class='spacer'></div></div>", Input, false);

                if (EndIndex == -1 || EndIndex <= StartIndex)
                {
                    break;
                }

                NewsLinksAsHTML.Add(Input.Substring(StartIndex, EndIndex - StartIndex));
                Input = Input.Substring(GetPositionOFStringInSource("</div><div class='spacer'></div></div>", Input));
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
            #region URL
            int StartIndexOFURL = GetPositionOFStringInSource("='news/", Source);

            if (StartIndexOFURL == -1)
            {
                throw new Exception("No URL");
            }

            int EndIndexOfURL = GetPositionOFStringInSource("' class='image_container'>", Source, false);

            if (EndIndexOfURL == -1 || StartIndexOFURL >= EndIndexOfURL)
            {
                throw new Exception("Invalid URL");
            }
            #endregion
            string URL = Source.Substring(StartIndexOFURL, EndIndexOfURL - StartIndexOFURL);

            #region ImageURL
            int StartIndexOFImageURL = GetPositionOFStringInSource("' src='", Source);

            if (StartIndexOFImageURL == -1)
            {
                throw new Exception("No URL");
            }

            int EndIndexOfImageURL = GetPositionOFStringInSource("' alt=''", Source, false);

            if (EndIndexOfImageURL == -1 || StartIndexOFImageURL >= EndIndexOfImageURL)
            {
                throw new Exception("Invalid URL");
            }
            #endregion
            string ImageURL = Source.Substring(StartIndexOFImageURL, EndIndexOfImageURL - StartIndexOFImageURL);


            return null;
        }

        private static int GetPositionOFStringInSource(string SearchQuery, string Source, bool GetEndoFStringIndex = true)
        {
            int Index = Source.IndexOf(SearchQuery);

            if (GetEndoFStringIndex)
                return (Index != -1) ?  Index + SearchQuery.Length : Index;

            return Index;
        }
    }
}

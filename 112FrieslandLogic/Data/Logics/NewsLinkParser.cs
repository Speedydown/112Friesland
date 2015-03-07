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
            return Input.Substring(GetPositionOFStringInSource("<div class='newsitems'>", Input));
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

            #region Title
            int StartIndexOFTitle = GetPositionOFStringInSource("class='newsitem_title'>", Source);

            if (StartIndexOFTitle == -1)
            {
                throw new Exception("No Title");
            }

            int EndIndexOFTitle = GetPositionOFStringInSource(" <img align='top'", Source, false);

            if (EndIndexOFTitle == -1)
            {
                EndIndexOFTitle = GetPositionOFStringInSource("</div>\n<div class='admin-icons'>", Source, false);
            }

            if (EndIndexOFTitle == -1 || StartIndexOFTitle >= EndIndexOFTitle)
            {
                throw new Exception("Invalid Title");
            }
            #endregion
            string Title = Source.Substring(StartIndexOFTitle, EndIndexOFTitle - StartIndexOFTitle);

            #region Content
            int StartIndexOFContent = GetPositionOFStringInSource("<div class='newsitem_content'>", Source);

            if (StartIndexOFContent == -1)
            {
                throw new Exception("No content");
            }

            int EndIndexOFContent = GetPositionOFStringInSource("<div class='spacer'></div><div class='newsmore'>", Source, false);

            if (EndIndexOFContent == -1 || StartIndexOFContent >= EndIndexOFContent)
            {
                throw new Exception("Invalid content");
            }
            #endregion
            string Content = Source.Substring(StartIndexOFContent, EndIndexOFContent - StartIndexOFContent);

            #region Author
            int StartIndexOFAuthor = GetPositionOFStringInSource("<div class='newsitem_author'>", Source);

            if (StartIndexOFAuthor == -1)
            {
                throw new Exception("No author");
            }

            int EndIndexOFAuthor = GetPositionOFStringInSource("<div class='newsitem_date'>", Source, false);

            if (EndIndexOFAuthor == -1 || StartIndexOFAuthor >= EndIndexOFAuthor)
            {
                throw new Exception("Invalid author");
            }
            #endregion
            string Author = Source.Substring(StartIndexOFAuthor, EndIndexOFAuthor - StartIndexOFAuthor);

            #region Date
            int StartIndexOfDate = GetPositionOFStringInSource("<div class='newsitem_date'>", Source);

            if (StartIndexOfDate == -1)
            {
                throw new Exception("No date");
            }

            Source = Source.Substring(StartIndexOfDate);
            StartIndexOfDate = 0;

            int EndIndexOfDate = GetPositionOFStringInSource("</div>", Source, false);

            if (EndIndexOfDate == -1 || StartIndexOfDate >= EndIndexOfDate)
            {
                throw new Exception("Invalid date");
            }
            #endregion
            string Date = Source.Substring(StartIndexOfDate, EndIndexOfDate - StartIndexOfDate);

            return new NewsLink(URL, ImageURL,Title, Content, Author, Date);
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

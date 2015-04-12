using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _112FrieslandLogic.Data
{
    public static class NewsPageParser
    {
        public static NewsPage GetNewsPageFromSource(string Source)
        {
            //Remove unusable HeaderData
            Source = RemoveHeader(Source);

            //Parse Items
            return GetNewsPageFromHTML(Source);
        }

        private static string RemoveHeader(string Input)
        {
            try
            {
                return Input.Substring(GetPositionOFStringInSource("class='newsitem_title'>", Input, false));
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static NewsPage GetNewsPageFromHTML(string Source)
        {
            #region Header
            int StartIndexOFHeader = GetPositionOFStringInSource("class='newsitem_title'>", Source);

            if (StartIndexOFHeader == -1)
            {
                throw new Exception("No Header");
            }

            int EndIndexOfHeader = GetPositionOFStringInSource(" <img align='top' src", Source, false);

            if (EndIndexOfHeader == -1)
            {
                EndIndexOfHeader = GetPositionOFStringInSource("</div>", Source, false);
            }
            
            if (EndIndexOfHeader == -1 || StartIndexOFHeader >= EndIndexOfHeader)
            {
                throw new Exception("Invalid Header");
            }
            #endregion

            string Header = Source.Substring(StartIndexOFHeader, EndIndexOfHeader - StartIndexOFHeader);
            Source = Source.Substring(GetPositionOFStringInSource("</div>", Source));

            #region ContentSummary
            int StartIndexOFContentSummary = GetPositionOFStringInSource("<div class='newsitem_content'><b>", Source);

            if (StartIndexOFContentSummary == -1)
            {
                throw new Exception("No Content summary");
            }

            int EndIndexOfContentSummary = GetPositionOFStringInSource("</b><br />", Source, false);

            if (EndIndexOfContentSummary == -1 || StartIndexOFContentSummary >= EndIndexOfContentSummary)
            {
                throw new Exception("Invalid Content summary");
            }
            #endregion

            string ContentSummary = Source.Substring(StartIndexOFContentSummary, EndIndexOfContentSummary - StartIndexOFContentSummary);
            Source = Source.Substring(GetPositionOFStringInSource("</b><br />", Source));

            #region Content
            int StartIndexOFContent = GetPositionOFStringInSource("</script><br /><br />", Source);

            if (StartIndexOFContent == -1)
            {
                StartIndexOFContent = GetPositionOFStringInSource("<br />", Source);

                if (StartIndexOFContent == -1)
                {
                    throw new Exception("No summary");
                }
            }
            

            int EndIndexOfContent = GetPositionOFStringInSource("<div class='swffile", Source, false);

            if (EndIndexOfContent == -1)
            {
                EndIndexOfContent = GetPositionOFStringInSource("</div>", Source, false);
            }

            if (EndIndexOfContent == -1 || StartIndexOFContent >= EndIndexOfContent)
            {
                throw new Exception("Invalid summary");
            }
            #endregion

            string Content = Source.Substring(StartIndexOFContent, EndIndexOfContent - StartIndexOFContent).Replace("<br />", "");
            Source = Source.Substring(GetPositionOFStringInSource("</div>", Source));

            #region Clean \n
            while (true)
            {
                string temp = Content.Substring(Content.Length - 1);

                if (temp == "\n")
                {
                    Content = Content.Substring(0, Content.Length - 1);
                }
                else
                {
                    break;
                }
            }
            #endregion

            #region Author
            int StartIndexOFAuthor = GetPositionOFStringInSource("<div class='newsitem_author'>", Source);

            if (StartIndexOFAuthor == -1)
            {
                throw new Exception("No Author");
            }

            int EndIndexOfAuthor = GetPositionOFStringInSource("<div class='newsitem_date'>", Source, false);

            if (EndIndexOfAuthor == -1 || StartIndexOFAuthor >= EndIndexOfAuthor)
            {
                throw new Exception("Invalid Author");
            }
            #endregion

            string Author = Source.Substring(StartIndexOFAuthor, EndIndexOfAuthor - StartIndexOFAuthor);
            Source = Source.Substring(EndIndexOfAuthor);

            #region Date
            int StartIndexOFDate = GetPositionOFStringInSource("<div class='newsitem_date'>", Source);

            if (StartIndexOFDate == -1)
            {
                throw new Exception("No Date");
            }

            int EndIndexOfDate = GetPositionOFStringInSource("</div>", Source, false);

            if (EndIndexOfDate == -1 || StartIndexOFDate >= EndIndexOfDate)
            {
                throw new Exception("Invalid Date");
            }
            #endregion

            string Date = Source.Substring(StartIndexOFDate, EndIndexOfDate - StartIndexOFDate);
            Source = Source.Substring(GetPositionOFStringInSource("</div>", Source));

            List<string> Images = GetImagesFromSource(Source);

            return new NewsPage(Header, ContentSummary, Content, Images, Author, Date);
        }

        private static List<string> GetImagesFromSource(string Source)
        {
            List<string> ImageList = new List<string>();

            //Clean garbage html
            Source = Source.Substring(GetPositionOFStringInSource("<div id='lightbox_gallery_controls_rightside'></div>", Source));

            while (Source.Length > 0)
            {
                int StartIndexOFImage = GetPositionOFStringInSource("LightBoxGalleryRegisterImage(\"", Source);
                int EndIndexOfImage = GetPositionOFStringInSource("\", \"\", \"\")", Source, false);

                if (StartIndexOFImage == -1 || EndIndexOfImage == -1 || StartIndexOFImage > EndIndexOfImage)
                {
                    break;
                }

                ImageList.Add(Source.Substring(StartIndexOFImage, EndIndexOfImage - StartIndexOFImage));
                Source = Source.Substring(GetPositionOFStringInSource("\", \"\", \"\")", Source));
            }

            return ImageList;
        }

        private static int GetPositionOFStringInSource(string SearchQuery, string Source, bool GetEndoFStringIndex = true)
        {
            int Index = Source.IndexOf(SearchQuery);

            if (GetEndoFStringIndex)
                return (Index != -1) ? Index + SearchQuery.Length : Index;

            return Index;
        }


    }
}

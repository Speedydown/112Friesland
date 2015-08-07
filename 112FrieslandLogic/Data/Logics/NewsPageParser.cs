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
            Source = RemoveHeader(Source, "class='newsitem_title'>");

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
            string Header = string.Empty;

            try
            {
                Header = HTMLParserUtil.GetContentAndSubstringInput("class='newsitem_title'>", " <img align='top' src", Source, out Source);
            }
            catch
            {
                Header = HTMLParserUtil.GetContentAndSubstringInput("class='newsitem_title'>", "</div>", Source, out Source);
            }

            string ContentSummary = HTMLParserUtil.GetContentAndSubstringInput("<div class='newsitem_content'><b>", "</b><br />", Source, out Source);
            string Content = string.Empty;

            try
            {
                Content = HTMLParserUtil.GetContentAndSubstringInput("</script><br /><br />", "<div class='swffile", Source, out Source);
            }
            catch
            {
                try
                {
                    Content = HTMLParserUtil.GetContentAndSubstringInput("<br />", "<div class='swffile", Source, out Source);
                }
                catch
                {
                    try
                    {
                        Content = HTMLParserUtil.GetContentAndSubstringInput("</script><br /><br />", "</div>", Source, out Source);
                    }
                    catch
                    {
                        Content = HTMLParserUtil.GetContentAndSubstringInput("<br />", "</div>", Source, out Source);
                    }
                }
            }

            string Author = HTMLParserUtil.GetContentAndSubstringInput("<div class='newsitem_author'>", "<div class='newsitem_date'>", Source, out Source,"", false);
            string Date = HTMLParserUtil.GetContentAndSubstringInput("<div class='newsitem_date'>", "</div>", Source, out Source);

            List<string> Images = GetImagesFromSource(Source);

            return new NewsPage(Header, ContentSummary, Content, Images, Author, Date);
        }

        private static List<string> GetImagesFromSource(string Source)
        {
            List<string> ImageList = new List<string>();

            //Clean garbage html
            Source = RemoveHeader(Source, "<div id='lightbox_gallery_controls_rightside'></div>");

            while (Source.Length > 0)
            {
                try
                {
                    string ImageURL = HTMLParserUtil.GetContentAndSubstringInput("LightBoxGalleryRegisterImage(\"", "\", \"\", \"\")", Source, out Source);
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

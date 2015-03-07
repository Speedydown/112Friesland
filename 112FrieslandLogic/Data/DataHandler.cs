using _112FrieslandLogic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace _112FrieslandLogic
{
    public static class DataHandler
    {
        private static async Task<IList<NewsLink>> GetEmptyNewsLinkList()
        {
            return new List<NewsLink>();
        }

        public static IAsyncOperation<IList<NewsLink>> GetNewsLinksByPage(int PageNumber)
        {
            IList<NewsLink> NewsList = new List<NewsLink>();

            try
            {
                return GetNewsLinksByPageHelper(PageNumber).AsAsyncOperation();
            }
            catch
            {
                return GetEmptyNewsLinkList().AsAsyncOperation();
            }
        }

        private static async Task<IList<NewsLink>> GetNewsLinksByPageHelper(int PageNumber)
        {
            string PageSource = await GetDataFromURL("http://www.112fryslan.nl/Home?&action=show&startat=" + ((PageNumber - 1) * 10));

            return NewsLinkParser.GetNewsLinksFromSource(PageSource);
        }

        private static async Task<string> GetDataFromURL(string URL)
        {
            string Output = string.Empty;

            try
            {
                var client = new System.Net.Http.HttpClient();

                var response = await client.GetAsync(new Uri(URL));

                var ByteArray = await response.Content.ReadAsByteArrayAsync();
                Output = Encoding.GetEncoding("iso-8859-1").GetString(ByteArray, 0, ByteArray.Length);
            }
            catch (Exception)
            {

            }

            return Output;
        }
    }
}

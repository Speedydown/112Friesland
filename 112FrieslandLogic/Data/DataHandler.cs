using _112FrieslandLogic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;

namespace _112FrieslandLogic
{
    public static class DataHandler
    {
        private static readonly Random Randomizer = new Random();
        private static readonly SemaphoreSlim Locker = new SemaphoreSlim(1,1);

        public static IAsyncOperation<IList<NewsLink>> GetNewsLinksByPage(int PageNumber)
        {
            return GetNewsLinksByPageHelper(PageNumber).AsAsyncOperation();
        }

        private static async Task<IList<NewsLink>> GetNewsLinksByPageHelper(int PageNumber)
        {
            string PageSource = await GetDataFromURL("https://www.112fryslan.nl/page/" + PageNumber);
            return NewsLinkParser.GetNewsLinksFromSource(PageSource);
        }

        public static IAsyncOperation<NewsPage> GetNewsPageFromURL(string URL)
        {
            return GetNewsPageFromURLHelper(URL).AsAsyncOperation();
        }

        private static async Task<NewsPage> GetNewsPageFromURLHelper(string URL)
        {
            string PageSource = await GetDataFromURL(URL);

            return await NewsPageParser.GetNewsPageFromSource(PageSource);
        }

        public static IAsyncOperation<string> GetDataFromURL(string URL)
        {
            return GetDataFromURLHelper(URL).AsAsyncOperation();
        }

        private static async Task<string> GetDataFromURLHelper(string URL)
        {
            await Locker.WaitAsync();
            string Output = string.Empty;

            try
            {
                var client = new System.Net.Http.HttpClient();

                var response = await client.GetAsync(new Uri(URL));

                var ByteArray = await response.Content.ReadAsByteArrayAsync();
                //Output = Encoding.GetEncoding("iso-8859-1").GetString(ByteArray, 0, ByteArray.Length);
                Output = Encoding.UTF8.GetString(ByteArray, 0, ByteArray.Length);
            }
            catch (Exception)
            {

            }

            Locker.Release();
            return Output;
        }
    }
}

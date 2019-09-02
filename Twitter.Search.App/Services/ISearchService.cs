using System.Threading.Tasks;

namespace Twitter.Search.App.Services
{
    public interface ISearchService
    {
        /// <summary>
        ///     Request data from the Twitter API
        /// </summary>
        /// <param name="q">query to send to the Twitter API</param>
        /// <returns>JSON string</returns>
        Task<string> GetData(string q);
    }
}
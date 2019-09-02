using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Twitter.Search.App.Models;
using Twitter.Search.App.Services;

namespace Twitter.Search.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly ISearchService _searchService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options">Options to serialize to <see cref="AppSettings"/></param>
        /// <param name="searchService">Implementation of <see cref="ISearchService"/></param>
        public HomeController(IOptions<AppSettings> options, ISearchService searchService)
        {
            _appSettings = (AppSettings)options.Value;
            _searchService = searchService;
        }

        /// <summary>
        ///     Default page
        /// </summary>
        /// <param name="ht">Hashtag to look for, will default to appsettings value if null</param>
        /// <returns></returns>
        public IActionResult Index(string ht = null)
        {
            ViewData["HashTag"] = ht ?? _appSettings.HashTag;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        /// <summary>
        ///     Get tweets for a certain hashtag
        /// </summary>
        /// <param name="ht">Hashtag to look for</param>
        /// <returns></returns>
        /// <remarks>
        ///     Response is cached for 5 minutes bashed on query key.
        ///     If there's no hashtag provided in the querystring, it will default to the hashtag in the appsettings.
        /// </remarks>
        [ResponseCache(Duration = 300, VaryByQueryKeys = new string[] {"ht"})]
        public JsonResult GetTweets(string ht = null)
        {
            var searchFor = ht ?? _appSettings.HashTag;

            var query = $"?q=%23{searchFor}&result_type=recent";

            var result = QueryTweets(query);

            return Json(result);
        }

        /// <summary>
        ///     Call Search Service and Query Twitter API
        /// </summary>
        /// <param name="query">Query to send to the Twitter API</param>
        /// <returns>List of Tweets, excluding retweets</returns>
        private List<Status> QueryTweets(string query)
        {
            var userStatusCollection = new List<Status>();
            var userStatusQuery = _searchService.GetData(query).Result;
            var userStatusObj = JsonConvert.DeserializeObject<TweetSearchResponse>(userStatusQuery);
            userStatusCollection.AddRange(userStatusObj.Statuses.Where(x => x.RetweetedStatus == null)); //  filter out retweets

            if (string.IsNullOrEmpty(userStatusObj.SearchMetadata?.NextResults))
                return userStatusCollection;
            else
                userStatusCollection.AddRange(QueryTweets($"{userStatusObj.SearchMetadata.NextResults}&tweet_mode=extended"));

            return userStatusCollection;
        }

    }
}

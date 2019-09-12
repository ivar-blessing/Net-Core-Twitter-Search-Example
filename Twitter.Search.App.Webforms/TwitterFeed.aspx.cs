using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace Twitter.Search.App.Webforms
{
    /// <summary>
    ///     Query twitter feed, more information on the API on https://developer.twitter.com/en/docs/tweets/search/api-reference/get-search-tweets.html
    /// </summary>
    public partial class TwitterFeed : System.Web.UI.Page
    {
        protected string HashTag { get; set; }
        protected string JsonFeed { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetHashTag();
            RegisterAsyncTask(new PageAsyncTask(GetData));
        }

        /// <summary>
        ///     Set the hashtag to search for
        /// </summary>
        private void SetHashTag()
        {
            HashTag = Request.QueryString["ht"];

            if (string.IsNullOrEmpty(HashTag))
            {
                HashTag = ConfigurationManager.AppSettings["DefaultHashTag"];
            }
        }


        /// <summary>
        ///  Setup the client and query
        /// </summary>
        private async Task GetData()
        {
            var cacheKey = $"twitter_cache_{HashTag}";

            if (Cache[cacheKey] == null)
            {
                var query = $"?q=%23{HashTag}&result_type=recent"; // mixed, recent, popular

                var result = await QueryTweets(query);

                JsonFeed = JsonConvert.SerializeObject(result);

                Cache.Insert(cacheKey, JsonFeed, null, DateTime.Now.AddSeconds(30), TimeSpan.Zero);
            }
            else
            {
                JsonFeed = Cache.Get(cacheKey).ToString();
            }

            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write(JsonFeed);
            Response.End();
        }

        /// <summary>
        ///     Call Search Service and Query Twitter API
        /// </summary>
        /// <param name="query">Query to send to the Twitter API</param>
        /// <returns>List of Tweets, excluding retweets</returns>
        private async Task<List<Status>> QueryTweets(string query)
        {
            var userStatusCollection = new List<Status>();
            var userStatusQuery = await GetDataFromTwitter(query);
            var userStatusObj = JsonConvert.DeserializeObject<TweetSearchResponse>(userStatusQuery);
            userStatusCollection.AddRange(userStatusObj.Statuses);//.Where(x => x.RetweetedStatus == null)); //  filter out retweets

            if (string.IsNullOrEmpty(userStatusObj.SearchMetadata?.NextResults))
                return userStatusCollection;
            else
                userStatusCollection.AddRange(await QueryTweets($"{userStatusObj.SearchMetadata.NextResults}&tweet_mode=extended"));

            return userStatusCollection;
        }

        /// <summary>
        ///     Get access token from the Twitter API
        /// </summary>
        /// <returns></returns>
        /// <remarks>Not used currently as token is read from AppSettings</remarks>
        private async Task<string> GetAccessToken()
        {
            var authToken = Base64Encode($"{ConfigurationManager.AppSettings["ConsumerKey"]}:{ConfigurationManager.AppSettings["ConsumerSecret"]}");
            var token = string.Empty;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {authToken}");
                //_client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");

                IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>> {
                { new KeyValuePair<string, string>("grant_type", "client_credentials") } };

                var result = await client.PostAsync("https://api.twitter.com/oauth2/token", new FormUrlEncodedContent(nameValueCollection));

                // Add this somewhere in your project, most likely in Startup.cs
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                TokenModel tm = JsonConvert.DeserializeObject<TokenModel>(await result.Content.ReadAsStringAsync());

                token = tm.AccessToken;
            }

            return token;
        }

        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public async Task<string> GetDataFromTwitter(string q)
        {
            var bearerToken = await GetAccessToken();

            if (string.IsNullOrEmpty(bearerToken))
            {
                throw new Exception("Invalid bearer token, set in Appsettings or call GetAccessToken method.");
            }

            using (var client = new HttpClient
            {
                BaseAddress = new Uri("https://api.twitter.com/1.1/search/tweets.json"),
            })
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
                //client.DefaultRequestHeaders.Add("Content-Type", "application/json");

                var response = await client.GetAsync(q);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    return result;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        #region Private classes
        #region Twitter Response classes
        /// <summary>
        ///     Response from a query to the Twitter API
        /// </summary>
        public partial class TweetSearchResponse
        {
            public Status[] Statuses { get; set; }
            public SearchMetadata SearchMetadata { get; set; }
        }

        public partial class SearchMetadata
        {
            [JsonProperty(PropertyName = "completed_in")]
            public double CompletedIn { get; set; }

            [JsonProperty(PropertyName = "max_id")]
            public double MaxId { get; set; }

            [JsonProperty(PropertyName = "max_id_str")]
            public string MaxIdStr { get; set; }

            [JsonProperty(PropertyName = "next_results")]
            public string NextResults { get; set; }
            public string Query { get; set; }

            [JsonProperty(PropertyName = "refresh_url")]
            public string RefreshUrl { get; set; }
            public long Count { get; set; }

            [JsonProperty(PropertyName = "since_id")]
            public long SinceId { get; set; }

            [JsonProperty(PropertyName = "since_id_str")]
            public long SinceIdStr { get; set; }
        }

        public partial class Status
        {
            [JsonProperty(PropertyName = "created_at")]
            public string CreatedAt { get; set; }
            public double Id { get; set; }

            [JsonProperty(PropertyName = "id_str")]
            public string IdStr { get; set; }

            public string Text { get; set; }
            public bool Truncated { get; set; }
            public StatusEntities Entities { get; set; }
            public Metadata Metadata { get; set; }
            public string Source { get; set; }

            [JsonProperty(PropertyName = "in_reply_to_status_id")]
            public object InReplyToStatusId { get; set; }

            [JsonProperty(PropertyName = "in_reply_to_status_id_str")]
            public object InReplyToStatusIdStr { get; set; }

            [JsonProperty(PropertyName = "in_reply_to_user_id")]
            public object InReplyToUserId { get; set; }

            [JsonProperty(PropertyName = "in_reply_to_user_id_str")]
            public object InReplyToUserIdStr { get; set; }

            [JsonProperty(PropertyName = "in_reply_to_screen_name")]
            public object InReplyToScreenName { get; set; }

            [JsonProperty(PropertyName = "user")]
            public User User { get; set; }
            public object Geo { get; set; }
            public object Coordinates { get; set; }
            public object Place { get; set; }
            public object Contributors { get; set; }

            [JsonProperty(PropertyName = "retweeted_status")]
            public RetweetedStatus RetweetedStatus { get; set; }

            [JsonProperty(PropertyName = "is_quote_status")]
            public bool IsQuoteStatus { get; set; }

            [JsonProperty(PropertyName = "retweet_count")]
            public long RetweetCount { get; set; }

            [JsonProperty(PropertyName = "favorite_count")]
            public long FavoriteCount { get; set; }
            public bool Favorited { get; set; }
            public bool Retweeted { get; set; }
            public string Lang { get; set; }

            [JsonProperty(PropertyName = "possibly_sensitive")]
            public bool? PossiblySensitive { get; set; }

            [JsonProperty(PropertyName = "is_retweet")]
            public bool IsRetweet => RetweetedStatus != null;
        }

        public partial class StatusEntities
        {
            public object[] Hashtags { get; set; }
            public object[] Symbols { get; set; }

            [JsonProperty(PropertyName = "user_mentions")]
            public UserMention[] UserMentions { get; set; }
            public Url[] Urls { get; set; }
        }

        public partial class Url
        {
            [JsonProperty(PropertyName = "url")]
            public Uri UrlUrl { get; set; }

            [JsonProperty(PropertyName = "expanded_url")]
            public Uri ExpandedUrl { get; set; }

            [JsonProperty(PropertyName = "display_url")]
            public string DisplayUrl { get; set; }
            public long[] Indices { get; set; }
        }

        public partial class UserMention
        {
            [JsonProperty(PropertyName = "screen_name")]
            public string ScreenName { get; set; }
            public string Name { get; set; }
            public long Id { get; set; }

            [JsonProperty(PropertyName = "id_str")]
            public long IdStr { get; set; }
            public long[] Indices { get; set; }
        }

        public partial class Metadata
        {
            [JsonProperty(PropertyName = "iso_language_code")]
            public string IsoLanguageCode { get; set; }

            [JsonProperty(PropertyName = "result_type")]
            public string ResultType { get; set; }
        }

        public partial class RetweetedStatus
        {
            [JsonProperty(PropertyName = "created_at")]
            public string CreatedAt { get; set; }
            public double Id { get; set; }

            [JsonProperty(PropertyName = "id_str")]
            public string IdStr { get; set; }
            public string Text { get; set; }
            public bool Truncated { get; set; }
            public StatusEntities Entities { get; set; }
            public Metadata Metadata { get; set; }
            public string Source { get; set; }

            [JsonProperty(PropertyName = "in_reply_to_status_id")]
            public object InReplyToStatusId { get; set; }

            [JsonProperty(PropertyName = "in_reply_to_status_id_str")]
            public object InReplyToStatusIdStr { get; set; }

            [JsonProperty(PropertyName = "in_reply_to_user_id")]
            public object InReplyToUserId { get; set; }

            [JsonProperty(PropertyName = "in_reply_to_user_id_str")]
            public object InReplyToUserIdStr { get; set; }

            [JsonProperty(PropertyName = "in_reply_to_screen_name")]
            public object InReplyToScreenName { get; set; }

            [JsonProperty(PropertyName = "user")]
            public User User { get; set; }
            public object Geo { get; set; }
            public object Coordinates { get; set; }
            public object Place { get; set; }
            public object Contributors { get; set; }

            [JsonProperty(PropertyName = "is_quote_status")]
            public bool IsQuoteStatus { get; set; }

            [JsonProperty(PropertyName = "retweet_count")]
            public long RetweetCount { get; set; }

            [JsonProperty(PropertyName = "favorite_count")]
            public long FavoriteCount { get; set; }
            public bool Favorited { get; set; }
            public bool Retweeted { get; set; }

            [JsonProperty(PropertyName = "possibly_sensitive")]
            public bool PossiblySensitive { get; set; }
            public string Lang { get; set; }
        }

        public partial class User
        {
            public long Id { get; set; }

            [JsonProperty(PropertyName = "id_str")]
            public string IdStr { get; set; }
            public string Name { get; set; }

            [JsonProperty(PropertyName = "screen_name")]
            public string ScreenName { get; set; }
            public string Location { get; set; }
            public string Description { get; set; }
            public Uri Url { get; set; }
            public UserEntities Entities { get; set; }
            public bool Protected { get; set; }

            [JsonProperty(PropertyName = "followers_count")]
            public long FollowersCount { get; set; }

            [JsonProperty(PropertyName = "friends_count")]
            public long FriendsCount { get; set; }

            [JsonProperty(PropertyName = "listed_count")]
            public long ListedCount { get; set; }

            [JsonProperty(PropertyName = "created_at")]
            public string CreatedAt { get; set; }

            [JsonProperty(PropertyName = "favourites_count")]
            public long FavouritesCount { get; set; }

            [JsonProperty(PropertyName = "utc_offset")]
            public object UtcOffset { get; set; }

            [JsonProperty(PropertyName = "time_zone")]
            public object TimeZone { get; set; }

            [JsonProperty(PropertyName = "geo_enabled")]
            public bool GeoEnabled { get; set; }
            public bool Verified { get; set; }

            [JsonProperty(PropertyName = "statuses_count")]
            public long StatusesCount { get; set; }
            public object Lang { get; set; }

            [JsonProperty(PropertyName = "contributers_enabled")]
            public bool ContributorsEnabled { get; set; }

            [JsonProperty(PropertyName = "is_translator")]
            public bool IsTranslator { get; set; }

            [JsonProperty(PropertyName = "is_translation_enabled")]
            public bool IsTranslationEnabled { get; set; }

            [JsonProperty(PropertyName = "profile_background_color")]
            public string ProfileBackgroundColor { get; set; }

            [JsonProperty(PropertyName = "profile_background_image_url")]
            public Uri ProfileBackgroundImageUrl { get; set; }

            [JsonProperty(PropertyName = "profile_background_image_url_https")]
            public Uri ProfileBackgroundImageUrlHttps { get; set; }

            [JsonProperty(PropertyName = "profile_background_tile")]
            public bool ProfileBackgroundTile { get; set; }

            [JsonProperty(PropertyName = "profile_image_url")]
            public Uri ProfileImageUrl { get; set; }

            [JsonProperty(PropertyName = "profile_image_url_https")]
            public Uri ProfileImageUrlHttps { get; set; }

            [JsonProperty(PropertyName = "profile_banner_url")]
            public Uri ProfileBannerUrl { get; set; }

            [JsonProperty(PropertyName = "profile_link_color")]
            public string ProfileLinkColor { get; set; }

            [JsonProperty(PropertyName = "profile_sidebar_border_color")]
            public string ProfileSidebarBorderColor { get; set; }

            [JsonProperty(PropertyName = "profile_sidebar_fill_color")]
            public string ProfileSidebarFillColor { get; set; }

            [JsonProperty(PropertyName = "profile_text_color")]
            public string ProfileTextColor { get; set; }

            [JsonProperty(PropertyName = "profile_use_background_image")]
            public bool ProfileUseBackgroundImage { get; set; }

            [JsonProperty(PropertyName = "has_extended_profile")]
            public bool HasExtendedProfile { get; set; }

            [JsonProperty(PropertyName = "default_profile")]
            public bool DefaultProfile { get; set; }

            [JsonProperty(PropertyName = "default_profile_image")]
            public bool DefaultProfileImage { get; set; }
            public object Following { get; set; }

            [JsonProperty(PropertyName = "follow_request_sent")]
            public object FollowRequestSent { get; set; }
            public object Notifications { get; set; }

            [JsonProperty(PropertyName = "translator_type")]
            public string TranslatorType { get; set; }
        }

        public partial class UserEntities
        {
            public Description Url { get; set; }
            public Description Description { get; set; }
        }

        public partial class Description
        {
            public Url[] Urls { get; set; }
        }
        #endregion
        #region Token model
        /// <summary>
        ///  Token model used to serialize the response with authenticating the app with Twitter
        /// </summary>
        public class TokenModel
        {
            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
        }
        #endregion
        #endregion
    }
}
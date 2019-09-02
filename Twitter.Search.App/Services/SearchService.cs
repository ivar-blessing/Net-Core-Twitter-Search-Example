using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Twitter.Search.App.Models;

namespace Twitter.Search.App.Services
{
    public class SearchService : ISearchService
    {
        private HttpClient _client;
        private readonly AppSettings _appSettings;
        private string _authToken = null;

        public SearchService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            GetAccessToken();
            CreateTwitterClient();
        }

        /// <summary>
        ///     Create a new client with authentication for the Twitter API
        /// </summary>
        private void CreateTwitterClient()
        {
            var bearerToken = _authToken ?? _appSettings.AuthorizeToken;

            if (string.IsNullOrEmpty(bearerToken))
            {
                throw new Exception("Invalid bearer token, set in Appsettings or call GetAccessToken method.");
            }

            _client = new HttpClient
            {
                BaseAddress = new Uri("https://api.twitter.com/1.1/search/tweets.json"),
            };
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
            //_client.DefaultRequestHeaders.Add("Content-Type", "application/json");
        }

        /// <summary>
        ///     Get access token from the Twitter API
        /// </summary>
        /// <returns></returns>
        /// <remarks>Not used currently as token is read from AppSettings</remarks>
        private void GetAccessToken()
        {
            var authToken = Base64Encode($"{_appSettings.Consumer.Key}:{_appSettings.Consumer.Secret}");

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", $"Basic {authToken}");
            //_client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");

            IList<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>> {
                { new KeyValuePair<string, string>("grant_type", "client_credentials") } };

            var result = _client.PostAsync("https://api.twitter.com/oauth2/token", new FormUrlEncodedContent(nameValueCollection)).Result;

            TokenModel tm = JsonConvert.DeserializeObject<TokenModel>(result.Content.ReadAsStringAsync().Result);

            _authToken = tm.AccessToken;
        }

        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public async Task<string> GetData(string q)
        {
            var result = await _client.GetStringAsync(q);
            return result;
        }
    }
}

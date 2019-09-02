using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Twitter.Search.App.Models
{
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
}
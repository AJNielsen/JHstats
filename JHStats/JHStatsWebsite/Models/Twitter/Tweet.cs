using Newtonsoft.Json;
using System;

namespace JHStatsWebsite.Models.Twitter
{
    public class Tweet
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("entities")]
        public TweetEntities Entities { get; set; }
    }
}

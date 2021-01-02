using Newtonsoft.Json;

namespace JHStatsWebsite.Models.Twitter
{
    public class TweetEntities
    {
        [JsonProperty("hashtags")]
        public Hashtag[] Hashtags { get; set; }
        [JsonProperty("urls")]
        public Url[] Urls { get; set; }
    }
}

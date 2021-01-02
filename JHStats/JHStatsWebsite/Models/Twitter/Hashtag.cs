using Newtonsoft.Json;

namespace JHStatsWebsite.Models.Twitter
{
    public class Hashtag
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }
    }
}

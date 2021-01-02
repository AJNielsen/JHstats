using Newtonsoft.Json;

namespace JHStatsWebsite.Models.Twitter
{
    public class Url
    {
        [JsonProperty("expanded_url")]
        public string ExpandedUrl { get; set; }
    }
}

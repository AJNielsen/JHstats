using Newtonsoft.Json;

namespace JHStatsWebsite.Models.Twitter
{
    /// <summary>
    /// Represents an individual tweet from the sample stream.
    /// Structure can be found here: https://developer.twitter.com/en/docs/twitter-api/tweets/sampled-stream/api-reference/get-tweets-sample-stream
    /// </summary>
    public class TweetData
    {
        [JsonProperty("data")]
        public Tweet Data { get; set; }
    }
}

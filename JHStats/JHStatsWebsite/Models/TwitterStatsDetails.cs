using System.Collections.Generic;

namespace JHStatsWebsite.Models
{
    public class TwitterStatsDetails
    {
        public int TotalTweets { get; set; }
        public Averages Averages { get; set; }
        public double PercentageOfTweetsWithEmojis { get; set; }
        public IEnumerable<KeyValuePair<string, int>> TopEmojis { get; set; }
        public double PercentageOfTweetsContainsUrl { get; set; }
        public double PercentageOfTweetsContainsImage { get; set; }
        public IEnumerable<KeyValuePair<string, int>> TopDomains { get; set; }
    }
}

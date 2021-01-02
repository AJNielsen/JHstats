using JHStatsWebsite.Data.Interfaces;
using JHStatsWebsite.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace JHStatsWebsite.Data
{

    public class TwitterEstimatedLiveDataRetriever : TwitterDataRetriverBase, ITwitterEstimatedLiveDataRetriever
    {

        public TwitterEstimatedLiveDataRetriever(ITwitterDataStorage twitterDataInMemoryStorage) : base(twitterDataInMemoryStorage) { }

        public TwitterStatsDetails GetStats()
        {
            if (_twitterDataInMemoryStorage.TweetTimeDictionary.Count == 0)
            {
                return new TwitterStatsDetails();
            }

            var topDomains = GetTopKeysWithCount(_twitterDataInMemoryStorage.DomainCountDictionary);
            var topEmojis = GetTopKeysWithCount(_twitterDataInMemoryStorage.EmojiCountDictionary);

            return new TwitterStatsDetails
            {
                Averages = new Averages
                {
                    TweetsPerHour = Math.Round(GetAverageTweetsPerTimeSpan(Hour) * 100,2),
                    TweetsPerMinute = Math.Round(GetAverageTweetsPerTimeSpan(Minute) * 100,2),
                    TweetsPerSecond = Math.Round(GetAverageTweetsPerTimeSpan(Second) * 100,2)
                },
                PercentageOfTweetsContainsImage = GetPercentageOfTotalTweets(_twitterDataInMemoryStorage.TweetsWithImageUrl),
                PercentageOfTweetsContainsUrl = GetPercentageOfTotalTweets(_twitterDataInMemoryStorage.TweetsWithAnyUrl),
                PercentageOfTweetsWithEmojis = GetPercentageOfTotalTweets(_twitterDataInMemoryStorage.TweetsWithEmojis),
                TopDomains = topDomains,
                TopEmojis = topEmojis,
                TotalTweets = _twitterDataInMemoryStorage.TotalTweetCount * 100
            };

        }

        protected override IEnumerable<KeyValuePair<string, int>> GetTopKeysWithCount(ConcurrentDictionary<string, int> dictionary)
        {
            return base.GetTopKeysWithCount(dictionary).Select(tk => new KeyValuePair<string, int>(tk.Key, tk.Value * 100));
        }
    }
}

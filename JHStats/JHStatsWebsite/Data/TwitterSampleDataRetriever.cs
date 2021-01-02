using JHStatsWebsite.Data.Interfaces;
using JHStatsWebsite.Models;
using System;

namespace JHStatsWebsite.Data
{
    public class TwitterSampleDataRetriever : TwitterDataRetriverBase, ITwitterSampleDataRetriever
    {
        public TwitterSampleDataRetriever(ITwitterDataStorage twitterDataInMemoryStorage) : base(twitterDataInMemoryStorage) { }

        public TwitterStatsDetails GetStats()
        {
            if (_twitterDataInMemoryStorage.TweetTimeDictionary.Count == 0)
            {
                return new TwitterStatsDetails();
            }

            var topDomains = GetTopKeysWithCount(_twitterDataInMemoryStorage.DomainCountDictionary);
            var topEmojis = GetTopKeysWithCount(_twitterDataInMemoryStorage.EmojiCountDictionary);

            return new TwitterStatsDetails()
            {
                Averages = new Averages()
                {
                    TweetsPerHour = Math.Round(GetAverageTweetsPerTimeSpan(Hour), 2),
                    TweetsPerMinute = Math.Round(GetAverageTweetsPerTimeSpan(Minute), 2),
                    TweetsPerSecond = Math.Round(GetAverageTweetsPerTimeSpan(Second), 2)
                },
                PercentageOfTweetsContainsImage = GetPercentageOfTotalTweets(_twitterDataInMemoryStorage.TweetsWithImageUrl),
                PercentageOfTweetsContainsUrl = GetPercentageOfTotalTweets(_twitterDataInMemoryStorage.TweetsWithAnyUrl),
                PercentageOfTweetsWithEmojis = GetPercentageOfTotalTweets(_twitterDataInMemoryStorage.TweetsWithEmojis),
                TopDomains = topDomains,
                TopEmojis = topEmojis,
                TotalTweets = _twitterDataInMemoryStorage.TotalTweetCount
            };
        }
    }
}

using JHStatsWebsite.Data.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace JHStatsWebsite.Data
{
    public abstract class TwitterDataRetriverBase
    {
        protected readonly TimeSpan Second = TimeSpan.FromSeconds(1);
        protected readonly TimeSpan Minute = TimeSpan.FromMinutes(1);
        protected readonly TimeSpan Hour = TimeSpan.FromHours(1);

        protected ITwitterDataStorage _twitterDataInMemoryStorage;

        public TwitterDataRetriverBase(ITwitterDataStorage twitterDataInMemoryStorage)
        {
            _twitterDataInMemoryStorage = twitterDataInMemoryStorage;
        }

        protected virtual IEnumerable<KeyValuePair<string, int>> GetTopKeysWithCount(ConcurrentDictionary<string, int> dictionary)
        {
            return dictionary.OrderByDescending(dt => dt.Value).Take(10);
        }

        protected double GetAverageTweetsPerTimeSpan(TimeSpan timeSpan)
        {
            return Math.Round(_twitterDataInMemoryStorage.TweetTimeDictionary.GroupBy(tt => tt.Key.AddTicks(-(tt.Key.Ticks % timeSpan.Ticks))).Average(min => min.Sum(m => m.Value)), 2);
        }

        protected double GetPercentageOfTotalTweets(int tweetCategoryCount)
        {
            return Math.Round((tweetCategoryCount * 100d) / _twitterDataInMemoryStorage.TotalTweetCount, 2);
        }
    }
}

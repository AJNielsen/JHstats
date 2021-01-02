using JHStatsWebsite.Data.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace JHStatsWebsite.Data
{
    public class TwitterDataInMemoryStorage : ITwitterDataStorage
    {
        public ConcurrentDictionary<DateTime, int> TweetTimeDictionary { get; set; } = new ConcurrentDictionary<DateTime, int>();
        public ConcurrentDictionary<string, int> EmojiCountDictionary { get; set; } = new ConcurrentDictionary<string, int>();
        public ConcurrentDictionary<string, int> HashtagCountDictionary { get; set; } = new ConcurrentDictionary<string, int>();
        public ConcurrentDictionary<string, int> DomainCountDictionary { get; set; } = new ConcurrentDictionary<string, int>();

        private int _totalTweetCount;
        public int TotalTweetCount
        {
            get
            {
                return _totalTweetCount;
            }
        }

        public void IncrementTotalTweets()
        {
            Interlocked.Increment(ref _totalTweetCount);
        }

        private int _tweetsWithEmojis;
        public int TweetsWithEmojis
        {
            get { return _tweetsWithEmojis; }
        }

        public void IncrementTweetWithEmoji()
        {
            Interlocked.Increment(ref _tweetsWithEmojis);
        }

        private int _tweetsWithAnyUrl;
        public int TweetsWithAnyUrl
        {
            get { return _tweetsWithAnyUrl; }
        }

        public void IncrementTweetWithUrl()
        {
            Interlocked.Increment(ref _tweetsWithAnyUrl);
        }

        private int _tweetsWithImageUrl;
        public int TweetsWithImageUrl
        {
            get { return _tweetsWithImageUrl; }
        }

        public void IncrementTweetsWithImageUrl()
        {
            Interlocked.Increment(ref _tweetsWithImageUrl);
        }
    }
}

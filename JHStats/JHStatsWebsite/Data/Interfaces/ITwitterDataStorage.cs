using System;
using System.Collections.Concurrent;

namespace JHStatsWebsite.Data.Interfaces
{
    public interface ITwitterDataStorage
    {
        ConcurrentDictionary<DateTime, int> TweetTimeDictionary { get; set; }
        ConcurrentDictionary<string, int> EmojiCountDictionary { get; set; }
        ConcurrentDictionary<string, int> HashtagCountDictionary { get; set; }
        ConcurrentDictionary<string, int> DomainCountDictionary { get; set; }
        int TotalTweetCount { get; }
        void IncrementTotalTweets();
        int TweetsWithEmojis { get; }
        void IncrementTweetWithEmoji();
        int TweetsWithAnyUrl { get; }
        void IncrementTweetWithUrl();
        int TweetsWithImageUrl { get; }
        void IncrementTweetsWithImageUrl();
    }
}

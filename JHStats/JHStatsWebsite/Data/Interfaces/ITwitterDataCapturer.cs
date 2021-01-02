using System;

namespace JHStatsWebsite.Data.Interfaces
{
    public interface ITwitterDataCapturer
    {
        void IncrementTweetAtTime(DateTime tweetTime);
        void IncrementEmoji(string emoji);
        void IncrementTweetsWithEmojis();
        void IncrementHashtag(string hashtag);
        void IncrementDomain(string domain);
        void IncrementTweetWithUrl();
        void IncrementTweetWithImageUrl();
    }
}

using JHStatsWebsite.Data.Interfaces;
using System;
using System.Collections.Concurrent;

namespace JHStatsWebsite.Data
{
    public class TwitterDataCapturer : ITwitterDataCapturer
    {
        ITwitterDataStorage _twitterDataInMemoryStorage;

        public TwitterDataCapturer(ITwitterDataStorage twitterDataInMemoryStorage)
        {
            _twitterDataInMemoryStorage = twitterDataInMemoryStorage;
        }

        public void IncrementDomain(string domain)
        {
            if(string.IsNullOrWhiteSpace(domain))
            {
                return;
            }

            IncrementDictionaryKeyValue(_twitterDataInMemoryStorage.DomainCountDictionary, domain.ToLower());
        }

        public void IncrementEmoji(string emoji)
        {
            if(string.IsNullOrWhiteSpace(emoji))
            {
                return;
            }

            IncrementDictionaryKeyValue(_twitterDataInMemoryStorage.EmojiCountDictionary, emoji);
        }

        public void IncrementHashtag(string hashtag)
        {
            if (string.IsNullOrWhiteSpace(hashtag))
            {
                return;
            }

            IncrementDictionaryKeyValue(_twitterDataInMemoryStorage.HashtagCountDictionary, hashtag.ToLower());
        }

        public void IncrementTweetAtTime(DateTime tweetTime)
        {
            IncrementDictionaryKeyValue(_twitterDataInMemoryStorage.TweetTimeDictionary, tweetTime);
            _twitterDataInMemoryStorage.IncrementTotalTweets();
        }

        public void IncrementTweetsWithEmojis()
        {
            _twitterDataInMemoryStorage.IncrementTweetWithEmoji();
        }

        public void IncrementTweetWithImageUrl()
        {
            _twitterDataInMemoryStorage.IncrementTweetsWithImageUrl();
        }

        public void IncrementTweetWithUrl()
        {
            _twitterDataInMemoryStorage.IncrementTweetWithUrl();
        }

        private void IncrementDictionaryKeyValue<T>(ConcurrentDictionary<T, int> dictionary, T key)
        {
            dictionary.AddOrUpdate(key, 1, (key, count) => count + 1);
        }
    }
}

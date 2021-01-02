using JHStatsWebsite.Data;
using Xunit;

namespace JHStatsWebsite.Tests.Data
{
    public class TwitterDataInMemoryStorageTests
    {
        [Fact]
        public void IncrementTotalTweetsIncrementsCountByOne()
        {
            TwitterDataInMemoryStorage twitterDataInMemoryStorage = new TwitterDataInMemoryStorage();
            var beforeIncrementTotal = twitterDataInMemoryStorage.TotalTweetCount;
            twitterDataInMemoryStorage.IncrementTotalTweets();
            Assert.Equal(beforeIncrementTotal + 1, twitterDataInMemoryStorage.TotalTweetCount);
        }

        [Fact]
        public void IncrementTweetWithEmojiIncrementsCountByOne()
        {
            TwitterDataInMemoryStorage twitterDataInMemoryStorage = new TwitterDataInMemoryStorage();
            var beforeIncrementTotal = twitterDataInMemoryStorage.TweetsWithEmojis;
            twitterDataInMemoryStorage.IncrementTweetWithEmoji();
            Assert.Equal(beforeIncrementTotal + 1, twitterDataInMemoryStorage.TweetsWithEmojis);
        }

        [Fact]
        public void IncrementTweetWithUrlIncrementsCountByOne()
        {
            TwitterDataInMemoryStorage twitterDataInMemoryStorage = new TwitterDataInMemoryStorage();
            var beforeIncrementTotal = twitterDataInMemoryStorage.TweetsWithAnyUrl;
            twitterDataInMemoryStorage.IncrementTweetWithUrl();
            Assert.Equal(beforeIncrementTotal + 1, twitterDataInMemoryStorage.TweetsWithAnyUrl);
        }

        [Fact]
        public void IncrementTweetsWithImageUrlIncrementsCountByOne()
        {
            TwitterDataInMemoryStorage twitterDataInMemoryStorage = new TwitterDataInMemoryStorage();
            var beforeIncrementTotal = twitterDataInMemoryStorage.TweetsWithImageUrl;
            twitterDataInMemoryStorage.IncrementTweetsWithImageUrl();
            Assert.Equal(beforeIncrementTotal + 1, twitterDataInMemoryStorage.TweetsWithImageUrl);
        }
    }
}

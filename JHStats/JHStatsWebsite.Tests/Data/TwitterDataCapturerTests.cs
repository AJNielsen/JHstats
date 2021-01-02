using JHStatsWebsite.Data;
using JHStatsWebsite.Data.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Concurrent;
using Xunit;

namespace JHStatsWebsite.Tests.Data
{
    public class TwitterDataCapturerTests
    {
        TwitterDataCapturer _twitterDataCapturer;
        Mock<ITwitterDataStorage> _mockTwitterDataStorage;

        public TwitterDataCapturerTests()
        {
            _mockTwitterDataStorage = new Mock<ITwitterDataStorage>();
            _twitterDataCapturer = new TwitterDataCapturer(_mockTwitterDataStorage.Object);
        }

        [Fact]
        public void EnsureDependenciesAreSetup()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build();

            ServiceCollection services = new ServiceCollection();

            //Due to needing a IHostingService when adding services.AddApplicationInsightsTelemetry() in Startup.
            //We need this line to avoid an exception being thrown when trying to get the IUrlHandler service.
            services.AddSingleton<TelemetryClient>();

            Startup startup = new Startup(configuration);
            startup.ConfigureServices(services);

            ITwitterDataCapturer twitterDataCapturer = services.BuildServiceProvider().GetService<ITwitterDataCapturer>();

            Assert.NotNull(twitterDataCapturer);
            Assert.IsType<TwitterDataCapturer>(twitterDataCapturer);
        }

        #region IncrementDomain Tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("         ")]
        public void IncrementingDomainWithNullEmptyOrWhitespaceWillNotIncrement(string domain)
        {
            ConcurrentDictionary<string, int> dictionary = new ConcurrentDictionary<string, int>();
            _mockTwitterDataStorage.SetupGet(mtds => mtds.DomainCountDictionary).Returns(dictionary);

            _twitterDataCapturer.IncrementDomain(domain);

            _mockTwitterDataStorage.VerifyGet(mtds => mtds.DomainCountDictionary, Times.Never);
        }

        [Fact]
        public void IncrementingDomainWithNewKeyIsConvertedToLowercase()
        {
            string domain = "DomainWithCapitals.com";

            ConcurrentDictionary<string, int> dictionary = new ConcurrentDictionary<string, int>();
            _mockTwitterDataStorage.SetupGet(mtds => mtds.DomainCountDictionary).Returns(dictionary);

            _twitterDataCapturer.IncrementDomain(domain);

            _mockTwitterDataStorage.VerifyGet(mtds => mtds.DomainCountDictionary, Times.AtLeastOnce());
            Assert.True(dictionary.ContainsKey(domain.ToLower()));
            Assert.False(dictionary.ContainsKey(domain));
        }

        [Fact]
        public void IncrementingDomainWithNewKeyAddsDomainAndValue()
        {
            string domain = "arandomdomain.com";

            ConcurrentDictionary<string, int> dictionary = new ConcurrentDictionary<string, int>();
            _mockTwitterDataStorage.SetupGet(mtds => mtds.DomainCountDictionary).Returns(dictionary);

            _twitterDataCapturer.IncrementDomain(domain);

            _mockTwitterDataStorage.VerifyGet(mtds => mtds.DomainCountDictionary, Times.AtLeastOnce());
            Assert.Equal(1, dictionary[domain]);
        }

        [Fact]
        public void IncrementingDomainWithExistingKeyUpdatesDomainAndValue()
        {
            string domain = "arandomdomain.com";

            ConcurrentDictionary<string, int> dictionary = new ConcurrentDictionary<string, int>();
            dictionary.AddOrUpdate(domain, 5, (domain, count) => count + 5);

            _mockTwitterDataStorage.SetupGet(mtds => mtds.DomainCountDictionary).Returns(dictionary);

            _twitterDataCapturer.IncrementDomain(domain);

            _mockTwitterDataStorage.VerifyGet(mtds => mtds.DomainCountDictionary, Times.AtLeastOnce());
            Assert.Equal(6, dictionary[domain]);
        }
        #endregion

        #region IncrementEmoji Tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("         ")]
        public void IncrementingEmojiWithNullEmptyOrWhitespaceWillNotIncrement(string emoji)
        {
            ConcurrentDictionary<string, int> dictionary = new ConcurrentDictionary<string, int>();
            _mockTwitterDataStorage.SetupGet(mtds => mtds.EmojiCountDictionary).Returns(dictionary);

            _twitterDataCapturer.IncrementEmoji(emoji);

            _mockTwitterDataStorage.VerifyGet(mtds => mtds.EmojiCountDictionary, Times.Never);
        }

        [Fact]
        public void IncrementingEmojiWithNewKeyAddsEmojiAndValue()
        {
            string emoji = "👍";

            ConcurrentDictionary<string, int> dictionary = new ConcurrentDictionary<string, int>();
            _mockTwitterDataStorage.SetupGet(mtds => mtds.EmojiCountDictionary).Returns(dictionary);

            _twitterDataCapturer.IncrementEmoji(emoji);

            _mockTwitterDataStorage.VerifyGet(mtds => mtds.EmojiCountDictionary, Times.AtLeastOnce());
            Assert.Equal(1, dictionary[emoji]);
        }

        [Fact]
        public void IncrementingEmojiWithExistingKeyUpdatesEmojiAndValue()
        {
            string emoji = "👍";

            ConcurrentDictionary<string, int> dictionary = new ConcurrentDictionary<string, int>();
            dictionary.AddOrUpdate(emoji, 5, (emoji, count) => count + 5);

            _mockTwitterDataStorage.SetupGet(mtds => mtds.EmojiCountDictionary).Returns(dictionary);

            _twitterDataCapturer.IncrementEmoji(emoji);

            _mockTwitterDataStorage.VerifyGet(mtds => mtds.EmojiCountDictionary, Times.AtLeastOnce());
            Assert.Equal(6, dictionary[emoji]);
        }
        #endregion

        #region IncrementHashtag Tests
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("         ")]
        public void IncrementingHashtagWithNullEmptyOrWhitespaceWillNotIncrement(string hashtag)
        {
            ConcurrentDictionary<string, int> dictionary = new ConcurrentDictionary<string, int>();
            _mockTwitterDataStorage.SetupGet(mtds => mtds.HashtagCountDictionary).Returns(dictionary);

            _twitterDataCapturer.IncrementHashtag(hashtag);

            _mockTwitterDataStorage.VerifyGet(mtds => mtds.HashtagCountDictionary, Times.Never);
        }

        [Fact]
        public void IncrementingHashtagWithNewKeyIsConvertedToLowercase()
        {
            string hashtag = "HashTagWithCapitals";

            ConcurrentDictionary<string, int> dictionary = new ConcurrentDictionary<string, int>();
            _mockTwitterDataStorage.SetupGet(mtds => mtds.HashtagCountDictionary).Returns(dictionary);

            _twitterDataCapturer.IncrementHashtag(hashtag);

            _mockTwitterDataStorage.VerifyGet(mtds => mtds.HashtagCountDictionary, Times.AtLeastOnce());
            Assert.True(dictionary.ContainsKey(hashtag.ToLower()));
            Assert.False(dictionary.ContainsKey(hashtag));
        }

        [Fact]
        public void IncrementingHashtagWithNewKeyAddsHashtagAndValue()
        {
            string hashtag = "randomhashtagvalue";

            ConcurrentDictionary<string, int> dictionary = new ConcurrentDictionary<string, int>();
            _mockTwitterDataStorage.SetupGet(mtds => mtds.HashtagCountDictionary).Returns(dictionary);

            _twitterDataCapturer.IncrementHashtag(hashtag);

            _mockTwitterDataStorage.VerifyGet(mtds => mtds.HashtagCountDictionary, Times.AtLeastOnce());
            Assert.Equal(1, dictionary[hashtag]);
        }

        [Fact]
        public void IncrementingHashtagWithExistingKeyUpdatesHashtagAndValue()
        {
            string hashtag = "randomhashtagvalue";

            ConcurrentDictionary<string, int> dictionary = new ConcurrentDictionary<string, int>();
            dictionary.AddOrUpdate(hashtag, 5, (hashtag, count) => count + 5);

            _mockTwitterDataStorage.SetupGet(mtds => mtds.HashtagCountDictionary).Returns(dictionary);

            _twitterDataCapturer.IncrementHashtag(hashtag);

            _mockTwitterDataStorage.VerifyGet(mtds => mtds.HashtagCountDictionary, Times.AtLeastOnce());
            Assert.Equal(6, dictionary[hashtag]);
        }
        #endregion

        #region IncrementTweetAtTime Tests
        [Fact]
        public void IncrementingTweetAtTimeWithNewKeyAddsDateTimeAndValue()
        {
            DateTime time = DateTime.Today;

            ConcurrentDictionary<DateTime, int> dictionary = new ConcurrentDictionary<DateTime, int>();
            _mockTwitterDataStorage.SetupGet(mtds => mtds.TweetTimeDictionary).Returns(dictionary);

            _twitterDataCapturer.IncrementTweetAtTime(time);

            _mockTwitterDataStorage.VerifyGet(mtds => mtds.TweetTimeDictionary, Times.AtLeastOnce());
            Assert.Equal(1, dictionary[time]);
        }

        [Fact]
        public void IncrementingTweetAtTimeWithExistingKeyUpdatesDateTimeAndValue()
        {
            DateTime time = DateTime.Today;

            ConcurrentDictionary<DateTime, int> dictionary = new ConcurrentDictionary<DateTime, int>();
            dictionary.AddOrUpdate(time, 5, (time, count) => count + 5);

            _mockTwitterDataStorage.SetupGet(mtds => mtds.TweetTimeDictionary).Returns(dictionary);

            _twitterDataCapturer.IncrementTweetAtTime(time);

            _mockTwitterDataStorage.VerifyGet(mtds => mtds.TweetTimeDictionary, Times.AtLeastOnce());
            Assert.Equal(6, dictionary[time]);
        }

        [Fact]
        public void IncrementingTweetAtTimeIncrementsTotalTweets()
        {
            DateTime time = DateTime.Today;

            ConcurrentDictionary<DateTime, int> dictionary = new ConcurrentDictionary<DateTime, int>();
            dictionary.AddOrUpdate(time, 5, (time, count) => count + 5);

            _mockTwitterDataStorage.SetupGet(mtds => mtds.TweetTimeDictionary).Returns(dictionary);

            _twitterDataCapturer.IncrementTweetAtTime(time);

            _mockTwitterDataStorage.Verify(mtds => mtds.IncrementTotalTweets(), Times.Once);
        }
        #endregion

        #region IncrementTweetsWithEmojis Tests
        [Fact]
        public void IncrementTweetsWithEmojisCallsIncrementTweetWithEmoji()
        {
            _twitterDataCapturer.IncrementTweetsWithEmojis();

            _mockTwitterDataStorage.Verify(mtds => mtds.IncrementTweetWithEmoji(), Times.Once);
        }
        #endregion

        #region IncrementTweetWithImageUrl Tests
        [Fact]
        public void IncrementTweetWithImageUrlCallsIncrementTweetsWithImageUrl()
        {
            _twitterDataCapturer.IncrementTweetWithImageUrl();

            _mockTwitterDataStorage.Verify(mtds => mtds.IncrementTweetsWithImageUrl(), Times.Once);
        }
        #endregion

        #region IncrementTweetWithUrl Tests
        [Fact]
        public void IncrementTweetWithUrlCallsIncrementTweetWithUrl()
        {
            _twitterDataCapturer.IncrementTweetWithUrl();

            _mockTwitterDataStorage.Verify(mtds => mtds.IncrementTweetWithUrl(), Times.Once);
        }
        #endregion
    }
}

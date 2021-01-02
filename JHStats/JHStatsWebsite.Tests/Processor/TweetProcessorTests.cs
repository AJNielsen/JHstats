using JHStatsWebsite.Data.Interfaces;
using JHStatsWebsite.Models.Twitter;
using JHStatsWebsite.Processor;
using JHStatsWebsite.Utility.Interfaces;
using Microsoft.ApplicationInsights;
using Moq;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xunit;

namespace JHStatsWebsite.Tests.Processor
{
    public class TweetProcessorTests
    {
        private Mock<ITwitterDataCapturer> _mockTwitterDataCapturer;
        private Mock<IUrlHandler> _mockUrlHandler;
        private TweetProcessor _tweetProcessor;

        public TweetProcessorTests()
        {
            _mockTwitterDataCapturer = new Mock<ITwitterDataCapturer>();
            _mockUrlHandler = new Mock<IUrlHandler>();
            _tweetProcessor = new TweetProcessor(_mockTwitterDataCapturer.Object, _mockUrlHandler.Object, new TelemetryClient());

            _mockTwitterDataCapturer.Setup(mtdc => mtdc.IncrementTweetAtTime(It.IsAny<DateTime>()));
            _mockTwitterDataCapturer.Setup(mtdc => mtdc.IncrementEmoji(It.IsAny<string>()));
            _mockTwitterDataCapturer.Setup(mtdc => mtdc.IncrementHashtag(It.IsAny<string>()));
            _mockTwitterDataCapturer.Setup(mtdc => mtdc.IncrementDomain(It.IsAny<string>()));            
        }

        [Fact]
        public void ProcessEndsGracefullyWhenTweetTaskIsFaulted()
        {
            Task<string> faultedTask = Task.FromException<string>(new Exception("Generate a faulted Task."));
            Task processTask = _tweetProcessor.Process(faultedTask);
            Assert.True(processTask.Wait(10000));
            _mockTwitterDataCapturer.Verify(mtdc => mtdc.IncrementTweetAtTime(It.IsAny<DateTime>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("         ")]
        public void ProcessEndsGracefullyWhenTweetisNullEmptyOrWhitespace(string tweetData)
        {
            Func<Task<string>> taskWithTweetData = () => Task.FromResult(tweetData);
            Task<string> tweetDataTask = Task.Run(taskWithTweetData);
            Task processTask = _tweetProcessor.Process(tweetDataTask);
            Assert.True(processTask.Wait(1000));
            _mockTwitterDataCapturer.Verify(mtdc => mtdc.IncrementTweetAtTime(It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public void ProcessIncrementsTweetAtTweetTime()
        {
            TweetData tweetData = new TweetData
            {
                Data = new Tweet
                {
                    CreatedAt = DateTime.Today,
                    Text = "Non Empty Text Data"
                }
            };

            string tweetDataJson = GetTweetJsonString(tweetData);

            Func<Task<string>> taskWithTweetData = () => Task.FromResult(tweetDataJson);
            Task<string> tweetDataTask = Task.Run(taskWithTweetData);
            Task processTask = _tweetProcessor.Process(tweetDataTask);
            Assert.True(processTask.Wait(1000));
            _mockTwitterDataCapturer.Verify(mtdc => mtdc.IncrementTweetAtTime(tweetData.Data.CreatedAt), Times.Once);
        }

        [Fact]
        public void ProcessFindsEmojiInStringAndIncrementsIt()
        {
            string emoji = "🙏";

            TweetData tweetData = new TweetData
            {
                Data = new Tweet
                {
                    CreatedAt = DateTime.Today,
                    Text = emoji
                }
            };

            string tweetDataJson = GetTweetJsonString(tweetData);

            Func<Task<string>> taskWithTweetData = () => Task.FromResult(tweetDataJson);
            Task<string> tweetDataTask = Task.Run(taskWithTweetData);
            Task processTask = _tweetProcessor.Process(tweetDataTask);
            Assert.True(processTask.Wait(1000));
            _mockTwitterDataCapturer.Verify(mtdc => mtdc.IncrementEmoji(emoji), Times.Once);
        }

        [Fact]
        public void ProcessFindsHashTagAndIncrementsTag()
        {
            string hashtag = "";
            var hashTags = new Hashtag[1];
            hashTags[0] = new Hashtag
            {
                Tag = hashtag
            };

            TweetData tweetData = new TweetData
            {
                Data = new Tweet
                {
                    CreatedAt = DateTime.Today,
                    Text = "Non Empty Text Data",
                    Entities = new TweetEntities
                    {
                        Hashtags = hashTags
                    }
                }
            };

            string tweetDataJson = GetTweetJsonString(tweetData);

            Func<Task<string>> taskWithTweetData = () => Task.FromResult(tweetDataJson);
            Task<string> tweetDataTask = Task.Run(taskWithTweetData);
            Task processTask = _tweetProcessor.Process(tweetDataTask);
            Assert.True(processTask.Wait(1000));
            _mockTwitterDataCapturer.Verify(mtdc => mtdc.IncrementHashtag(hashtag), Times.Once);
        }

        [Fact]
        public void ProcessFindsUrlAndIncrementsTweetWithUrl()
        {
            string expectedUrl = "http://fakewebsite.com";

            var urls = new Url[1];
            urls[0] = new Url
            {
                ExpandedUrl = expectedUrl
            };

            TweetData tweetData = new TweetData
            {
                Data = new Tweet
                {
                    CreatedAt = DateTime.Today,
                    Text = "Non Empty Text Data",
                    Entities = new TweetEntities
                    {
                        Urls = urls
                    }
                }
            };

            string tweetDataJson = GetTweetJsonString(tweetData);

            Func<Task<string>> taskWithTweetData = () => Task.FromResult(tweetDataJson);
            Task<string> tweetDataTask = Task.Run(taskWithTweetData);
            Task processTask = _tweetProcessor.Process(tweetDataTask);
            Assert.True(processTask.Wait(1000));
            _mockUrlHandler.Verify(muh => muh.GetDomainFromUrl(expectedUrl), Times.Once);
            _mockTwitterDataCapturer.Verify(mtdc => mtdc.IncrementTweetWithUrl(), Times.Once);
        }

        [Fact]
        public void ProcessFindsUrlAndIncrementsDomainCount()
        {
            string expectedDomain = "fakewebsite.com";
            string expectedUrl = $"http://{expectedDomain}";

            _mockUrlHandler.Setup(muh => muh.GetDomainFromUrl(expectedUrl)).Returns(expectedDomain);

            var urls = new Url[1];
            urls[0] = new Url
            {
                ExpandedUrl = expectedUrl
            };

            TweetData tweetData = new TweetData
            {
                Data = new Tweet
                {
                    CreatedAt = DateTime.Today,
                    Text = "Non Empty Text Data",
                    Entities = new TweetEntities
                    {
                        Urls = urls
                    }
                }
            };

            string tweetDataJson = GetTweetJsonString(tweetData);

            Func<Task<string>> taskWithTweetData = () => Task.FromResult(tweetDataJson);
            Task<string> tweetDataTask = Task.Run(taskWithTweetData);
            Task processTask = _tweetProcessor.Process(tweetDataTask);
            Assert.True(processTask.Wait(1000));
            _mockUrlHandler.Verify(muh => muh.GetDomainFromUrl(expectedUrl), Times.Once);
            _mockTwitterDataCapturer.Verify(mtdc => mtdc.IncrementDomain(expectedDomain), Times.Once);
        }

        [Fact]
        public void ProcessFindsImageUrlAndIncrementsTweetWithImages()
        {
            string expectedDomain = "fakewebsite.com";
            string expectedUrl = $"http://{expectedDomain}";

            _mockUrlHandler.Setup(muh => muh.GetDomainFromUrl(expectedUrl)).Returns(expectedDomain);
            _mockUrlHandler.Setup(muh => muh.DomainIsImageDomain(expectedDomain)).Returns(true);

            var urls = new Url[1];
            urls[0] = new Url
            {
                ExpandedUrl = expectedUrl
            };

            TweetData tweetData = new TweetData
            {
                Data = new Tweet
                {
                    CreatedAt = DateTime.Today,
                    Text = "Non Empty Text Data",
                    Entities = new TweetEntities
                    {
                        Urls = urls
                    }
                }
            };

            string tweetDataJson = GetTweetJsonString(tweetData);

            Func<Task<string>> taskWithTweetData = () => Task.FromResult(tweetDataJson);
            Task<string> tweetDataTask = Task.Run(taskWithTweetData);
            Task processTask = _tweetProcessor.Process(tweetDataTask);
            Assert.True(processTask.Wait(1000));
            _mockUrlHandler.Verify(muh => muh.GetDomainFromUrl(expectedUrl), Times.Once);
            _mockUrlHandler.Verify(muh => muh.DomainIsImageDomain(expectedDomain), Times.Once);
            _mockTwitterDataCapturer.Verify(mtdc => mtdc.IncrementTweetWithImageUrl(), Times.Once);
        }

        [Fact]
        public void ProcessFindsMultipleImageUrlsButOnlyIncrementsTweetsWithImagesOnce()
        {
            string expectedDomain = "fakewebsite.com";
            string expectedUrl = $"http://{expectedDomain}";

            _mockUrlHandler.Setup(muh => muh.GetDomainFromUrl(expectedUrl)).Returns(expectedDomain);
            _mockUrlHandler.Setup(muh => muh.DomainIsImageDomain(expectedDomain)).Returns(true);

            var urls = new Url[2];
            urls[0] = new Url
            {
                ExpandedUrl = expectedUrl
            };
            urls[1] = new Url
            {
                ExpandedUrl = expectedUrl
            };

            TweetData tweetData = new TweetData
            {
                Data = new Tweet
                {
                    CreatedAt = DateTime.Today,
                    Text = "Non Empty Text Data",
                    Entities = new TweetEntities
                    {
                        Urls = urls
                    }
                }
            };

            string tweetDataJson = GetTweetJsonString(tweetData);

            Func<Task<string>> taskWithTweetData = () => Task.FromResult(tweetDataJson);
            Task<string> tweetDataTask = Task.Run(taskWithTweetData);
            Task processTask = _tweetProcessor.Process(tweetDataTask);
            Assert.True(processTask.Wait(1000));
            _mockUrlHandler.Verify(muh => muh.GetDomainFromUrl(expectedUrl), Times.Exactly(2));
            _mockUrlHandler.Verify(muh => muh.DomainIsImageDomain(expectedDomain), Times.Once);
            _mockTwitterDataCapturer.Verify(mtdc => mtdc.IncrementTweetWithImageUrl(), Times.Once);
        }

        private string GetTweetJsonString(TweetData tweetData)
        {
            return JsonConvert.SerializeObject(tweetData);
        }
    }
}

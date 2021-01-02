using JHStatsWebsite.Models.Twitter;
using JHStatsWebsite.Processor;
using JHStatsWebsite.Processor.Interfaces;
using JHStatsWebsite.Tests.MessageHandlers;
using Microsoft.ApplicationInsights;
using Moq;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace JHStatsWebsite.Tests.Processor
{
    public class TwitterStreamProcessorTests
    {
        private TwitterStreamProcessor _twitterStreamProcessor;
        private Mock<ITweetProcessor> _mockTweetProcessor;
        private CancellationTokenSource _cancellationTokenSource;
        private HttpClient _httpClient;
        private TestMessageHandler _testMessageHandler;

        public TwitterStreamProcessorTests()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _mockTweetProcessor = new Mock<ITweetProcessor>();
            _testMessageHandler = new TestMessageHandler();
            _httpClient = new HttpClient(_testMessageHandler);
            _httpClient.BaseAddress = new Uri( "http://localhost/");
            _twitterStreamProcessor = new TwitterStreamProcessor(_mockTweetProcessor.Object, _httpClient, new TelemetryClient());
        }

        [Fact]
        public void ProcessTwitterStreamEndsWhenCancellationTokenIsCancelled()
        {
            _cancellationTokenSource.Cancel();

            Task processTask = _twitterStreamProcessor.ProcessTwitterStream(_cancellationTokenSource.Token);

            Assert.True(processTask.Wait(1000));
        }

        [Fact]
        public void ProcessTwitterStreamEndsWhenEndOfStreamIsReached()
        {
            Task processTask = _twitterStreamProcessor.ProcessTwitterStream(_cancellationTokenSource.Token);

            Assert.True(processTask.Wait(1000));
        }

        [Fact]
        public void ProcessTwitterStreamProcessesTweet()
        {
            Task<string> capturedTaskString = null;
            

            _mockTweetProcessor.Setup(mtp => mtp.Process(It.IsAny<Task<string>>())).Callback((Task<string> taskString) => capturedTaskString = taskString);

            TweetData tweetData = new TweetData
            {
                Data = new Tweet
                {
                    CreatedAt = DateTime.Today,
                    Id = "1234",
                    Text = "Tweet Text"
                }
            };

            string tweetJson = JsonConvert.SerializeObject(tweetData);
            _testMessageHandler.AddMessagesToStream(Enumerable.Repeat(tweetJson, 50000));


            Task processTask = _twitterStreamProcessor.ProcessTwitterStream(_cancellationTokenSource.Token);

            Assert.True(processTask.Wait(1000));

            _mockTweetProcessor.Verify(mtp => mtp.Process(It.IsAny<Task<string>>()), Times.AtLeastOnce);
            string capturedJson = capturedTaskString.Result;
            Assert.Equal(tweetJson, capturedJson);
        }

        [Fact]
        public void CancelTwitterStreamProcessingEndsProcessingOfTweets()
        {
            TweetData tweetData = new TweetData
            {
                Data = new Tweet
                {
                    CreatedAt = DateTime.Today,
                    Id = "1234",
                    Text = "Tweet Text"
                }
            };

            string tweetJson = JsonConvert.SerializeObject(tweetData);

            _testMessageHandler.AddMessagesToStream(Enumerable.Repeat(tweetJson, 50000));

            Task processTask = _twitterStreamProcessor.ProcessTwitterStream(_cancellationTokenSource.Token);
            Task cancelTask = _twitterStreamProcessor.CancelTwitterStreamProcessing();

            Assert.True(processTask.Wait(1000));
            Assert.True(cancelTask.Wait(1000));

            _mockTweetProcessor.Verify(mtp => mtp.Process(It.IsAny<Task<string>>()), Times.AtLeastOnce());
        }
    }
}

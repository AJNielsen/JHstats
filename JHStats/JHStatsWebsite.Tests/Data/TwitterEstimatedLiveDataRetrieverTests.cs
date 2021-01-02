using JHStatsWebsite.Data;
using JHStatsWebsite.Data.Interfaces;
using JHStatsWebsite.Models;
using Moq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace JHStatsWebsite.Tests.Data
{
    public class TwitterEstimatedLiveDataRetrieverTests
    {
        private TwitterEstimatedLiveDataRetriever _twitterEstimatedLiveDataRetriever;
        private Mock<ITwitterDataStorage> _mockTwitterDataStorage;
        ConcurrentDictionary<DateTime, int> _tweetTimeDictionary;
        ConcurrentDictionary<string, int> _domainCountDictionary;
        ConcurrentDictionary<string, int> _emojiCountDictionary;

        public TwitterEstimatedLiveDataRetrieverTests()
        {
            _mockTwitterDataStorage = new Mock<ITwitterDataStorage>();
            _twitterEstimatedLiveDataRetriever = new TwitterEstimatedLiveDataRetriever(_mockTwitterDataStorage.Object);

            _tweetTimeDictionary = new ConcurrentDictionary<DateTime, int>();
            _mockTwitterDataStorage.SetupGet(mtds => mtds.TweetTimeDictionary).Returns(_tweetTimeDictionary);

            _domainCountDictionary = new ConcurrentDictionary<string, int>();
            _mockTwitterDataStorage.SetupGet(mtds => mtds.DomainCountDictionary).Returns(_domainCountDictionary);

            _emojiCountDictionary = new ConcurrentDictionary<string, int>();
            _mockTwitterDataStorage.SetupGet(mtds => mtds.EmojiCountDictionary).Returns(_emojiCountDictionary);

            _tweetTimeDictionary.AddOrUpdate(DateTime.Today, 1, (time, count) => count + 1);
        }

        [Fact]
        public void GetStatsReturnsAnEmptyTwitterStatsDetailsWhenThereIsNoTweetData()
        {
            _tweetTimeDictionary.Clear();

            _mockTwitterDataStorage.SetupGet(mtds => mtds.TweetTimeDictionary).Returns(_tweetTimeDictionary);

            TwitterStatsDetails twitterStatsDetails = _twitterEstimatedLiveDataRetriever.GetStats();

            Assert.NotNull(twitterStatsDetails);
            Assert.Null(twitterStatsDetails.TopDomains);
            Assert.Null(twitterStatsDetails.TopEmojis);
            Assert.Null(twitterStatsDetails.Averages);
            Assert.Equal(0, twitterStatsDetails.TotalTweets);
            Assert.Equal(0, twitterStatsDetails.PercentageOfTweetsWithEmojis);
            Assert.Equal(0, twitterStatsDetails.PercentageOfTweetsContainsUrl);
            Assert.Equal(0, twitterStatsDetails.PercentageOfTweetsContainsImage);
        }

        [Fact]
        public void GetStatsReturnsTop10Domains()
        {
            string domainOne = "domainOne";
            string domainTwo = "domainTwo";
            string domainThree = "domainThree";
            string domainFour = "domainFour";
            string domainFive = "domainFive";
            string domainSix = "domainSix";
            string domainSeven = "domainSeven";
            string domainEight = "domainEight";
            string domainNine = "domainNine";
            string domainTen = "domainTen";
            string domainEleven = "domainEleven";
            string domainTweleve = "domainTweleve";

            int domainOneValue = 100;
            int domainTwoValue = 95;
            int domainThreeValue = 90;
            int domainFourValue = 85;
            int domainFiveValue = 80;
            int domainSixValue = 75;
            int domainSevenValue = 70;
            int domainEightValue = 65;
            int domainNineValue = 60;
            int domainTenValue = 55;
            int domainElevenValue = 50;
            int domainTweleveValue = 45;

            AddToDictionary(_domainCountDictionary, domainTweleve, domainTweleveValue);
            AddToDictionary(_domainCountDictionary, domainEleven, domainElevenValue);
            AddToDictionary(_domainCountDictionary, domainTen, domainTenValue);
            AddToDictionary(_domainCountDictionary, domainNine, domainNineValue);
            AddToDictionary(_domainCountDictionary, domainEight, domainEightValue);
            AddToDictionary(_domainCountDictionary, domainSeven, domainSevenValue);
            AddToDictionary(_domainCountDictionary, domainSix, domainSixValue);
            AddToDictionary(_domainCountDictionary, domainFive, domainFiveValue);
            AddToDictionary(_domainCountDictionary, domainFour, domainFourValue);
            AddToDictionary(_domainCountDictionary, domainThree, domainThreeValue);
            AddToDictionary(_domainCountDictionary, domainTwo, domainTwoValue);
            AddToDictionary(_domainCountDictionary, domainOne, domainOneValue);

            TwitterStatsDetails twitterStatsDetails = _twitterEstimatedLiveDataRetriever.GetStats();

            Assert.NotNull(twitterStatsDetails);
            Assert.NotNull(twitterStatsDetails.TopDomains);
            Assert.Equal(10, twitterStatsDetails.TopDomains.Count());
            KeyValuePair<string, int>[] keyValues = twitterStatsDetails.TopDomains.ToArray();

            Assert.Equal(domainOne, keyValues[0].Key);
            Assert.Equal(domainOneValue * 100, keyValues[0].Value);

            Assert.Equal(domainTwo, keyValues[1].Key);
            Assert.Equal(domainTwoValue * 100, keyValues[1].Value);

            Assert.Equal(domainThree, keyValues[2].Key);
            Assert.Equal(domainThreeValue * 100, keyValues[2].Value);

            Assert.Equal(domainFour, keyValues[3].Key);
            Assert.Equal(domainFourValue * 100, keyValues[3].Value);

            Assert.Equal(domainFive, keyValues[4].Key);
            Assert.Equal(domainFiveValue * 100, keyValues[4].Value);

            Assert.Equal(domainSix, keyValues[5].Key);
            Assert.Equal(domainSixValue * 100, keyValues[5].Value);

            Assert.Equal(domainSeven, keyValues[6].Key);
            Assert.Equal(domainSevenValue * 100, keyValues[6].Value);

            Assert.Equal(domainEight, keyValues[7].Key);
            Assert.Equal(domainEightValue * 100, keyValues[7].Value);

            Assert.Equal(domainNine, keyValues[8].Key);
            Assert.Equal(domainNineValue * 100, keyValues[8].Value);

            Assert.Equal(domainTen, keyValues[9].Key);
            Assert.Equal(domainTenValue * 100, keyValues[9].Value);
        }

        [Fact]
        public void GetStatsReturnsTop10Emojis()
        {
            string emojiOne = "🐮";
            string emojiTwo = "💡";
            string emojiThree = "👍🏻";
            string emojiFour = "😳";
            string emojiFive = "🙏";
            string emojiSix = "😫";
            string emojiSeven = "💔";
            string emojiEight = "❤";
            string emojiNine = "☝🏼";
            string emojiTen = "☺️";
            string emojiEleven = "😆";
            string emojiTweleve = "😭";

            int emojiOneValue = 100;
            int emojiTwoValue = 95;
            int emojiThreeValue = 90;
            int emojiFourValue = 85;
            int emojiFiveValue = 80;
            int emojiSixValue = 75;
            int emojiSevenValue = 70;
            int emojiEightValue = 65;
            int emojiNineValue = 60;
            int emojiTenValue = 55;
            int emojiElevenValue = 50;
            int emojiTweleveValue = 45;

            AddToDictionary(_emojiCountDictionary, emojiTweleve, emojiTweleveValue);
            AddToDictionary(_emojiCountDictionary, emojiEleven, emojiElevenValue);
            AddToDictionary(_emojiCountDictionary, emojiTen, emojiTenValue);
            AddToDictionary(_emojiCountDictionary, emojiNine, emojiNineValue);
            AddToDictionary(_emojiCountDictionary, emojiEight, emojiEightValue);
            AddToDictionary(_emojiCountDictionary, emojiSeven, emojiSevenValue);
            AddToDictionary(_emojiCountDictionary, emojiSix, emojiSixValue);
            AddToDictionary(_emojiCountDictionary, emojiFive, emojiFiveValue);
            AddToDictionary(_emojiCountDictionary, emojiFour, emojiFourValue);
            AddToDictionary(_emojiCountDictionary, emojiThree, emojiThreeValue);
            AddToDictionary(_emojiCountDictionary, emojiTwo, emojiTwoValue);
            AddToDictionary(_emojiCountDictionary, emojiOne, emojiOneValue);

            TwitterStatsDetails twitterStatsDetails = _twitterEstimatedLiveDataRetriever.GetStats();

            Assert.NotNull(twitterStatsDetails);
            Assert.NotNull(twitterStatsDetails.TopEmojis);
            Assert.Equal(10, twitterStatsDetails.TopEmojis.Count());
            KeyValuePair<string, int>[] keyValues = twitterStatsDetails.TopEmojis.ToArray();

            Assert.Equal(emojiOne, keyValues[0].Key);
            Assert.Equal(emojiOneValue * 100, keyValues[0].Value);

            Assert.Equal(emojiTwo, keyValues[1].Key);
            Assert.Equal(emojiTwoValue * 100, keyValues[1].Value);

            Assert.Equal(emojiThree, keyValues[2].Key);
            Assert.Equal(emojiThreeValue * 100, keyValues[2].Value);

            Assert.Equal(emojiFour, keyValues[3].Key);
            Assert.Equal(emojiFourValue * 100, keyValues[3].Value);

            Assert.Equal(emojiFive, keyValues[4].Key);
            Assert.Equal(emojiFiveValue * 100, keyValues[4].Value);

            Assert.Equal(emojiSix, keyValues[5].Key);
            Assert.Equal(emojiSixValue * 100, keyValues[5].Value);

            Assert.Equal(emojiSeven, keyValues[6].Key);
            Assert.Equal(emojiSevenValue * 100, keyValues[6].Value);

            Assert.Equal(emojiEight, keyValues[7].Key);
            Assert.Equal(emojiEightValue * 100, keyValues[7].Value);

            Assert.Equal(emojiNine, keyValues[8].Key);
            Assert.Equal(emojiNineValue * 100, keyValues[8].Value);

            Assert.Equal(emojiTen, keyValues[9].Key);
            Assert.Equal(emojiTenValue * 100, keyValues[9].Value);
        }

        [Fact]
        public void GetStatsReturnsTotalTweets()
        {
            _mockTwitterDataStorage.SetupGet(mtds => mtds.TotalTweetCount).Returns(200);

            TwitterStatsDetails twitterStatsDetails = _twitterEstimatedLiveDataRetriever.GetStats();

            Assert.NotNull(twitterStatsDetails);
            Assert.Equal(20000, twitterStatsDetails.TotalTweets);
        }

        [Fact]
        public void GetStatsReturnsPercentageOfTweetsWithImage()
        {
            int tweetsWithImage = 37;
            int totalTweets = 123;

            _mockTwitterDataStorage.SetupGet(mtds => mtds.TweetsWithImageUrl).Returns(tweetsWithImage);
            _mockTwitterDataStorage.SetupGet(mtds => mtds.TotalTweetCount).Returns(totalTweets);

            TwitterStatsDetails twitterStatsDetails = _twitterEstimatedLiveDataRetriever.GetStats();

            Assert.NotNull(twitterStatsDetails);
            Assert.Equal(Math.Round((tweetsWithImage * 100d) / totalTweets, 2), twitterStatsDetails.PercentageOfTweetsContainsImage);
        }

        [Fact]
        public void GetStatsReturnsPercentageOfTweetsWithUrl()
        {
            int tweetsWithUrl = 43;
            int totalTweets = 135;

            _mockTwitterDataStorage.SetupGet(mtds => mtds.TweetsWithAnyUrl).Returns(tweetsWithUrl);
            _mockTwitterDataStorage.SetupGet(mtds => mtds.TotalTweetCount).Returns(totalTweets);

            TwitterStatsDetails twitterStatsDetails = _twitterEstimatedLiveDataRetriever.GetStats();

            Assert.NotNull(twitterStatsDetails);
            Assert.Equal(Math.Round((tweetsWithUrl * 100d) / totalTweets, 2), twitterStatsDetails.PercentageOfTweetsContainsUrl);
        }

        [Fact]
        public void GetStatsReturnsPercentageOfTweetsWithEmojis()
        {
            int tweetsWithEmoji = 23;
            int totalTweets = 157;

            _mockTwitterDataStorage.SetupGet(mtds => mtds.TweetsWithEmojis).Returns(tweetsWithEmoji);
            _mockTwitterDataStorage.SetupGet(mtds => mtds.TotalTweetCount).Returns(totalTweets);

            TwitterStatsDetails twitterStatsDetails = _twitterEstimatedLiveDataRetriever.GetStats();

            Assert.NotNull(twitterStatsDetails);
            Assert.Equal(Math.Round((tweetsWithEmoji * 100d) / totalTweets, 2), twitterStatsDetails.PercentageOfTweetsWithEmojis);
        }

        [Fact]
        public void GetStatsReturnsAveragesOfTweetTimesForSeconds()
        {
            _tweetTimeDictionary.Clear();

            int secondOne = 37;
            int secondTwo = 43;
            int secondThree = 23;
            int secondFour = 51;
            int secondFive = 49;

            AddToDictionary(_tweetTimeDictionary, DateTime.Today, secondOne);
            AddToDictionary(_tweetTimeDictionary, DateTime.Today.AddSeconds(1), secondTwo);
            AddToDictionary(_tweetTimeDictionary, DateTime.Today.AddSeconds(2), secondThree);
            AddToDictionary(_tweetTimeDictionary, DateTime.Today.AddSeconds(3), secondFour);
            AddToDictionary(_tweetTimeDictionary, DateTime.Today.AddSeconds(4), secondFive);

            TwitterStatsDetails twitterStatsDetails = _twitterEstimatedLiveDataRetriever.GetStats();

            Assert.NotNull(twitterStatsDetails);
            Assert.NotNull(twitterStatsDetails.Averages);

            double expectedAverage = Math.Round((secondOne + secondTwo + secondThree + secondFour + secondFive * 1d) / 5 * 100, 2);

            Assert.Equal(expectedAverage, twitterStatsDetails.Averages.TweetsPerSecond);
        }

        [Fact]
        public void GetStatsReturnsAveragesOfTweetTimesForMinutes()
        {
            _tweetTimeDictionary.Clear();

            int minuteOne = 22;
            int minuteTwo = 67;
            int minuteThree = 71;
            int minuteFour = 33;
            int minuteFive = 46;

            AddToDictionary(_tweetTimeDictionary, DateTime.Today, minuteOne);
            AddToDictionary(_tweetTimeDictionary, DateTime.Today.AddMinutes(1), minuteTwo);
            AddToDictionary(_tweetTimeDictionary, DateTime.Today.AddMinutes(2), minuteThree);
            AddToDictionary(_tweetTimeDictionary, DateTime.Today.AddMinutes(3), minuteFour);
            AddToDictionary(_tweetTimeDictionary, DateTime.Today.AddMinutes(4), minuteFive);

            TwitterStatsDetails twitterStatsDetails = _twitterEstimatedLiveDataRetriever.GetStats();

            Assert.NotNull(twitterStatsDetails);
            Assert.NotNull(twitterStatsDetails.Averages);

            double expectedAverage = Math.Round((minuteOne + minuteTwo + minuteThree + minuteFour + minuteFive * 1d) / 5 * 100, 2);

            Assert.Equal(expectedAverage, twitterStatsDetails.Averages.TweetsPerMinute);
        }

        [Fact]
        public void GetStatsReturnsAveragesOfTweetTimesForHours()
        {
            _tweetTimeDictionary.Clear();

            int hourOne = 1035;
            int hourTwo = 674;
            int hourThree = 713;
            int hourFour = 339;
            int hourFive = 468;

            AddToDictionary(_tweetTimeDictionary, DateTime.Today, hourOne);
            AddToDictionary(_tweetTimeDictionary, DateTime.Today.AddHours(1), hourTwo);
            AddToDictionary(_tweetTimeDictionary, DateTime.Today.AddHours(2), hourThree);
            AddToDictionary(_tweetTimeDictionary, DateTime.Today.AddHours(3), hourFour);
            AddToDictionary(_tweetTimeDictionary, DateTime.Today.AddHours(4), hourFive);

            TwitterStatsDetails twitterStatsDetails = _twitterEstimatedLiveDataRetriever.GetStats();

            Assert.NotNull(twitterStatsDetails);
            Assert.NotNull(twitterStatsDetails.Averages);

            double expectedAverage = Math.Round((hourOne + hourTwo + hourThree + hourFour + hourFive * 1d) / 5 * 100, 2);

            Assert.Equal(expectedAverage, twitterStatsDetails.Averages.TweetsPerHour);
        }

        private void AddToDictionary<T>(ConcurrentDictionary<T, int> dictionary, T key, int value)
        {
            dictionary.AddOrUpdate(key, value, (item, count) => count + value);
        }
    }
}

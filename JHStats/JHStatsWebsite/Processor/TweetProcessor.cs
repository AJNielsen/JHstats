using JHStatsWebsite.Data.Interfaces;
using JHStatsWebsite.Models.Twitter;
using JHStatsWebsite.Processor.Interfaces;
using JHStatsWebsite.Utility;
using JHStatsWebsite.Utility.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JHStatsWebsite.Processor
{
    public class TweetProcessor : ITweetProcessor
    {
        private Regex emojiRegex = new Regex(Constants.TWITTER_REGEX_STRING, RegexOptions.Compiled);
        private ITwitterDataCapturer _twitterDataCapturer;
        private readonly IUrlHandler _urlHandler;
        private readonly TelemetryClient _telementryClient;

        public TweetProcessor(ITwitterDataCapturer twitterDataCapturer, IUrlHandler urlHandler, TelemetryClient telementryClient)
        {
            _twitterDataCapturer = twitterDataCapturer;
            _urlHandler = urlHandler;
            _telementryClient = telementryClient;
        }

        public async Task Process(Task<string> singleTweetTask)
        {
            if (singleTweetTask.IsFaulted)
            {
                _telementryClient.TrackTrace($"{nameof(TweetProcessor)} failed to process tweet due to the tweet task being in a faulted state.", SeverityLevel.Error);
                return;
            }

            string tweetData = await singleTweetTask;

            if (string.IsNullOrWhiteSpace(tweetData))
            {
                _telementryClient.TrackTrace($"{nameof(TweetProcessor)} failed to process tweet due to the tweet data being null or whitespace.", SeverityLevel.Error);
                return;
            }

            TweetData deserializedTweet = JsonConvert.DeserializeObject<TweetData>(tweetData);

            _twitterDataCapturer.IncrementTweetAtTime(deserializedTweet.Data.CreatedAt);

            MatchCollection emojiMatches = emojiRegex.Matches(deserializedTweet.Data.Text);

            if (emojiMatches.Count > 0)
            {
                _twitterDataCapturer.IncrementTweetsWithEmojis();

                foreach (Match emojiMatch in emojiMatches)
                {
                    _twitterDataCapturer.IncrementEmoji(emojiMatch.Value);
                }
            }

            if (deserializedTweet.Data.Entities == null)
            {
                return;
            }

            if (deserializedTweet.Data.Entities.Hashtags != null)
            {
                foreach (Hashtag hashtag in deserializedTweet.Data.Entities.Hashtags)
                {
                    _twitterDataCapturer.IncrementHashtag(hashtag.Tag);
                }
            }

            if (deserializedTweet.Data.Entities.Urls != null && deserializedTweet.Data.Entities.Urls.Length > 0)
            {
                bool imageDomainFound = false;
                _twitterDataCapturer.IncrementTweetWithUrl();
                foreach (Url url in deserializedTweet.Data.Entities.Urls)
                {
                    string domainUrl = _urlHandler.GetDomainFromUrl(url.ExpandedUrl);

                    _twitterDataCapturer.IncrementDomain(domainUrl);
                    if (!imageDomainFound && _urlHandler.DomainIsImageDomain(domainUrl))
                    {
                        imageDomainFound = true;
                        _twitterDataCapturer.IncrementTweetWithImageUrl();
                    }
                }
            }
        }
    }
}

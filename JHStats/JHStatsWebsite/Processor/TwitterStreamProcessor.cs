using JHStatsWebsite.Processor.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace JHStatsWebsite.Processor
{
    public class TwitterStreamProcessor : ITwitterStreamProcessor
    {
        private ITweetProcessor _tweetProcessor;
        private readonly HttpClient _httpClient;
        private readonly TelemetryClient _telementryClient;
        private bool cancelRequested;

        public TwitterStreamProcessor(ITweetProcessor tweetProcessor, HttpClient httpClient, TelemetryClient telementryClient)
        {
            _tweetProcessor = tweetProcessor;
            _httpClient = httpClient;
            _telementryClient = telementryClient;
        }

        public async Task CancelTwitterStreamProcessing()
        {
            cancelRequested = true;
        }

        public async Task ProcessTwitterStream(CancellationToken cancellationToken)
        {
            using (Stream stream = await _httpClient.GetStreamAsync("2/tweets/sample/stream?tweet.fields=created_at,entities"))
            using (var streamReader = new StreamReader(stream))
            {
                while (!cancellationToken.IsCancellationRequested && !cancelRequested)
                {
                    if (streamReader.EndOfStream)
                    {
                        _telementryClient.TrackTrace($"{nameof(TwitterStreamProcessor)} end of stream reached when processing stream. Exiting processor due to unknown state.", SeverityLevel.Error);
                        return;
                    }

                    Task<string> tweetDataTask;

                    try
                    {
                        tweetDataTask = streamReader.ReadLineAsync();
                    }
                    catch(Exception ex)
                    {
                        _telementryClient.TrackTrace($"{nameof(TwitterStreamProcessor)} threw an exception when attempting to read line async.", SeverityLevel.Error);
                        _telementryClient.TrackException(ex);
                        continue;
                    }

                    Task.Run(() => _tweetProcessor.Process(tweetDataTask));
                }
            }
        }
    }
}

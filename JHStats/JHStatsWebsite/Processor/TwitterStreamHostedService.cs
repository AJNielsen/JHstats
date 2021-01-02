using JHStatsWebsite.Processor.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JHStatsWebsite.Processor
{
    public class TwitterStreamHostedService : IHostedService
    {
        private ITwitterStreamProcessor _twitterStreamProcessor;
        private readonly TelemetryClient _telementryClient;
        public bool cancelRequested = false;

        public TwitterStreamHostedService(ITwitterStreamProcessor twitterStreamProcessor, TelemetryClient telementryClient)
        {
            _twitterStreamProcessor = twitterStreamProcessor;
            _telementryClient = telementryClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _telementryClient.TrackTrace($"Starting {nameof(TwitterStreamHostedService)}.", SeverityLevel.Information);

            while (!cancellationToken.IsCancellationRequested && !cancelRequested)
            {
                Task twitterStreamTask = _twitterStreamProcessor.ProcessTwitterStream(cancellationToken);
                try
                {
                    await twitterStreamTask.ConfigureAwait(false);
                }
                catch(Exception ex)
                {
                    _telementryClient.TrackTrace($"Exception thrown when waiting for the stream processor to complete.", SeverityLevel.Error);
                    _telementryClient.TrackException(ex);
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _telementryClient.TrackTrace($"Stopping {nameof(TwitterStreamHostedService)}.", SeverityLevel.Information);
            cancelRequested = true;
            await _twitterStreamProcessor.CancelTwitterStreamProcessing();
        }
    }
}

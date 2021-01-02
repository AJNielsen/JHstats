using JHStatsWebsite.Processor;
using JHStatsWebsite.Processor.Interfaces;
using Microsoft.ApplicationInsights;
using Moq;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace JHStatsWebsite.Tests.Processor
{
    public class TwitterStreamHostedServiceTests
    {
        TwitterStreamHostedService _twitterStreamHostedService;
        Mock<ITwitterStreamProcessor> _mockTwitterStreamProcessor;

        public TwitterStreamHostedServiceTests()
        {
            _mockTwitterStreamProcessor = new Mock<ITwitterStreamProcessor>();
            _twitterStreamHostedService = new TwitterStreamHostedService(_mockTwitterStreamProcessor.Object, new TelemetryClient());
        }

        public object StopWatch { get; private set; }

        [Fact]
        public void StartBeginsProcessingTwitterStream()
        {
            _mockTwitterStreamProcessor.Setup(mtsp => mtsp.ProcessTwitterStream(It.IsAny<CancellationToken>())).Returns(Task.Delay(10000));
            Task startingTask = _twitterStreamHostedService.StartAsync(new CancellationToken());
            _mockTwitterStreamProcessor.Verify(mtsp => mtsp.ProcessTwitterStream(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void StartEndsIfCancellationRequested()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            _mockTwitterStreamProcessor.Setup(mtsp => mtsp.ProcessTwitterStream(It.IsAny<CancellationToken>())).Returns(Task.Delay(10000));

            Task startingTask = _twitterStreamHostedService.StartAsync(cancellationTokenSource.Token);
            Assert.True(startingTask.Wait(1000));

            _mockTwitterStreamProcessor.Verify(mtsp => mtsp.ProcessTwitterStream(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        public void StartBeginsProcessingTwitterStreamAndRestartsOnFailure()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            _mockTwitterStreamProcessor.SetupSequence(mtsp => mtsp.ProcessTwitterStream(It.IsAny<CancellationToken>()))
                .Returns(Task.Run(async () =>
                {
                    throw new Exception("Exception to test retry.");
                }))
                .Returns(Task.Run(async () =>
                {
                    while (!cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        await Task.Delay(1);
                    }
                }));

            Task startingTask = _twitterStreamHostedService.StartAsync(cancellationTokenSource.Token);

            Stopwatch sw = Stopwatch.StartNew();
            while (_mockTwitterStreamProcessor.Invocations.Count < 2 && sw.Elapsed.TotalSeconds < 1) ;
            sw.Stop();
            Assert.True(sw.Elapsed.TotalSeconds < 1);

            cancellationTokenSource.Cancel();
            Assert.True(startingTask.Wait(1000));
            _mockTwitterStreamProcessor.Verify(mtsp => mtsp.ProcessTwitterStream(It.IsAny<CancellationToken>()), Times.AtLeast(2));
        }

        [Fact]
        public void StopAsyncTogglesTheFlagToGracefullyStopTheRunningProcess()
        {
            bool stopProcessing = false;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            _mockTwitterStreamProcessor.Setup(mtsp => mtsp.ProcessTwitterStream(It.IsAny<CancellationToken>())).Returns(Task.Run(() =>
            {
                while (!stopProcessing)
                {
                    Task.Delay(1).Wait();
                }
            }));

            _mockTwitterStreamProcessor.Setup(mtsp => mtsp.CancelTwitterStreamProcessing()).Callback(() => stopProcessing = true);

            Task startingTask = _twitterStreamHostedService.StartAsync(cancellationTokenSource.Token);
            Task stopTask = _twitterStreamHostedService.StopAsync(cancellationTokenSource.Token);

            Assert.True(stopTask.Wait(1000));
            Assert.True(startingTask.Wait(1000));

            _mockTwitterStreamProcessor.Verify(mtsp => mtsp.CancelTwitterStreamProcessing(), Times.Once);
            _mockTwitterStreamProcessor.Verify(mtsp => mtsp.ProcessTwitterStream(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

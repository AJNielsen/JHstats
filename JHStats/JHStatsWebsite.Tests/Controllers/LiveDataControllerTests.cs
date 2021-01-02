using JHStatsWebsite.Controllers;
using JHStatsWebsite.Data.Interfaces;
using JHStatsWebsite.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace JHStatsWebsite.Tests.Controllers
{
    public class LiveDataControllerTests
    {
        [Fact]
        public void EnsureDependenciesAreSetup()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build();

            ServiceCollection services = new ServiceCollection();
            services.AddTransient<LiveDataController>();

            Startup startup = new Startup(configuration);
            startup.ConfigureServices(services);

            LiveDataController liveDataController = services.BuildServiceProvider().GetService<LiveDataController>();

            Assert.NotNull(liveDataController);
        }

        [Fact]
        public void RequestToLiveDataControllerReturnsTwitterStatsDetails()
        {
            TwitterStatsDetails expectedTwitterStatsDetails = new TwitterStatsDetails();

            Mock<ITwitterEstimatedLiveDataRetriever> mockTwitterEstimatedLiveDataRetriever = new Mock<ITwitterEstimatedLiveDataRetriever>();
            mockTwitterEstimatedLiveDataRetriever.Setup(mteldr => mteldr.GetStats()).Returns(expectedTwitterStatsDetails);

            LiveDataController liveDataController = new LiveDataController(mockTwitterEstimatedLiveDataRetriever.Object);
            TwitterStatsDetails returnedTwitterStatsDetails = liveDataController.Get();

            Assert.Equal(expectedTwitterStatsDetails, returnedTwitterStatsDetails);
        }
    }
}

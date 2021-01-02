using JHStatsWebsite.Controllers;
using JHStatsWebsite.Data.Interfaces;
using JHStatsWebsite.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace JHStatsWebsite.Tests.Controllers
{
    public class SampleDataControllerTests
    {
        [Fact]
        public void EnsureDependenciesAreSetup()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build();

            ServiceCollection services = new ServiceCollection();
            services.AddTransient<SampleDataController>();

            Startup startup = new Startup(configuration);
            startup.ConfigureServices(services);

            SampleDataController sampleDataController = services.BuildServiceProvider().GetService<SampleDataController>();

            Assert.NotNull(sampleDataController);
        }

        [Fact]
        public void RequestToLiveDataControllerReturnsTwitterStatsDetails()
        {
            TwitterStatsDetails expectedTwitterStatsDetails = new TwitterStatsDetails();

            Mock<ITwitterSampleDataRetriever> mockTwitterSampleDataRetriever = new Mock<ITwitterSampleDataRetriever>();
            mockTwitterSampleDataRetriever.Setup(mteldr => mteldr.GetStats()).Returns(expectedTwitterStatsDetails);

            SampleDataController sampleDataController = new SampleDataController(mockTwitterSampleDataRetriever.Object);
            TwitterStatsDetails returnedTwitterStatsDetails = sampleDataController.Get();

            Assert.Equal(expectedTwitterStatsDetails, returnedTwitterStatsDetails);
        }
    }
}

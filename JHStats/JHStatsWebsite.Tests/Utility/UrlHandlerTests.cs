using JHStatsWebsite.Configuration;
using JHStatsWebsite.Utility;
using JHStatsWebsite.Utility.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace JHStatsWebsite.Tests.Utility
{
    public class UrlHandlerTests
    {
        UrlHandler _urlHandler;
        ImageDomainConfiguration _imageDomainConfiguration;
        string[] _imageDomains = { "pic.twitter.com", "instagram.com", "imgur.com" };

        public UrlHandlerTests()
        {
            _imageDomainConfiguration = new ImageDomainConfiguration
            {
                DomainList = _imageDomains
            };
            _urlHandler = new UrlHandler(_imageDomainConfiguration, new TelemetryClient());
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

            IUrlHandler urlHandler = services.BuildServiceProvider().GetService<IUrlHandler>();

            Assert.NotNull(urlHandler);
            Assert.IsType<UrlHandler>(urlHandler);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("         ")]
        public void GivenNullEmptyOrWhitespaceDomainDomainIsImageDomainReturnsFalse(string domainValue)
        {
            bool result = _urlHandler.DomainIsImageDomain(domainValue);
            Assert.False(result);
        }

        [Fact]
        public void GivenImageDomainDomainIsImageReturnsTrue()
        {
            bool result = _urlHandler.DomainIsImageDomain(_imageDomains[0]);
            Assert.True(result);
        }

        [Fact]
        public void GivenImageDomainDomainIsImageReturnsFalse()
        {
            bool result = _urlHandler.DomainIsImageDomain("NotAnImageDomain.com");
            Assert.False(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("         ")]
        public void GivenNullEmptyOrWhitespaceUrlGetDomainFromUrlReturnsEmptyString(string url)
        {
            string domain = _urlHandler.GetDomainFromUrl(url);
            Assert.Equal(string.Empty, domain);
        }

        [Fact]
        public void GivenAnInvalidUriGetDomainFromUrlReturnsEmptyString()
        {
            string domain = _urlHandler.GetDomainFromUrl("NotA#Uri");
            Assert.Equal(string.Empty, domain);
        }

        [Fact]
        public void GivenAValidUriGetDomainFromUrlReturnsDomain()
        {
            string expectedDomain = "validdomain.com";
            string domain = _urlHandler.GetDomainFromUrl($"https://{expectedDomain}/api/realendpoint");
            Assert.Equal(expectedDomain, domain);
        }

        [Fact]
        public void GivenAValidUriWithWWWGetDomainFromUrlReturnsDomainWithoutWWW()
        {
            string expectedDomain = "validdomain.com";
            string domain = _urlHandler.GetDomainFromUrl($"https://www.{expectedDomain}/api/realendpoint");
            Assert.Equal(expectedDomain, domain);
        }
    }
}

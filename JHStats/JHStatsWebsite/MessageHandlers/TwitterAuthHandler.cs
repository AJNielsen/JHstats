using JHStatsWebsite.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace JHStatsWebsite.MessageHandlers
{
    public class TwitterAuthHandler : DelegatingHandler
    {
        private readonly TwitterConfiguration _twitterAuthConfiguration;

        public TwitterAuthHandler(TwitterConfiguration twitterAuthConfiguration)
        {
            _twitterAuthConfiguration = twitterAuthConfiguration;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _twitterAuthConfiguration.BearerToken);

            return base.SendAsync(request, cancellationToken);
        }
    }
}

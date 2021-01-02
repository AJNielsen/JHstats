using JHStatsWebsite.Configuration;
using JHStatsWebsite.Utility.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Linq;

namespace JHStatsWebsite.Utility
{
    public class UrlHandler : IUrlHandler
    {
        ImageDomainConfiguration _imageDomainConfiguration;
        private readonly TelemetryClient _telementryClient;

        public UrlHandler(ImageDomainConfiguration imageDomainConfiguration, TelemetryClient telementryClient)
        {
            _imageDomainConfiguration = imageDomainConfiguration;
            _telementryClient = telementryClient;
        }

        public bool DomainIsImageDomain(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                return false;
            }

            return _imageDomainConfiguration.DomainList.Contains(domain.ToLower());
        }

        public string GetDomainFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            Uri uri;

            try
            {
                uri = new Uri(url.ToLower());
            }
            catch(Exception ex)
            {
                _telementryClient.TrackTrace($"{nameof(UrlHandler)} threw an exception when try to instantiate a new Uri from the provided url.", SeverityLevel.Error);
                _telementryClient.TrackException(ex);

                return string.Empty;
            }

            string host = uri.Host;

            if (host.StartsWith("www."))
            {
                return host.Substring(4);
            }
            return host;
        }
    }
}

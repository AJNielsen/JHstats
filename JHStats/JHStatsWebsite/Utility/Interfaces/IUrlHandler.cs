using System.Linq;

namespace JHStatsWebsite.Utility.Interfaces
{
    public interface IUrlHandler
    {
        bool DomainIsImageDomain(string domain);
        string GetDomainFromUrl(string url);
    }
}

using JHStatsWebsite.Models;

namespace JHStatsWebsite.Data.Interfaces
{
    public interface ITwitterSampleDataRetriever
    {
        TwitterStatsDetails GetStats();
    }
}

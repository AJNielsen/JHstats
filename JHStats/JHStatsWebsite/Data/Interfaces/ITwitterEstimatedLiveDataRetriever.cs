using JHStatsWebsite.Models;

namespace JHStatsWebsite.Data.Interfaces
{
    public interface ITwitterEstimatedLiveDataRetriever
    {
        TwitterStatsDetails GetStats();
    }
}

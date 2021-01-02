using System.Threading.Tasks;

namespace JHStatsWebsite.Processor.Interfaces
{
    public interface ITweetProcessor
    {
        Task Process(Task<string> singleTweetTask);
    }
}

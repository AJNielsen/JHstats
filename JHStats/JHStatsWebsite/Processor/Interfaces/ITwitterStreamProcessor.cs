using System.Threading;
using System.Threading.Tasks;

namespace JHStatsWebsite.Processor.Interfaces
{
    public interface ITwitterStreamProcessor
    {
        Task ProcessTwitterStream(CancellationToken cancellationToken);
        Task CancelTwitterStreamProcessing();
    }
}

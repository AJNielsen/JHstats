using JHStatsWebsite.Data.Interfaces;
using JHStatsWebsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace JHStatsWebsite.Controllers
{
    /// <summary>
    /// Controller to represent the sample data stream as estimated values for the full live data.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LiveDataController : ControllerBase
    {
        ITwitterEstimatedLiveDataRetriever _twitterEstimatedLiveDataRetriever;

        public LiveDataController(ITwitterEstimatedLiveDataRetriever twitterEstimatedLiveDataRetriever)
        {
            _twitterEstimatedLiveDataRetriever = twitterEstimatedLiveDataRetriever;
        }

        [HttpGet]
        public TwitterStatsDetails Get()
        {
            return _twitterEstimatedLiveDataRetriever.GetStats();
        }
    }
}

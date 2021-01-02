using JHStatsWebsite.Data.Interfaces;
using JHStatsWebsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace JHStatsWebsite.Controllers
{
    /// <summary>
    /// Controller that represents the stats from the sample stream.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SampleDataController : ControllerBase
    {
        ITwitterSampleDataRetriever _sampleDataRetriever;

        public SampleDataController(ITwitterSampleDataRetriever sampleDataRetriever)
        {
            _sampleDataRetriever = sampleDataRetriever;
        }

        [HttpGet]
        public TwitterStatsDetails Get()
        {   
            return _sampleDataRetriever.GetStats();
        }
    }
}

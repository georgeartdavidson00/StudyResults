using Microsoft.AspNetCore.Mvc;
using StudyResults.DTOs;
using StudyResults.Services;

namespace StudyResults.Controllers;

[ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly IStudyResultsService _studyResultsService;
        
        public StatsController(IStudyResultsService studyResultsService)
        {
            _studyResultsService = studyResultsService;
        }
        
        /// <summary>
        /// Get summary statistics for a specific metric
        /// </summary>
        /// <param name="metric">Metric name</param>
        /// <param name="from">Start date filter</param>
        /// <param name="to">End date filter</param>
        /// <returns>Summary statistics including average, min, max, count, and standard deviation</returns>
        [HttpGet]
        [ProducesResponseType(typeof(SummaryStatsDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetSummaryStats(
            [FromQuery] string metric,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            if (string.IsNullOrEmpty(metric))
            {
                return BadRequest("Metric parameter is required");
            }
            
            try
            {
                var stats = await _studyResultsService.GetSummaryStatsAsync(metric, from, to);
                return Ok(stats);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
        
        /// <summary>
        /// Get time series data for a specific metric
        /// </summary>
        /// <param name="metric">Metric name</param>
        /// <param name="from">Start date filter</param>
        /// <param name="to">End date filter</param>
        /// <returns>Time series data with daily averages</returns>
        [HttpGet("timeseries")]
        [ProducesResponseType(typeof(List<TimeSeriesDataDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetTimeSeriesData(
            [FromQuery] string metric,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            if (string.IsNullOrEmpty(metric))
            {
                return BadRequest("Metric parameter is required");
            }
            
            var data = await _studyResultsService.GetTimeSeriesDataAsync(metric, from, to);
            return Ok(data);
        }
    }
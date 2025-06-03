using Microsoft.AspNetCore.Mvc;
using StudyResults.DTOs;
using StudyResults.Services;

namespace StudyResults.Controllers;

 [ApiController]
    [Route("api/[controller]")]
    public class StudyResultsController : ControllerBase
    {
        private readonly IStudyResultsService _studyResultsService;
        
        public StudyResultsController(IStudyResultsService studyResultsService)
        {
            _studyResultsService = studyResultsService;
        }
        
       
        [HttpGet]
        [ProducesResponseType(typeof(List<StudyResultDto>), 200)]
        public async Task<IActionResult> GetStudyResults(
            [FromQuery] string? metricName = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] string? participantId = null)
        {
            var results = await _studyResultsService.GetStudyResultsAsync(metricName, from, to, participantId);
            return Ok(results);
        }
        
        
        [HttpGet("metrics")]
        [ProducesResponseType(typeof(List<string>), 200)]
        public async Task<IActionResult> GetAvailableMetrics()
        {
            var metrics = await _studyResultsService.GetAvailableMetricsAsync();
            return Ok(metrics);
        }
        
      
        [HttpGet("participants")]
        [ProducesResponseType(typeof(List<string>), 200)]
        public async Task<IActionResult> GetParticipantIds()
        {
            var participants = await _studyResultsService.GetParticipantIdsAsync();
            return Ok(participants);
        }
    }
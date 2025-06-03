using Microsoft.AspNetCore.Mvc;
using StudyResults.DTOs;
using StudyResults.Services;

namespace StudyResults.Controllers;

[ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly ICsvProcessingService _csvProcessingService;
        private readonly ILogger<UploadController> _logger;
        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
        
        public UploadController(ICsvProcessingService csvProcessingService, ILogger<UploadController> logger)
        {
            _csvProcessingService = csvProcessingService;
            _logger = logger;
        }
        
       
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(UploadResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(413)]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }
    
            if (file.Length > 10 * 1024 * 1024) // 10MB limit
            {
                return BadRequest("File too large");
            }
    
            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only CSV files are allowed");
            }
    
            try
            {
                
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
        
                var result = await _csvProcessingService.ProcessCsvAsync(memoryStream, file.FileName);
        
                if (result.Status == "Failed")
                {
                    return BadRequest(result);
                }
        
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file {FileName}", file.FileName);
                return StatusCode(500, "Internal server error during file processing");
            }
        }    }
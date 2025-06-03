using StudyResults.DTOs;
using StudyResults.Models;

namespace StudyResults.Services;

public interface ICsvProcessingService
{
    Task<UploadResponseDto> ProcessCsvAsync(Stream csvStream, string fileName);
    Task<List<StudyResult>> ParseCsvAsync(Stream csvStream);
    Task<bool> ValidateCsvStructureAsync(Stream csvStream);
}
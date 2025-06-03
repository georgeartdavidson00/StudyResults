using StudyResults.DTOs;

namespace StudyResults.Services;

public interface IStudyResultsService
{
    Task<List<StudyResultDto>> GetStudyResultsAsync(string? metricName = null, DateTime? from = null, DateTime? to = null, string? participantId = null);
    Task<SummaryStatsDto> GetSummaryStatsAsync(string metricName, DateTime? from = null, DateTime? to = null);
    Task<List<TimeSeriesDataDto>> GetTimeSeriesDataAsync(string metricName, DateTime? from = null, DateTime? to = null);
    Task<List<string>> GetAvailableMetricsAsync();
    Task<List<string>> GetParticipantIdsAsync();
}
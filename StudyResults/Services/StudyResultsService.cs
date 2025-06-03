using Microsoft.EntityFrameworkCore;
using StudyResults.Data;
using StudyResults.DTOs;

namespace StudyResults.Services;

public class StudyResultsService : IStudyResultsService
    {
        private readonly AppDbContext _context;
        
        public StudyResultsService(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<StudyResultDto>> GetStudyResultsAsync(string? metricName = null, DateTime? from = null, DateTime? to = null, string? participantId = null)
        {
            var query = _context.StudyResults.AsQueryable();
    
            if (!string.IsNullOrEmpty(metricName))
            {
                query = query.Where(r => r.MetricName == metricName);
            }
    
            if (!string.IsNullOrEmpty(participantId))
            {
                query = query.Where(r => r.ParticipantId == participantId);
            }
    
            // Convert DateTime parameters to UTC before querying
            if (from.HasValue)
            {
                var fromUtc = from.Value.Kind == DateTimeKind.Unspecified 
                    ? DateTime.SpecifyKind(from.Value, DateTimeKind.Utc) 
                    : from.Value.ToUniversalTime();
                query = query.Where(r => r.Timestamp >= fromUtc);
            }
    
            if (to.HasValue)
            {
                var toUtc = to.Value.Kind == DateTimeKind.Unspecified 
                    ? DateTime.SpecifyKind(to.Value, DateTimeKind.Utc) 
                    : to.Value.ToUniversalTime();
                query = query.Where(r => r.Timestamp <= toUtc);
            }
    
            var results = await query
                .OrderBy(r => r.Timestamp)
                .Select(r => new StudyResultDto
                {
                    Id = r.Id,
                    ParticipantId = r.ParticipantId,
                    MetricName = r.MetricName,
                    MetricValue = r.MetricValue,
                    Timestamp = r.Timestamp,
                    Category = r.Category,
                    Notes = r.Notes
                })
                .ToListAsync();
    
            return results;
        }        
        public async Task<SummaryStatsDto> GetSummaryStatsAsync(string metricName, DateTime? from = null, DateTime? to = null)
        {
            var query = _context.StudyResults
                .Where(r => r.MetricName == metricName);
    
            // Convert DateTime parameters to UTC before querying
            if (from.HasValue)
            {
                var fromUtc = from.Value.Kind == DateTimeKind.Unspecified 
                    ? DateTime.SpecifyKind(from.Value, DateTimeKind.Utc) 
                    : from.Value.ToUniversalTime();
                query = query.Where(r => r.Timestamp >= fromUtc);
            }
    
            if (to.HasValue)
            {
                var toUtc = to.Value.Kind == DateTimeKind.Unspecified 
                    ? DateTime.SpecifyKind(to.Value, DateTimeKind.Utc) 
                    : to.Value.ToUniversalTime();
                query = query.Where(r => r.Timestamp <= toUtc);
            }
    
            var values = await query.Select(r => r.MetricValue).ToListAsync();
    
            if (!values.Any())
            {
                return new SummaryStatsDto
                {
                    MetricName = metricName,
                    Count = 0,
                    Average = 0,
                    Min = 0,
                    Max = 0,
                    StandardDeviation = 0
                };
            }
    
            var average = values.Average();
            var variance = values.Select(v => Math.Pow(v - average, 2)).Average();
            var standardDeviation = Math.Sqrt(variance);
    
            return new SummaryStatsDto
            {
                MetricName = metricName,
                Count = values.Count,
                Average = average,
                Min = values.Min(),
                Max = values.Max(),
                StandardDeviation = standardDeviation
            };
        }
        
        public async Task<List<TimeSeriesDataDto>> GetTimeSeriesDataAsync(string metricName, DateTime? from = null, DateTime? to = null)
        {
            var query = _context.StudyResults
                .Where(r => r.MetricName == metricName);
    
            // Convert DateTime parameters to UTC before querying
            if (from.HasValue)
            {
                var fromUtc = from.Value.Kind == DateTimeKind.Unspecified 
                    ? DateTime.SpecifyKind(from.Value, DateTimeKind.Utc) 
                    : from.Value.ToUniversalTime();
                query = query.Where(r => r.Timestamp >= fromUtc);
            }
    
            if (to.HasValue)
            {
                var toUtc = to.Value.Kind == DateTimeKind.Unspecified 
                    ? DateTime.SpecifyKind(to.Value, DateTimeKind.Utc) 
                    : to.Value.ToUniversalTime();
                query = query.Where(r => r.Timestamp <= toUtc);
            }
    
            var results = await query
                .OrderBy(r => r.Timestamp)
                .Select(r => new TimeSeriesDataDto
                {
                    Timestamp = r.Timestamp,
                    Value = r.MetricValue
                })
                .ToListAsync();
    
            return results;
        }        
        public async Task<List<string>> GetAvailableMetricsAsync()
        {
            return await _context.StudyResults
                .Select(r => r.MetricName)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
        }
        
        public async Task<List<string>> GetParticipantIdsAsync()
        {
            return await _context.StudyResults
                .Select(r => r.ParticipantId)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();
        }
    }
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using StudyResults.Data;
using StudyResults.DTOs;
using StudyResults.Models;

namespace StudyResults.Services;

public class CsvProcessingService:ICsvProcessingService
{
        private readonly AppDbContext _context;
        private readonly ILogger<CsvProcessingService> _logger;
        
        public CsvProcessingService(AppDbContext context, ILogger<CsvProcessingService> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public async Task<UploadResponseDto> ProcessCsvAsync(Stream csvStream, string fileName)
        {
            var batch = new UploadBatch
            {
                FileName = fileName,
                FileSizeBytes = csvStream.Length,
                Status = UploadBatch.UploadStatus.Processing
            };
            
            _context.UploadBatches.Add(batch);
            await _context.SaveChangesAsync();
            
            try
            {
                var studyResults = await ParseCsvAsync(csvStream);
                
                foreach (var result in studyResults)
                {
                    result.UploadBatchId = batch.Id;
                }
                
                _context.StudyResults.AddRange(studyResults);
                
                batch.RecordCount = studyResults.Count;
                batch.Status = UploadBatch.UploadStatus.Completed;
                
                await _context.SaveChangesAsync();
                
                return new UploadResponseDto
                {
                    BatchId = batch.Id,
                    RecordsProcessed = studyResults.Count,
                    Status = "Success"
                };
            }
            catch (Exception ex)
            {
                batch.Status = UploadBatch.UploadStatus.Failed;
                batch.ErrorMessage = ex.Message;
                await _context.SaveChangesAsync();
                
                _logger.LogError(ex, "Error processing CSV file {FileName}", fileName);
                
                return new UploadResponseDto
                {
                    BatchId = batch.Id,
                    RecordsProcessed = 0,
                    Status = "Failed",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        
        public async Task<List<StudyResult>> ParseCsvAsync(Stream csvStream)
        {
            csvStream.Position = 0;
    
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim
            };
    
            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, config);
    
            var results = new List<StudyResult>();
    
            await csv.ReadAsync();
            csv.ReadHeader();
    
            while (await csv.ReadAsync())
            {
                try
                {
                    var timestampValue = csv.GetField<DateTime>("Timestamp");
            
                    var utcTimestamp = timestampValue.Kind switch
                    {
                        DateTimeKind.Utc => timestampValue,
                        DateTimeKind.Local => timestampValue.ToUniversalTime(),
                        DateTimeKind.Unspecified => DateTime.SpecifyKind(timestampValue, DateTimeKind.Utc),
                        _ => DateTime.SpecifyKind(timestampValue, DateTimeKind.Utc)
                    };
            
                    var result = new StudyResult
                    {
                        ParticipantId = csv.GetField<string>("ParticipantId") ?? throw new ArgumentException("ParticipantId is required"),
                        MetricName = csv.GetField<string>("MetricName") ?? throw new ArgumentException("MetricName is required"),
                        MetricValue = csv.GetField<double>("MetricValue"),
                        Timestamp = utcTimestamp, 
                        Category = csv.GetField<string>("Category"),
                        Notes = csv.GetField<string>("Notes")
                    };
            
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Skipping invalid row at line {LineNumber}: {Error}", csv.Parser.Row, ex.Message);
                }
            }
    
            return results;
        }
        
        public async Task<bool> ValidateCsvStructureAsync(Stream csvStream)
        {
            csvStream.Position = 0;
            
            try
            {
                using var reader = new StreamReader(csvStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                
                await csv.ReadAsync();
                csv.ReadHeader();
                
                var requiredHeaders = new[] { "ParticipantId", "MetricName", "MetricValue", "Timestamp" };
                var headers = csv.HeaderRecord;
                
                return requiredHeaders.All(h => headers != null && headers.Contains(h, StringComparer.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }
    }

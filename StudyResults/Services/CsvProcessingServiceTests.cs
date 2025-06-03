using Microsoft.EntityFrameworkCore;
using StudyResults.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using StudyResults.Models;
using Xunit;

namespace StudyResults.Services;

 public class CsvProcessingServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly Mock<ILogger<CsvProcessingService>> _loggerMock;
        private readonly CsvProcessingService _service;

        public CsvProcessingServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _loggerMock = new Mock<ILogger<CsvProcessingService>>();
            _service = new CsvProcessingService(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task ParseCsvAsync_ValidCsv_ReturnsStudyResults()
        {
            var csvContent = "ParticipantId,MetricName,MetricValue,Timestamp,Category,Notes\n" +
                           "P001,Accuracy,0.95,2024-01-01T10:00:00,Test,Good performance\n" +
                           "P002,Speed,120.5,2024-01-01T10:05:00,Test,Fast response";
            
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            var results = await _service.ParseCsvAsync(stream);

            Assert.Equal(2, results.Count);
            
            var first = results[0];
            Assert.Equal("P001", first.ParticipantId);
            Assert.Equal("Accuracy", first.MetricName);
            Assert.Equal(0.95, first.MetricValue);
            Assert.Equal("Test", first.Category);
            Assert.Equal("Good performance", first.Notes);
        }

        [Fact]
        public async Task ValidateCsvStructureAsync_ValidHeaders_ReturnsTrue()
        {
            var csvContent = "ParticipantId,MetricName,MetricValue,Timestamp\n" +
                           "P001,Accuracy,0.95,2024-01-01T10:00:00";
            
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            var isValid = await _service.ValidateCsvStructureAsync(stream);

            Assert.True(isValid);
        }

        [Fact]
        public async Task ValidateCsvStructureAsync_MissingRequiredHeaders_ReturnsFalse()
        {
            var csvContent = "ParticipantId,MetricName\n" +
                           "P001,Accuracy";
            
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            
            var isValid = await _service.ValidateCsvStructureAsync(stream);

            Assert.False(isValid);
        }

        [Fact]
        public async Task ProcessCsvAsync_ValidCsv_CreatesUploadBatchAndResults()
        {
            var csvContent = "ParticipantId,MetricName,MetricValue,Timestamp\n" +
                           "P001,Accuracy,0.95,2024-01-01T10:00:00\n" +
                           "P002,Speed,120.5,2024-01-01T10:05:00";
            
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            var result = await _service.ProcessCsvAsync(stream, "test.csv");

            
            Assert.Equal("Success", result.Status);
            Assert.Equal(2, result.RecordsProcessed);
            
            var batch = await _context.UploadBatches.Include(b => b.StudyResults).FirstAsync();
            Assert.Equal("test.csv", batch.FileName);
            Assert.Equal(UploadBatch.UploadStatus.Completed, batch.Status);
            Assert.Equal(2, batch.StudyResults.Count);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
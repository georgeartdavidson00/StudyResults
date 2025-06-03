using Microsoft.EntityFrameworkCore;
using StudyResults.Data;
using StudyResults.Models;
using Xunit;

namespace StudyResults.Services;

public class StudyResultsServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly StudyResultsService _service;

        public StudyResultsServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _service = new StudyResultsService(_context);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var batch = new UploadBatch
            {
                FileName = "test.csv",
                FileSizeBytes = 1000,
                RecordCount = 3,
                Status = UploadBatch.UploadStatus.Completed
            };

            _context.UploadBatches.Add(batch);
            _context.SaveChanges();

            var results = new List<StudyResult>
            {
                new StudyResult
                {
                    ParticipantId = "P001",
                    MetricName = "Accuracy",
                    MetricValue = 0.95,
                    Timestamp = new DateTime(2024, 1, 1, 10, 0, 0),
                    UploadBatchId = batch.Id
                },
                new StudyResult
                {
                    ParticipantId = "P001",
                    MetricName = "Accuracy",
                    MetricValue = 0.92,
                    Timestamp = new DateTime(2024, 1, 2, 10, 0, 0),
                    UploadBatchId = batch.Id
                },
                new StudyResult
                {
                    ParticipantId = "P002",
                    MetricName = "Speed",
                    MetricValue = 120.5,
                    Timestamp = new DateTime(2024, 1, 1, 10, 0, 0),
                    UploadBatchId = batch.Id
                }
            };

            _context.StudyResults.AddRange(results);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetSummaryStatsAsync_ValidMetric_ReturnsCorrectStats()
        {
            // Act
            var stats = await _service.GetSummaryStatsAsync("Accuracy");

            // Assert
            Assert.Equal("Accuracy", stats.MetricName);
            Assert.Equal(0.935, stats.Average, 3);
            Assert.Equal(0.92, stats.Min);
            Assert.Equal(0.95, stats.Max);
            Assert.Equal(2, stats.Count);
        }

        [Fact]
        public async Task GetAvailableMetricsAsync_ReturnsDistinctMetrics()
        {
            // Act
            var metrics = await _service.GetAvailableMetricsAsync();

            // Assert
            Assert.Equal(2, metrics.Count);
            Assert.Contains("Accuracy", metrics);
            Assert.Contains("Speed", metrics);
        }

        [Fact]
        public async Task GetStudyResultsAsync_WithFilters_ReturnsFilteredResults()
        {
            // Act
            var results = await _service.GetStudyResultsAsync(
                metricName: "Accuracy",
                from: new DateTime(2024, 1, 1),
                to: new DateTime(2024, 1, 1, 23, 59, 59));

            Assert.Single(results);
            Assert.Equal("P001", results[0].ParticipantId);
            Assert.Equal(0.95, results[0].MetricValue);
        }

        [Fact]
        public async Task GetTimeSeriesDataAsync_ValidMetric_ReturnsTimeSeriesData()
        {
            
            var data = await _service.GetTimeSeriesDataAsync("Accuracy");

            Assert.Equal(2, data.Count);
            Assert.Equal(new DateTime(2024, 1, 1).Date, data[0].Timestamp);
            Assert.Equal(0.95, data[0].Value);
            Assert.Equal(new DateTime(2024, 1, 2).Date, data[1].Timestamp);
            Assert.Equal(0.92, data[1].Value);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
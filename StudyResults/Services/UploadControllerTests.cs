using System.Text;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudyResults.Controllers;
using StudyResults.DTOs;
using Xunit;

namespace StudyResults.Services;

public class UploadControllerTests
    {
        private readonly Mock<ICsvProcessingService> _csvServiceMock;
        private readonly Mock<ILogger<UploadController>> _loggerMock;
        private readonly UploadController _controller;

        public UploadControllerTests()
        {
            _csvServiceMock = new Mock<ICsvProcessingService>();
            _loggerMock = new Mock<ILogger<UploadController>>();
            _controller = new UploadController(_csvServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task UploadCsv_ValidFile_ReturnsOkResult()
        {
            // Arrange
            var content = "ParticipantId,MetricName,MetricValue,Timestamp\nP001,Accuracy,0.95,2024-01-01T10:00:00";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.csv");
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            _csvServiceMock.Setup(s => s.ValidateCsvStructureAsync(It.IsAny<Stream>()))
                          .ReturnsAsync(true);
            
            _csvServiceMock.Setup(s => s.ProcessCsvAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                          .ReturnsAsync(new UploadResponseDto
                          {
                              BatchId = 1,
                              RecordsProcessed = 1,
                              Status = "Success"
                          });

            // Act
            var result = await _controller.UploadCsv(fileMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<UploadResponseDto>(okResult.Value);
            Assert.Equal("Success", response.Status);
            Assert.Equal(1, response.RecordsProcessed);
        }

        [Fact]
        public async Task UploadCsv_NoFile_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UploadCsv(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No file uploaded", badRequestResult.Value);
        }

        [Fact]
        public async Task UploadCsv_NonCsvFile_ReturnsBadRequest()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f => f.Length).Returns(100);

            // Act
            var result = await _controller.UploadCsv(fileMock.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Only CSV files are allowed", badRequestResult.Value);
        }
    }
using LogMonitoring.API.Controllers;
using LogMonitoring.Application.Interfaces;
using LogMonitoring.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace LogMonitoring.API.Tests.Controllers;

public class LogsControllerTests
{
    private readonly Mock<ILogParser> _mockLogParser;
    private readonly Mock<IJobProcessor> _mockJobProcessor;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<LogsController>> _mockLogger;
    private readonly LogsController _controller;

    public LogsControllerTests()
    {
        _mockLogParser = new Mock<ILogParser>();
        _mockJobProcessor = new Mock<IJobProcessor>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<LogsController>>();

        _controller = new LogsController(
            _mockLogParser.Object,
            _mockJobProcessor.Object,
            _mockConfiguration.Object,
            _mockLogger.Object);
    }

    [Fact]
    public void ProcessLogs_ShouldReturnOkObjectResult_WhenSuccessful()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["LogFilePath"]).Returns("dummy/path/logs.log");

        var fakeLogEntries = new List<LogEntry> { new(TimeSpan.Zero, "Test", Domain.Enums.LogEntryType.START, 1) };
        _mockLogParser.Setup(p => p.Parse(It.IsAny<string>())).Returns(fakeLogEntries);

        var fakeJobs = new List<Job> { new(1, "Test Job", TimeSpan.Zero) };
        _mockJobProcessor.Setup(p => p.ProcessLogEntries(It.IsAny<IEnumerable<LogEntry>>())).Returns(fakeJobs.AsReadOnly());

        // Act
        var result = _controller.ProcessLogs();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public void ProcessLogs_ShouldReturnBadRequest_WhenLogFilePathIsMissing()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["LogFilePath"]).Returns(string.Empty);

        // Act
        var result = _controller.ProcessLogs();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Log file path is not configured.", badRequestResult.Value);
    }

    [Fact]
    public void ProcessLogs_ShouldReturnOkWithMessage_WhenParserReturnsNoEntries()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["LogFilePath"]).Returns("dummy/path/logs.log");
        _mockLogParser.Setup(p => p.Parse(It.IsAny<string>())).Returns(new List<LogEntry>());

        // Act
        var result = _controller.ProcessLogs();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Log file is empty or could not be parsed. No jobs processed.", okResult.Value);
    }

    [Fact]
    public void ProcessLogs_ShouldReturn500InternalServerError_OnException()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["LogFilePath"]).Returns("dummy/path/logs.log");
        _mockLogParser.Setup(p => p.Parse(It.IsAny<string>())).Throws(new Exception("Parsing failed unexpectedly"));

        // Act
        var result = _controller.ProcessLogs();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An internal server error occurred.", statusCodeResult.Value);
    }
}

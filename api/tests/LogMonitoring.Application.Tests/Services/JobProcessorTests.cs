using LogMonitoring.Application.Services;
using LogMonitoring.Domain.Entities;
using LogMonitoring.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Moq;

namespace LogMonitoring.Application.Tests.Services;

public class JobProcessorTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;

    public JobProcessorTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
    }

    private JobProcessor CreateProcessor(int warningMinutes, int errorMinutes)
    {
        // Mock the configuration values
        var configSection = new Mock<IConfigurationSection>();
        configSection.Setup(x => x.Value).Returns(warningMinutes.ToString());
        _mockConfiguration.Setup(c => c.GetSection("JobThresholds:WarningThresholdMinutes")).Returns(configSection.Object);

        var errorConfigSection = new Mock<IConfigurationSection>();
        errorConfigSection.Setup(x => x.Value).Returns(errorMinutes.ToString());
        _mockConfiguration.Setup(c => c.GetSection("JobThresholds:ErrorThresholdMinutes")).Returns(errorConfigSection.Object);

        return new JobProcessor(_mockConfiguration.Object);
    }

    [Fact]
    public void ProcessLogEntries_ShouldCorrectlyProcessCompletedJob_WithOkStatus()
    {
        // Arrange
        var processor = CreateProcessor(warningMinutes: 2, errorMinutes: 5);
        var logEntries = new List<LogEntry>
        {
            new(new TimeSpan(10, 0, 0), "Job A", LogEntryType.START, 101),
            new(new TimeSpan(10, 1, 0), "Job A", LogEntryType.END, 101)
        };

        // Act
        var result = processor.ProcessLogEntries(logEntries);
        var job = result.First();

        // Assert
        Assert.Single(result);
        Assert.Equal(1, job.Duration?.Minutes);
        Assert.Equal(JobStatus.Completed, job.Status);
    }

    [Fact]
    public void ProcessLogEntries_ShouldAssignWarningStatus_WhenDurationExceedsWarningThreshold()
    {
        // Arrange
        var processor = CreateProcessor(warningMinutes: 1, errorMinutes: 5);
        var logEntries = new List<LogEntry>
        {
            new(new TimeSpan(10, 0, 0), "Job B", LogEntryType.START, 102),
            new(new TimeSpan(10, 2, 0), "Job B", LogEntryType.END, 102)
        };

        // Act
        var result = processor.ProcessLogEntries(logEntries);
        var job = result.First();

        // Assert
        Assert.Single(result);
        Assert.Equal(2, job.Duration?.Minutes);
        Assert.Equal(JobStatus.Warning, job.Status);
    }

    [Fact]
    public void ProcessLogEntries_ShouldAssignErrorStatus_WhenDurationExceedsErrorThreshold()
    {
        // Arrange
        var processor = CreateProcessor(warningMinutes: 1, errorMinutes: 3);
        var logEntries = new List<LogEntry>
        {
            new(new TimeSpan(10, 0, 0), "Job C", LogEntryType.START, 103),
            new(new TimeSpan(10, 4, 0), "Job C", LogEntryType.END, 103)
        };

        // Act
        var result = processor.ProcessLogEntries(logEntries);
        var job = result.First();

        // Assert
        Assert.Single(result);
        Assert.Equal(4, job.Duration?.Minutes);
        Assert.Equal(JobStatus.Error, job.Status);
    }

    [Fact]
    public void ProcessLogEntries_ShouldMarkJobAsRunning_WhenEndLogIsMissing()
    {
        // Arrange
        var processor = CreateProcessor(warningMinutes: 2, errorMinutes: 5);
        var logEntries = new List<LogEntry>
        {
            new(new TimeSpan(11, 0, 0), "Incomplete Job", LogEntryType.START, 201)
        };

        // Act
        var result = processor.ProcessLogEntries(logEntries);
        var job = result.First();

        // Assert
        Assert.Single(result);
        Assert.Null(job.EndTime);
        Assert.Null(job.Duration);
        Assert.Equal(JobStatus.Running, job.Status);
    }

    [Fact]
    public void ProcessLogEntries_ShouldReturnEmptyList_WhenNoLogEntriesProvided()
    {
        // Arrange
        var processor = CreateProcessor(warningMinutes: 2, errorMinutes: 5);
        var logEntries = new List<LogEntry>();

        // Act
        var result = processor.ProcessLogEntries(logEntries);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenErrorThresholdIsLessThanWarning()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => CreateProcessor(warningMinutes: 5, errorMinutes: 2));
    }
}

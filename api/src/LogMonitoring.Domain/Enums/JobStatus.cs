namespace LogMonitoring.Domain.Enums;

/// <summary>
/// Represents the current status of a tracked job.
/// </summary>
public enum JobStatus
{
    Running,
    Completed,
    Warning,
    Error
}

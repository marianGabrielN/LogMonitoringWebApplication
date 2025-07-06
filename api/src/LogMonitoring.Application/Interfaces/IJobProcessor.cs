using LogMonitoring.Domain.Entities;

namespace LogMonitoring.Application.Interfaces;

public interface IJobProcessor
{
    IReadOnlyList<Job> ProcessLogEntries(IEnumerable<LogEntry> logEntries);
}

using LogMonitoring.Domain.Entities;

namespace LogMonitoring.Application.Interfaces;

public interface ILogParser
{
    IEnumerable<LogEntry> Parse(string logFilePath);
}

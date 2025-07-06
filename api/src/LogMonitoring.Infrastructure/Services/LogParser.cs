using LogMonitoring.Application.Interfaces;
using LogMonitoring.Domain.Entities;
using LogMonitoring.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace LogMonitoring.Infrastructure.Services;

public class LogParser : ILogParser
{
    private readonly ILogger<LogParser> _logger;

    public LogParser(ILogger<LogParser> logger)
    {
        _logger = logger;
    }

    public IEnumerable<LogEntry> Parse(string logFilePath)
    {
        if (!File.Exists(logFilePath))
        {
            _logger.LogWarning("The log file was not found at: '{LogFilePath}'", logFilePath);
            return Enumerable.Empty<LogEntry>();
        }

        var logEntries = new List<LogEntry>();
        var lines = File.ReadLines(logFilePath);

        int lineNumber = 0;
        foreach (var line in lines)
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            string[] parts = line.Split(',');

            if (parts.Length != 4)
            {
                _logger.LogWarning("Line {LineNumber}: Expected 4 parts, but found {PartCount}. Skipping line: '{Line}'", lineNumber, parts.Length, line);
                continue;
            }

            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();
            }

            try
            {
                var timestamp = TimeSpan.ParseExact(parts[0], "hh\\:mm\\:ss", null);
                string description = parts[1];
                var type = Enum.Parse<LogEntryType>(parts[2], ignoreCase: true);
                int processId = int.Parse(parts[3]);

                logEntries.Add(new LogEntry(timestamp, description, type, processId));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Line {LineNumber}: An error occurred during parsing. Skipping line: '{Line}'", lineNumber, line);
            }
        }

        return logEntries;
    }
}

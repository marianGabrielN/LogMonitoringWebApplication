using LogMonitoring.Application.Interfaces;
using LogMonitoring.Domain.Entities;
using LogMonitoring.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace LogMonitoring.Application.Services;

public class JobProcessor : IJobProcessor
{
    private readonly TimeSpan _warningThreshold;
    private readonly TimeSpan _errorThreshold;

    public JobProcessor(IConfiguration configuration)
    {
        var warningThresholdMinutes = configuration.GetValue<int>("JobThresholds:WarningThresholdMinutes");
        var errorThresholdMinutes = configuration.GetValue<int>("JobThresholds:ErrorThresholdMinutes");

        if (errorThresholdMinutes < warningThresholdMinutes)
        {
            throw new ArgumentException("Error threshold must be greater than or equal to warning threshold.");
        }

        _warningThreshold = TimeSpan.FromMinutes(warningThresholdMinutes);
        _errorThreshold = TimeSpan.FromMinutes(errorThresholdMinutes);
    }

    public IReadOnlyList<Job> ProcessLogEntries(IEnumerable<LogEntry> logEntries)
    {
        var runningJobs = new Dictionary<int, Job>();
        var processedJobs = new List<Job>();

        foreach (var entry in logEntries.OrderBy(e => e.ProcessId))
        {
            if (entry.Type == LogEntryType.START)
            {
                var newJob = new Job(entry.ProcessId, entry.Description, entry.Timestamp);
                runningJobs.Add(entry.ProcessId, newJob);
            }
            else if (entry.Type == LogEntryType.END)
            {
                if (runningJobs.TryGetValue(entry.ProcessId, out var runningJob))
                {
                    runningJob.Complete(entry.Timestamp);
                    DetermineJobStatus(runningJob);
                    processedJobs.Add(runningJob);
                    runningJobs.Remove(entry.ProcessId);
                }
            }
        }

        foreach (var incompleteJob in runningJobs.Values)
        {
            processedJobs.Add(incompleteJob);
        }

        return processedJobs.AsReadOnly();
    }

    private void DetermineJobStatus(Job job)
    {
        if (!job.Duration.HasValue) return;

        if (job.Duration.Value >= _errorThreshold)
        {
            job.Status = JobStatus.Error;
        }
        else if (job.Duration.Value >= _warningThreshold)
        {
            job.Status = JobStatus.Warning;
        }
        else
        {
            job.Status = JobStatus.Completed;
        }
    }
}

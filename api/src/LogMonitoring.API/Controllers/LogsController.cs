using LogMonitoring.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LogMonitoring.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController(
    ILogParser logParser,
    IJobProcessor jobProcessor,
    IConfiguration configuration,
    ILogger<LogsController> logger) : ControllerBase
{
    private readonly ILogParser _logParser = logParser;
    private readonly IJobProcessor _jobProcessor = jobProcessor;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<LogsController> _logger = logger;

    [HttpPost("process")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ProcessLogs()
    {
        try
        {
            string? logFilePath = _configuration["LogFilePath"];
            if (string.IsNullOrEmpty(logFilePath))
            {
                _logger.LogError("LogFilePath is not configured in appsettings.json.");
                return BadRequest("Log file path is not configured.");
            }

            // Task 1: Parse the log file
            var entries = _logParser.Parse(logFilePath);
            if (!entries.Any())
            {
                return Ok("Log file is empty or could not be parsed. No jobs processed.");
            }

            // Task 2: Identify and track jobs
            var jobs = _jobProcessor.ProcessLogEntries(entries);

            // Task 3: Return the report as a JSON response
            return Ok(new
            {
                totalJobsProcessed = jobs.Count,
                completed = jobs.Count(j => j.Status == Domain.Enums.JobStatus.Completed),
                warnings = jobs.Count(j => j.Status == Domain.Enums.JobStatus.Warning),
                errors = jobs.Count(j => j.Status == Domain.Enums.JobStatus.Error),
                running = jobs.Count(j => j.Status == Domain.Enums.JobStatus.Running),
                jobs = jobs.OrderBy(j => j.ProcessId).ThenBy(j => j.StartTime)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while processing logs.");
            return StatusCode(500, "An internal server error occurred.");
        }
    }
}

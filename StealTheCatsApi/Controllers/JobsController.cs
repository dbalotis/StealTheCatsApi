using Hangfire;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public class JobsController : ControllerBase
{
    /// <summary>
    /// Returns the status of the Hangfire background job.
    /// </summary>
    /// <param name="jobId">The background job id</param>
    /// <returns>Information about the job. We are mainly interested about its status</returns>
    [HttpGet("api/jobs/{jobId}")]
    public IActionResult GetJobStatus(string jobId)
    {
        using (var connection = JobStorage.Current.GetConnection())
        {
            var jobData = connection.GetJobData(jobId);

            if (jobData == null)
                return NotFound("Job not found");

            return Ok(new
            {
                JobId = jobId,
                State = jobData.State,
                CreatedAt = jobData.CreatedAt,
                Job = jobData.Job?.ToString()
            });
        }
    }
}

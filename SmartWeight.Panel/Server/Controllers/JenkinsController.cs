using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Narochno.Jenkins;

namespace SmartWeight.Panel.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JenkinsController : ControllerBase
    {
        private readonly JenkinsClient _jenkinsClient;
        public JenkinsController(JenkinsClient jenkinsClient)
        {
            _jenkinsClient = jenkinsClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("SearchJobs")]
        public async Task<IEnumerable<string>> GetJobsAsync(string? query = "")
        {
            var master = await _jenkinsClient.GetMaster(CancellationToken.None);
            var factories = master.Jobs
                .Select(job => job.Name)
                .Where(job => job.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Take(5);

            return factories;
        }
    }
}

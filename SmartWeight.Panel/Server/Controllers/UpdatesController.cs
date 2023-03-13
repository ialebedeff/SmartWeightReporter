using Database;
using Entities;
using Microsoft.AspNetCore.Mvc;
using SmartWeight.RemoteStorage;

namespace SmartWeight.Panel.Server.Controllers
{
    /// <summary>
    /// API контроллер для 
    /// взаимодействия со сборками
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UpdatesController : ControllerBase
    {
        private readonly BuildManager _buildManager;
        public UpdatesController(
            BuildManager buildManager,
            RemoteStorageProvider remoteStorageProvider)
        {
            _buildManager = buildManager;
        }
        /// <summary>
        /// Получить информацию по 
        /// определенному билду
        /// </summary>
        /// <param name="buildNumber"></param>
        /// <returns></returns>
        [HttpGet("GetBuildInformation")]
        public async Task<Build?> GetBuildInformationAsync(int buildNumber)
        {
            return await _buildManager.GetBuildAsync(buildNumber);
        } 
        /// <summary>
        /// Получить информацию по билдам
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetBuilds")]
        public Task<List<Build>> GetBuildsAsync()
            => _buildManager.GetAllBuildsAsync();
        /// <summary>
        /// Получить файл билда
        /// </summary>
        /// <param name="buildNumber"></param>
        /// <param name="buildFileNumber"></param>
        /// <returns></returns>
        [HttpGet("GetBuildFile")]
        public async Task<FileContentResult> GetPhysicalFileAsync(
            int buildNumber,
            int buildFileNumber)
            => new FileContentResult(
                  await _buildManager.GetBuildFileAsync(buildNumber, buildFileNumber)
                , "application/octet-stream");
    }
}

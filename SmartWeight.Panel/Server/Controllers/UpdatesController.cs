using Database;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWeight.RemoteStorage;
using System;

namespace SmartWeight.Panel.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpdatesController : ControllerBase
    {
        private readonly RemoteStorageProvider _remoteStorageProvider;
        private readonly BuildManager _buildManager;
        public UpdatesController(
            BuildManager buildManager,
            RemoteStorageProvider remoteStorageProvider)
        {
            _remoteStorageProvider = remoteStorageProvider;
            _buildManager = buildManager;
        }

        [HttpGet("GetBuildInformation")]
        public async Task<Build?> GetBuildInformationAsync(int buildNumber)
        {
            return await _buildManager.GetBuildAsync(buildNumber);
        } 

        [HttpGet("GetBuilds")]
        public async Task<List<Build>> GetBuildsAsync()
        {
            return await _buildManager.GetAllBuildsAsync();
        }

        [HttpGet("GetBuildFile")] //http://192.168.0.19:8080/job/%D0%90%D0%B2%D1%82%D0%BE%D0%B2%D0%B5%D1%81%D1%8B%20dev/lastSuccessfulBuild/artifact/bin/build/
        public async Task<FileContentResult> GetPhysicalFileAsync(
            int buildNumber,
            int buildFileNumber)
            => new FileContentResult(await _buildManager.GetBuildFileAsync(buildNumber, buildFileNumber), "application/octet-stream");
    }
}

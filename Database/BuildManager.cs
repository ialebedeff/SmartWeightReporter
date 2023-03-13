using Database.Repositories;
using Entities;
using Microsoft.EntityFrameworkCore;
using SmartWeight.RemoteStorage;

namespace Database;

public class BuildManager
{
    private readonly BuildRepository _buildRepository;
    private readonly RemoteStorageProvider _remoteStorageProvider;

    public BuildManager(
        BuildRepository buildRepository,
        RemoteStorageProvider remoteStorageProvider)
    {
        _buildRepository = buildRepository;
        _remoteStorageProvider = remoteStorageProvider;
    }

    /// <summary>
    /// Получить информацию по последней сборке 
    /// </summary>
    /// <returns></returns>
    public async Task<Build?> GetLastBuildAsync()
    { 
        var buildNumber = _remoteStorageProvider.GetLastBuildNumber();
        var build = await GetBuildAsync(buildNumber);

        return build;
    }

    /// <summary>
    /// Получить информацию по всем билдам
    /// </summary>
    /// <returns></returns>
    public Task<List<Build>> GetAllBuildsAsync()
        => _buildRepository.GetAllBuildsAsync();
    /// <summary>
    /// Получить информацию по определенной сборке
    /// </summary>
    /// <param name="buildNumber"></param>
    /// <returns></returns>
    public async Task<Build?> GetBuildAsync(int buildNumber)
    {
        var build = await _buildRepository
            .Context
            .Builds
            .Include(build => build.Binaries)
            .FirstOrDefaultAsync(build => build.Id == buildNumber);

        if (build is not null)
        {
            return build;
        }

        var buildInformation = await _remoteStorageProvider.GetBuildInformationAsync(buildNumber);

        if (buildInformation is null)
        {
            return null;
        }

        build = new Build()
        {
            Binaries = new List<BinaryFileInformation>(buildInformation),
            Id = buildNumber,
        };

        var result = await _buildRepository.Add(build);

        return result.Result;
    }

    public Task<byte[]> GetBuildFileAsync(
        int buildNumber, int fileBuildNumber)
        => _remoteStorageProvider.GetFileAsync(
            _buildRepository.GetFilePath(buildNumber, fileBuildNumber));
}

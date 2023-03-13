using Database.Repositories;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql.TypeMapping;
using SmartWeight.RemoteStorage;
using System.Security.Claims;

namespace Database;
public class NoteManager
{
    private readonly NoteRepository _noteRepository;
    private readonly FactoryManager _factoryManager;
    private readonly UserManager<User> _userManager;
    public NoteManager(
        FactoryManager factoryManager,
        NoteRepository noteRepository, 
        UserManager<User> userManager)
    {
        _noteRepository = noteRepository;
        _userManager = userManager;
        _factoryManager = factoryManager;
    }

    public async Task<OperationResult<Note>> CreateNoteAsync(Note note, ClaimsPrincipal author)
    {
        var user = await _userManager.GetUserAsync(author);
        var factory = await _factoryManager.GetFactoryAsync(note.FactoryId);
        if (user is null || factory is null) { return OperationResult<Note>.Failed("Не найден пользователь или завод"); }

        note.User = user;
        note.Factory = factory;

        return await _noteRepository.Add(note);
    }

    public Task<IEnumerable<Note>> GetNotesAsync(int factoryId, int skip, int take)
        => _noteRepository.GetNotesAsync(factoryId, skip, take);
}
public class FactoryManager
{
    private readonly FactoryRepository _factoryRepository;
    private readonly UserManager<User> _userManager;
    public FactoryManager(
        FactoryRepository factoryRepo,
        UserManager<User> userManager)
    {
        _factoryRepository = factoryRepo;
        _userManager = userManager;
    }

    public Task<IEnumerable<Factory>> SearchAsync(string? query)
        => _factoryRepository.SearchAsync(query);
    //public async Task<OperationResult> CreateFactoriesAsync(
    //    User user, string password, IEnumerable<Factory> factories)
    //{
    //    var userResult = await _userManager.CreateAsync(user, password);
    //    var operationResult = OperationResult.FromIdentityResult(userResult);

    //    if (operationResult.Succeed)
    //    { 
            
    //    }

    //    return operationResult;
    //}
    public async Task<OperationResult> CreateFactoryAsync(
        Factory factory,
        User user, string? password)
    {
        var factoryResult = await _factoryRepository.Add(factory);

        if (factoryResult.Succeed &&
            factoryResult.Result is not null)
        {
            var userResult = password is not null ? 
                await _userManager.CreateAsync(user, password) : 
                await _userManager.CreateAsync(user);

            if (userResult.Succeeded)
            {
                if (await _userManager.FindByNameAsync(user.UserName) is User createdUser)
                {
                    factoryResult.Result.User = createdUser;

                    await _userManager.AddToRoleAsync(user, "User");
                    await _userManager.AddClaimAsync(user, new Claim("FactoryId", factoryResult.Result.IdString));
                    await _factoryRepository.UpdateAsync(factoryResult.Result);

                    return OperationResult.Ok();
                }
            }

            await _factoryRepository.DeleteAsync(factoryResult.Result);

            return OperationResult.Failed(userResult.Errors);
        }

        return OperationResult.Failed($"Не удалось создать {nameof(factory)} запись");
    }

    public ValueTask<List<Factory>> GetAllFactoriesAsync()
        => new ValueTask<List<Factory>>(_factoryRepository.GetAllFactoriesAsync());
    public ValueTask<Factory?> GetFactoryAsync(int factoryId)
        => new ValueTask<Factory?>(_factoryRepository.GetFactoryAsync(factoryId));
    public ValueTask<IEnumerable<Factory>> GetFactoriesAsync(User user)
        => new ValueTask<IEnumerable<Factory>>(_factoryRepository.GetFactories(user));
    public async ValueTask<IEnumerable<Factory>> GetFactoriesAsync(ClaimsPrincipal userClaimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(userClaimsPrincipal);
        var factories = await GetFactoriesAsync(user);

        return factories;
    }
    public async Task<Factory?> FindFactoryByClaimsAsync(
        IEnumerable<Claim> claims)
    {
        var factoryIdString = claims
            .FirstOrDefault(claim => claim.Type == "FactoryId")?
            .Value;

        if (!string.IsNullOrEmpty(factoryIdString))
        {
            return await _factoryRepository.GetFactoryAsync(Convert.ToInt32(factoryIdString));
        }

        return null;
    }

    public async Task<Factory?> FindFactoryByUserIdAsync(
        int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var claims = await _userManager.GetClaimsAsync(user);
        var factory = await FindFactoryByClaimsAsync(claims);

        return factory;
    }

    public Task<OperationResult<Factory>> UpdateFactoryAsync(
        Factory factory)
        => _factoryRepository.UpdateAsync(factory);

    public ValueTask<Factory?> FindFactoryAsync(int id)
        => _factoryRepository.GetByIdAsync(id);

    public ValueTask<OperationResult> DeleteFactoryAsync(
        Factory? factory)
    {
        if (factory is null)
        {
            return new ValueTask<OperationResult>(OperationResult.Failed());
        }

        return new ValueTask<OperationResult>(_factoryRepository.DeleteAsync(factory));
    }
    public async ValueTask<OperationResult> DeleteFactoryAsync(
        int factoryId)
    {
        var factory = await FindFactoryAsync(factoryId);
        var result = await DeleteFactoryAsync(factory);

        return result;
    }
}

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

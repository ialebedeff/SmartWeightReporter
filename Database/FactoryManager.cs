using Database.Repositories;
using Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Database;
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

using AutoMapper;
using Database;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartWeight.Panel.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FactoryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly FactoryManager _factoryManager;
        public FactoryController(
            IMapper mapper,
            FactoryManager factoryManager)
        {
            _mapper = mapper;
            _factoryManager = factoryManager;
        }
        /// <summary>
        /// Создать новое производство
        /// </summary>
        /// <param name="factoryTitle"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("CreateFactoryPreview")]
        public Task<OperationResult> CreateFactoryAsync(string factoryTitle)
        {
            var factory = _mapper.Map<Factory>(factoryTitle); 
            var user = _mapper.Map<User>(factoryTitle);

            return _factoryManager.CreateFactoryAsync(factory, user, null);
        }
        /// <summary>
        /// Обновить данные о производстве
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("UpdateFactory")]
        public Task<OperationResult<Factory>> UpdateFactoryAsync(Factory factory)
            => _factoryManager.UpdateFactoryAsync(factory);
        /// <summary>
        /// Получить производства текущего пользователя
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetCurrentUserFactories")]
        public ValueTask<IEnumerable<Factory>> GetCurrentUserFactoriesAsync()
            => _factoryManager.GetFactoriesAsync(User);
        /// <summary>
        /// Удалить производство
        /// </summary>
        /// <param name="factoryId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteFactory")]
        public ValueTask<OperationResult> DeleteFactoryAsync(int factoryId)
            => _factoryManager.DeleteFactoryAsync(factoryId);
        /// <summary>
        /// Получить все производства
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllFactories")]
        public ValueTask<List<Factory>> GetAllFactoriesAsync()
            => _factoryManager.GetAllFactoriesAsync();
        /// <summary>
        /// Получить производство по идентификатору пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("GetFactoryByUser")]
        public Task<Factory?> GetFactoryAsync(int userId)
            => _factoryManager.FindFactoryByUserIdAsync(userId);
        /// <summary>
        /// Поиск производства по названию
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("Search")]
        public Task<IEnumerable<Factory>> SearchAsync(string? query = null)
            => _factoryManager.SearchAsync(query);
        /// <summary>
        /// Получить производство по идентификатору
        /// </summary>
        /// <param name="factoryId"></param>
        /// <returns></returns>
        [HttpGet("GetFactory")]
        [Authorize(Roles = "Admin")]
        public ValueTask<Factory?> GetFactoryByIdAsync(int factoryId)
            => _factoryManager.GetFactoryAsync(factoryId);
        /// <summary>
        /// Получить текущее производство
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCurrentFactory")]
        public Task<Factory?> GetCurrentFactoryAsync()
            => _factoryManager.FindFactoryByClaimsAsync(User.Claims);
    }
}

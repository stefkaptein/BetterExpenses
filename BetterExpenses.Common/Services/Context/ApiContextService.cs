using BetterExpenses.Common.Database.Sql;
using BetterExpenses.Common.Models.User;
using BetterExpenses.Common.Options;
using BetterExpenses.Common.ServiceModels;
using BetterExpenses.Common.Services.Crypto;
using Bunq.Sdk.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BetterExpenses.Common.Services.Context;

public interface IApiContextService
{
    public Task CreateAndSaveNewApiContext(Guid userId, string apiKey);
    public Task<ApiContext> GetApiContextOrLoad(Guid userId);
    public Task EnsureApiContextExists(Guid userId);
    public void RemoveApiContext(Guid userId);
}

public class ApiContextService(
    SqlDbContext dbContext,
    IApiContextCryptoFileService apiContextCryptoFileService,
    ApiContextCache apiContextCache,
    IOptions<ApiContextOptions> apiContextOptions) : IApiContextService
{
    private readonly IApiContextCryptoFileService _apiContextCryptoFileService = apiContextCryptoFileService;
    private readonly ApiContextCache _apiContextCache = apiContextCache;
    private readonly DbSet<BetterExpensesUser> _users = dbContext.Users;
    private readonly List<string> _allowedIps = apiContextOptions.Value.AllowedIps;

    public async Task CreateAndSaveNewApiContext(Guid userId, string apiKey)
    {
        var apiContext = ApiContext.Create(ApiEnvironmentType.PRODUCTION, apiKey, "Better Expenses Backend Server",
            _allowedIps);
        await _apiContextCryptoFileService.SaveApiContext(apiContext, userId);
        _apiContextCache.Add(userId, apiContext);
    }

    public async Task<ApiContext> LoadApiContextFromFile(Guid userId)
    {
        return await _apiContextCryptoFileService.LoadApiContext(userId);
    }

    public async Task EnsureApiContextExists(Guid userId) => await GetApiContextOrLoad(userId);
    public void RemoveApiContext(Guid userId)
    {
        _apiContextCryptoFileService.DeleteApiContext(userId);
    }

    public async Task<ApiContext> GetApiContextOrLoad(Guid userId)
    {
        if (!_apiContextCache.TryGetValue(userId, out var apiContext))
        {
            apiContext = await LoadApiContextFromFile(userId);
            _apiContextCache.Add(userId, apiContext);
        }

        if (apiContext == null)
        {
            throw new Exception(
                "This shouldn't be possible. The context cannot be null after it was just created or retrieved.");
        }

        return apiContext;
    }
}
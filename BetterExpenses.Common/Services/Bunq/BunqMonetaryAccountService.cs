using BetterExpenses.Common.Services.Context;
using Bunq.Sdk.Model.Generated.Endpoint;

namespace BetterExpenses.Common.Services.Bunq;

public interface IBunqMonetaryAccountService
{
    public Task<List<MonetaryAccount>> ListMonetaryAccountsAsync(Guid userId);
}

public class BunqMonetaryAccountService(IApiContextService contextService)
    : BunqApiService(contextService), IBunqMonetaryAccountService
{
    public async Task<List<MonetaryAccount>> ListMonetaryAccountsAsync(Guid userId)
    {
        GetClientAndUserId(userId, out var apiClient, out var bunqUserId);

        var url = $"user/{bunqUserId}/monetary-account";

        return await GetAllPaginationAsync<MonetaryAccount>(url, apiClient, false, pageSize: MaxPageSize);
    }

    public override bool IsAllFieldNull()
    {
        throw new NotImplementedException();
    }
}
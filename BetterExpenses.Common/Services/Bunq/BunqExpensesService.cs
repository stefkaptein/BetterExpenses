using AutoMapper;
using BetterExpenses.Common.Models.Expenses;
using BetterExpenses.Common.Services.Context;
using Bunq.Sdk.Model.Generated.Endpoint;

namespace BetterExpenses.Common.Services.Bunq;

public interface IBunqExpensesService
{
    public List<UserExpense> GetExpenses(Guid userId, int accountId, DateTime? fetchUntil = null);
    public List<UserExpense> GetExpensesFrom(Guid userId, int accountId, int lastFetchedId, DateTime? fetchUntil);
    public List<UserExpense> GetExpensesAfter(Guid userId, int accountId, int firstFetchedId);
}

public class BunqExpensesService(IApiContextService contextService, IMapper mapper)
    : BunqApiService(contextService), IBunqExpensesService
{
    private const int FetchExpensesPageSize = 100;

    public List<UserExpense> GetExpenses(Guid userId, int accountId, DateTime? fetchUntil = null)
    {
        GetClientAndUserId(userId, out var apiClient, out var bunqUserId);

        var url = $"user/{bunqUserId}/monetary-account/{accountId}/payment";

        var result = fetchUntil == null
            ? GetAllPagination<Payment>(url, apiClient, true, pageSize: FetchExpensesPageSize)
            : GetAllPagination<Payment>(url, apiClient, true, FinishCondition, FetchExpensesPageSize);

        return mapper.Map<List<UserExpense>>(result);
            
        bool FinishCondition (Payment p) => DateTime.Parse(p.Created) < fetchUntil;
    }

    public List<UserExpense> GetExpensesFrom(Guid userId, int accountId, int lastFetchedId, DateTime? fetchUntil)
    {
        GetClientAndUserId(userId, out var apiClient, out var bunqUserId);

        var url = $"user/{bunqUserId}/monetary-account/{accountId}/payment";

        var result = fetchUntil == null
            ? GetPaginationPrevious<Payment>(url, apiClient, true, lastFetchedId, pageSize: FetchExpensesPageSize)
            : GetPaginationPrevious<Payment>(url, apiClient, true, lastFetchedId, FinishCondition, FetchExpensesPageSize);

        return mapper.Map<List<UserExpense>>(result);
            
        bool FinishCondition (Payment p) => DateTime.Parse(p.Created) < fetchUntil;
    }

    public List<UserExpense> GetExpensesAfter(Guid userId, int accountId, int firstFetchedId)
    {
        GetClientAndUserId(userId, out var apiClient, out var bunqUserId);

        var url = $"user/{bunqUserId}/monetary-account/{accountId}/payment";

        var result = GetPaginationInFuture<Payment>(url, apiClient, true, firstFetchedId, FetchExpensesPageSize);

        return mapper.Map<List<UserExpense>>(result);
    }
}
using AutoMapper;
using BetterExpenses.Common.Models.Expenses;
using BetterExpenses.Common.ServiceModels;
using BetterExpenses.Common.Services.Context;
using Bunq.Sdk.Model.Generated.Endpoint;

namespace BetterExpenses.Common.Services.Bunq;

public interface IBunqExpensesService
{
    public List<UserExpense> GetExpenses(Guid userId, int accountId, DateTime? fetchUntil = null);
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
            ? GetAllPagination<Payment>(url, apiClient, true)
            : GetAllPagination(url, apiClient, true, (Func<Payment, bool>?)FinishCondition, FetchExpensesPageSize);

        return mapper.Map<List<UserExpense>>(result);
            
        bool FinishCondition (Payment p) => DateTime.Parse(p.Created) < fetchUntil;
    }
}
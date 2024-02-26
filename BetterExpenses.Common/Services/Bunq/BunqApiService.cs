using BetterExpenses.Common.Models.Exceptions;
using BetterExpenses.Common.ServiceModels;
using BetterExpenses.Common.Services.Context;
using Bunq.Sdk.Http;
using Bunq.Sdk.Model.Core;
using Bunq.Sdk.Model.Generated.Endpoint;

namespace BetterExpenses.Common.Services.Bunq;

public abstract class BunqApiService(IApiContextService contextService) : BunqModel
{
    private readonly IApiContextService contextService = contextService;

    public const int MaxPageSize = 200;
    
    protected static Dictionary<string, string> EmptyParameters { get; } = new();

    /// <summary>
    /// Gets the API context for the given user id.
    ///
    /// Throws an exception when there is no cached API context.
    /// </summary>
    /// <returns></returns>
    protected void GetClientAndUserId(Guid userId, out ApiClient apiClient, out int bunqUserId)
    {
        var apiContext = contextService.GetApiContextOrLoad(userId).Result 
                         ?? throw new NoApiContextForUserException(userId.ToString());
        apiClient = new ApiClient(apiContext);
        bunqUserId = apiContext.SessionContext.UserId;
    }

    protected static async Task<List<T>> GetAllPaginationAsync<T>(string url, ApiClient apiClient, bool wrap,
        Func<T, bool>? finishCondition = null, int pageSize = 10)
    {
        return await Task.Run(() => GetAllPagination(url, apiClient, wrap, finishCondition, pageSize));
    }

    /// <summary>
    /// Gets all the records for the given type and url.
    /// 
    /// Finish condition is checked for every item.
    /// when the finish condition is met the method stops and returns the list at that time.
    /// </summary>
    /// <param name="url">The url to fetch</param>
    /// <param name="apiClient">Api client to use for making the call</param>
    /// <param name="wrap">Whether the json response is wrapped</param>
    /// <param name="finishCondition">Function that determines if the pagination should stop at a specific item</param>
    /// <param name="pageSize">The size of the pages returned by the bunq API</param>
    /// <typeparam name="T">The return type</typeparam>
    /// <returns>Full list of all the items, or the items that do not satisfy the finish condition</returns>
    protected static List<T> GetAllPagination<T>(string url, ApiClient apiClient, bool wrap, Func<T, bool>? finishCondition = null, int pageSize = 10)
    {
        var pagination = new Pagination { Count = pageSize };
        var uriParams = pagination.UrlParamsCountOnly;
        List<T> resultList = [];
        while (true)
        {
            var rawResponse = apiClient.Get(url, uriParams, EmptyParameters);
            var response = wrap ? FromJsonList<T>(rawResponse, typeof(T).Name) : FromJsonList<T>(rawResponse);
            if (finishCondition != null)
            {
                foreach (var item in response.Value)
                {
                    resultList.Add(item);
                    if (!finishCondition(item)) continue;
                    
                    return resultList;
                }
            }
            else
            {
                resultList.AddRange(response.Value);
            }
            
            if (!response.Pagination.HasPreviousPage())
            {
                break;
            }

            uriParams = response.Pagination.UrlParamsPreviousPage;
        }

        return resultList;
    }
    
    public override bool IsAllFieldNull()
    {
        throw new NotImplementedException();
    }
}
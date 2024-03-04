using System.Collections.Specialized;
using System.Net.Http.Json;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using BetterExpenses.Web.Json;
using BetterExpenses.Web.Services.Api.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace BetterExpenses.Web.Services.Api;

public abstract class ApiService(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    protected async Task<ApiResponseModel<TResult>> PostModel<TResult, TRequest>(string url, TRequest requestModel,
        Dictionary<string, string>? queryParams = null)
    {
        var result = await _httpClient.PostAsJsonAsync(url, requestModel);
        var jsonString = await result.Content.ReadAsStringAsync();

        var content = BetterExpensesJsonSerializer.DeserializeRequired<TResult>(jsonString);

        return new ApiResponseModel<TResult>
        {
            StatusCode = result.StatusCode,
            Headers = result.Headers,
            ResponseModel = content
        };
    }

    protected async Task<ApiResponse> PostModel<TRequest>(string url, TRequest requestModel,
        Dictionary<string, string>? queryParams = null)
    {
        var result = await _httpClient.PostAsJsonAsync(url, requestModel);
        var content = await result.Content.ReadAsStringAsync();

        return new ApiResponse
        {
            StatusCode = result.StatusCode,
            Headers = result.Headers,
            Content = content
        };
    }

    protected async Task<ApiResponse> Post(string url, Dictionary<string, string>? queryParams = null)
    {
        url = GetUrlWithQuery(url, queryParams);
        var result = await _httpClient.PostAsync(url, null);
        
        var content = await result.Content.ReadAsStringAsync();

        return new ApiResponse
        {
            StatusCode = result.StatusCode,
            Headers = result.Headers,
            Content = content
        };
    }

    protected async Task<ApiResponse> Get(string url, Dictionary<string, string>? queryParams = null)
    {
        url = GetUrlWithQuery(url, queryParams);
        var result = await _httpClient.GetAsync(url);
        var content = await result.Content.ReadAsStringAsync();

        return new ApiResponse
        {
            StatusCode = result.StatusCode,
            Headers = result.Headers,
            Content = content
        };
    }
    
    protected async Task<ApiResponseModel<TResult>> GetJson<TResult>(string url, Dictionary<string, string>? queryParams = null)
    {
        url = GetUrlWithQuery(url, queryParams);
        var result = await _httpClient.GetAsync(url);
        var jsonString = await result.Content.ReadAsStringAsync();
        
        var content = BetterExpensesJsonSerializer.DeserializeRequired<TResult>(jsonString);

        return new ApiResponseModel<TResult>
        {
            StatusCode = result.StatusCode,
            Headers = result.Headers,
            ResponseModel = content
        };
    }

    private string GetUrlWithQuery(string url, IDictionary<string, string>? queryParams)
    {
        return queryParams == null ? url : QueryHelpers.AddQueryString(url, queryParams!);
    }
}
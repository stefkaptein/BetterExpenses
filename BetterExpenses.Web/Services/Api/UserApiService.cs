using BetterExpenses.Common.DTO.User;
using BetterExpenses.Common.Models.User;
using Bunq.Sdk.Model.Generated.Endpoint;

namespace BetterExpenses.Web.Services.Api;

public interface IUserApiService
{
    public Task<UserSettingsDto> GetUserSettings();
    public Task<List<UserMonetaryAccount>> GetUserAccounts();
    public Task UpdateAnalyseAccount(int accountId, bool analyse);
}

public class UserApiService(HttpClient httpClient) : ApiService(httpClient), IUserApiService
{
    private const string GetUserSettingsPath = "api/User/Settings";
    private const string GetMonetaryAccountsPath = "api/User/MonetaryAccounts";
    private const string UpdateMonetaryAnalyseAccountPath = "api/User/UpdateAnalyseAccount";
    
    public async Task<UserSettingsDto> GetUserSettings()
    {
        return (await GetJson<UserSettingsDto>(GetUserSettingsPath)).ResponseModel;
    }

    public async Task<List<UserMonetaryAccount>> GetUserAccounts()
    {
        return (await GetJson<List<UserMonetaryAccount>>(GetMonetaryAccountsPath)).ResponseModel;
    }

    public async Task UpdateAnalyseAccount(int accountId, bool analyse)
    {
        var qParams = new Dictionary<string, string>()
        {
            { nameof(accountId), accountId.ToString() },
            { nameof(analyse), analyse.ToString() }
        };
        await Post(UpdateMonetaryAnalyseAccountPath, qParams);
    }
}
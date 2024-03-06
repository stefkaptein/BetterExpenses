namespace BetterExpenses.Web.Services.Api;

public interface IUserTaskApiService
{
    public Task FetchAccounts();
    public Task FetchExpenses();
    public Task ProcessExpenses();
}

public class UserTaskApiService(HttpClient httpClient) : ApiService(httpClient), IUserTaskApiService
{
    private const string FetchExpensesPath = "/api/UserTask/FetchExpenses";
    private const string FetchAccountsPath = "/api/UserTask/FetchMonetaryAccounts";
    private const string ProcessExpensesPath = "/api/UserTask/ProcessExpenses";
    
    public async Task FetchAccounts()
    {
        await Get(FetchAccountsPath);
    }

    public async Task FetchExpenses()
    {
        await Get(FetchExpensesPath);
    }

    public async Task ProcessExpenses()
    {
        await Get(ProcessExpensesPath);
    }
}
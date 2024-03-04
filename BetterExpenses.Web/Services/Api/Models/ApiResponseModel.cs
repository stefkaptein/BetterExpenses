namespace BetterExpenses.Web.Services.Api.Models;

public class ApiResponseModel<T> : ApiResponse
{
    public required T ResponseModel { get; set; }
}
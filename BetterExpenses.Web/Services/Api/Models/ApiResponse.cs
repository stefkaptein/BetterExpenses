using System.Net;
using System.Net.Http.Headers;

namespace BetterExpenses.Web.Services.Api.Models;

public class ApiResponse
{
    public required HttpStatusCode StatusCode { get; set; }

    public bool IsSuccessStatusCode => (int)StatusCode >= 200 && (int)StatusCode <= 299;
    
    public required HttpResponseHeaders Headers { get; set; } 
    
    public string? Content { get; set; }
}
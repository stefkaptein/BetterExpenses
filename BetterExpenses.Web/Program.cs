using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BetterExpenses.Web;
using BetterExpenses.Web.Models.Options;
using BetterExpenses.Web.Services;
using BetterExpenses.Web.Services.Api;
using BetterExpenses.Web.Services.StateProviders;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseAddress = builder.Configuration["Api:BaseUrl"] ?? throw new Exception("API base address not configured");

builder.Services.AddScoped(sp => 
    new HttpClient
    {
        BaseAddress = new Uri(apiBaseAddress)
    });

builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Auth"));
builder.Services.AddBlazorBootstrap();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

builder.Services.AddScoped<IAuthApiService, AuthApiApiService>();
builder.Services.AddScoped<IUserApiService, UserApiService>();
builder.Services.AddScoped<IUserTaskApiService, UserTaskApiService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IGraphApiService, GraphApiService>();

await builder.Build().RunAsync();
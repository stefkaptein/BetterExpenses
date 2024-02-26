using BetterExpenses.CalculatorWorker.Workers;
using BetterExpenses.CalculatorWorker.Workers.Accounts;
using BetterExpenses.CalculatorWorker.Workers.Expenses;
using BetterExpenses.Common.Database;
using BetterExpenses.Common.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<FetchAccountsWorker>();
builder.Services.AddHostedService<FetchExpensesWorker>();

builder.Services.BindCommonConfiguration(builder.Configuration);

builder.Services.ConfigureCommonServices();
builder.Services.AddWorkerServices();

builder.Services.ConfigurePostgres(builder.Configuration);
builder.Services.ConfigureMongo(builder.Configuration);

var host = builder.Build();
host.Run();
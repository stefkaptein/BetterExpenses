using BetterExpenses.Common.Database.Mongo;
using BetterExpenses.Common.Models.Expenses;
using MongoDB.Driver;

namespace BetterExpenses.Common.Services.Mongo.Expenses;

public interface IFixedExpensesMongoService : IMongoService<FixedUserExpense>
{
}

public class FixedExpensesMongoService(IMongoConnection mongoConnection)
    : MongoService<FixedUserExpense>(mongoConnection), IFixedExpensesMongoService
{
}
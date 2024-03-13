using BetterExpenses.Common.Database.Mongo;
using BetterExpenses.Common.Models.Expenses;
using MongoDB.Driver;

namespace BetterExpenses.Common.Services.Mongo.Expenses;

public interface IExpensesMongoService : IMongoService<UserExpense>
{
    public Task<List<UserExpense>> GetExpensesForAccount(int accountId);
}

public class ExpensesMongoService(IMongoConnection mongoConnection)
    : MongoService<UserExpense>(mongoConnection), IExpensesMongoService
{
    public async Task<List<UserExpense>> GetExpensesForAccount(int accountId)
    {
        return await Collection
            .Find(x => x.MonetaryAccountId == accountId)
            .Sort(Builders<UserExpense>.Sort.Descending(x => x.Id))
            .ToListAsync();
    }
}
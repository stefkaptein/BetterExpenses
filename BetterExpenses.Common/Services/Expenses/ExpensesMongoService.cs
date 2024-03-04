using BetterExpenses.Common.Database.Mongo;
using BetterExpenses.Common.Models.Expenses;
using MongoDB.Driver;
// ReSharper disable PossibleMultipleEnumeration

namespace BetterExpenses.Common.Services.Expenses;

public interface IExpensesMongoService
{
    public Task InsertMany(IEnumerable<UserAccountExpensesList> toAdd);
}

public class ExpensesMongoService(IMongoConnection mongoConnection) : IExpensesMongoService
{
    private readonly IMongoCollection<UserAccountExpensesList> _userExpensesCollection =
        mongoConnection.GetCollection<UserAccountExpensesList>(MongoConnection.ExpensesCollectionName);

    public async Task InsertMany(IEnumerable<UserAccountExpensesList> toAdd)
    {
        if (!toAdd.Any())
        {
            return;
        }
        await _userExpensesCollection.InsertManyAsync(toAdd);
    }
}
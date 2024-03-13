using System.Linq.Expressions;
using BetterExpenses.Common.Database.Mongo;
using BetterExpenses.Common.Models.Expenses;
using MongoDB.Driver;
// ReSharper disable PossibleMultipleEnumeration

namespace BetterExpenses.Common.Services.Expenses;

public interface IExpensesMongoService
{
    public Task InsertMany(IEnumerable<UserExpense> toAdd);
    public Task BulkWrite(IEnumerable<WriteModel<UserExpense>> writeModels);
    public Task<List<UserExpense>> GetExpensesForAccount(int accountId);
}

public class ExpensesMongoService(IMongoConnection mongoConnection) : IExpensesMongoService
{
    private readonly IMongoCollection<UserExpense> _userExpensesCollection =
        mongoConnection.GetCollection<UserExpense>(MongoConnection.ExpensesCollectionName);

    public async Task InsertMany(IEnumerable<UserExpense> toAdd)
    {
        if (!toAdd.Any())
        {
            return;
        }
        await _userExpensesCollection.InsertManyAsync(toAdd);
    }

    public async Task BulkWrite(IEnumerable<WriteModel<UserExpense>> writeModels)
    {
        if (writeModels.Any())
        {
            await _userExpensesCollection.BulkWriteAsync(writeModels);
        }
    }

    public async Task<List<UserExpense>> GetExpensesForAccount(int accountId)
    {
        return await _userExpensesCollection
            .Find(x => x.MonetaryAccountId == accountId)
            .Sort(Builders<UserExpense>.Sort.Descending(x => x.Id))
            .ToListAsync();
    }
}
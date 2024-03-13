using BetterExpenses.Common.Database.Mongo;
using BetterExpenses.Common.Models.Graphs;
using MongoDB.Driver;

namespace BetterExpenses.Common.Services.Mongo.Expenses;

public interface IExpensesGraphMongoService : IMongoService<ExpensesGraph>
{
    public Task<ExpensesGraph?> GetGraphForUser(Guid userId);
}

public class ExpensesGraphMongoService(IMongoConnection mongoConnection)
    : MongoService<ExpensesGraph>(mongoConnection), IExpensesGraphMongoService
{
    public async Task<ExpensesGraph?> GetGraphForUser(Guid userId)
    {
        return await Collection
            .Find(x => x.UserId == userId)
            .FirstOrDefaultAsync();
        
    }
}
using BetterExpenses.Common.Database.Sql;
using BetterExpenses.Common.Models.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BetterExpenses.Common.Services.Tasks;

public interface ICalculatorTaskService
{
    public Task AddTask<T>(T task) where T : CalculatorTask;
    public Task<T?> GetNextTask<T>() where T : CalculatorTask;

    public Task DeleteTask<T>(Guid taskId) where T : CalculatorTask;
}

public class CalculatorTaskService(SqlDbContext dbContext, ILogger<CalculatorTaskService> logger)
    : ICalculatorTaskService
{
    public async Task AddTask<T>(T task) where T : CalculatorTask
    {
        dbContext.Add(task);
        await dbContext.SaveChangesAsync();
        
        logger.LogDebug("Added {TaskType} with ID {Id} to task queue", task.GetType().Name, task.Id);
    }

    public async Task<T?> GetNextTask<T>() where T : CalculatorTask
    {
        var task = await dbContext.Set<T>()
            .OrderBy(x => x.CreationDate)
            .FirstOrDefaultAsync();
        if (task == null)
        {
            logger.LogDebug("No {TaskType} in queue", typeof(T).Name);
        }

        return task;
    }

    public async Task DeleteTask<T>(Guid taskId) where T : CalculatorTask
    {
        var dbSet = dbContext.Set<T>();
        var task = await dbSet.FirstOrDefaultAsync(x => x.Id == taskId);
        if (task == null) return;
        
        dbSet.Remove(task);
        await dbContext.SaveChangesAsync();
        
        logger.LogDebug("Removed task with ID {Id} from the queue", taskId);
    }
}
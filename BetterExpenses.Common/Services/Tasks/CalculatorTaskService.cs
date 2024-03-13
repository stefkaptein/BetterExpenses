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
    public Task SetTaskRunning<T>(Guid taskId) where T : CalculatorTask;
    public Task SetTaskResult<T>(Guid taskId, CalculatorTaskStatus newStatus) where T : CalculatorTask;
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
            .Where(x => x.Status == CalculatorTaskStatus.Pending)
            .OrderByDescending(x => x.Priority)
            .ThenBy(x => x.CreationDate)
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

    public async Task SetTaskRunning<T>(Guid taskId) where T : CalculatorTask
    {
        const CalculatorTaskStatus newStatus = CalculatorTaskStatus.Running;
        
        var task = await GetTaskTracking<T>(taskId);
        if (task == null)
        {
            logger.LogWarning("Tried updating task {TaskId} to {Status}, but task does not exists", taskId, newStatus);
            return;
        }
        
        task.Status = newStatus;
        task.ProcessDate = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
    }

    public async Task SetTaskResult<T>(Guid taskId, CalculatorTaskStatus newStatus) where T : CalculatorTask
    {
        var task = await GetTaskTracking<T>(taskId);
        if (task == null)
        {
            logger.LogWarning("Tried updating task {TaskId} to {Status}, but task does not exists", taskId, newStatus);
            return;
        }

        task.Status = newStatus;
        task.FinishDate = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
    }

    private async Task<T?> GetTaskTracking<T>(Guid taskId) where T : CalculatorTask
    {
        var task = await dbContext.Set<T>()
            .FirstOrDefaultAsync(x => x.Id == taskId);
        if (task == null)
        {
            logger.LogWarning("Task not found {TaskId}", taskId);
        }

        return task;
    } 
}
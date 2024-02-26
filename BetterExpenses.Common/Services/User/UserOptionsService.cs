using AutoMapper;
using BetterExpenses.Common.Database.Sql;
using BetterExpenses.Common.Models.Exceptions;
using BetterExpenses.Common.Models.User;
using Microsoft.EntityFrameworkCore;

namespace BetterExpenses.Common.Services.User;

public interface IUserOptionsService
{
    public Task<UserOptions?> GetOptionsForUser(Guid userId);
    public Task UpdateUserOptions(Guid userId, UserOptions optionsUpdates);
}

public class UserOptionsService(SqlDbContext dbContext, IMapper mapper) : IUserOptionsService
{
    private readonly SqlDbContext _dbContext = dbContext;
    private readonly DbSet<UserOptions> _userOptionsSet = dbContext.UserOptions;

    public async Task<UserOptions?> GetOptionsForUser(Guid userId)
    {
        return await _userOptionsSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId);
    }

    public async Task UpdateUserOptions(Guid userId, UserOptions optionsUpdates)
    {
        var optionsFromDb = await _userOptionsSet
            .FirstOrDefaultAsync(x => x.Id == userId);
        if (optionsFromDb == null)
        {
            throw new IdNotFoundInDatabase(userId.ToString());
        }

        mapper.Map(optionsUpdates, optionsFromDb);
        await _dbContext.SaveChangesAsync();
    }
}
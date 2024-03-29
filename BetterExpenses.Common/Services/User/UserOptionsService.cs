﻿using AutoMapper;
using BetterExpenses.Common.Database.Sql;
using BetterExpenses.Common.Models.Exceptions;
using BetterExpenses.Common.Models.User;
using Microsoft.EntityFrameworkCore;

namespace BetterExpenses.Common.Services.User;

public interface IUserOptionsService
{
    public Task CreateDefaultUserSettings(Guid userId);
    public Task<UserSettings?> GetOptionsForUser(Guid userId);
    public Task<DateTime> GetFetchPaymentsTillForUser(Guid userId);
    public Task UpdateUserOptions(Guid userId, Action<UserSettings> settingsUpdates);
    public Task SetBunqLinked(Guid userId, bool value);
}

public class UserOptionsService(SqlDbContext dbContext, IMapper mapper) : IUserOptionsService
{
    private readonly SqlDbContext _dbContext = dbContext;
    private readonly DbSet<UserSettings> _userOptionsSet = dbContext.UserOptions;

    public async Task CreateDefaultUserSettings(Guid userId)
    {
        var newUserDefaultOptions = new UserSettings
        {
            Id = userId
        };
        _userOptionsSet.Add(newUserDefaultOptions);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<UserSettings?> GetOptionsForUser(Guid userId)
    {
        return await _userOptionsSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId);
    }

    public async Task<DateTime> GetFetchPaymentsTillForUser(Guid userId)
    {
        var options = await GetOptionsForUser(userId) 
                      ?? throw new IdNotFoundInDatabase($"User options not found for user {userId}");
        return DateTime.UtcNow.Subtract(options.FetchPaymentsTill);
    }

    public async Task UpdateUserOptions(Guid userId, Action<UserSettings> settingsUpdates)
    {
        var optionsFromDb = await _userOptionsSet
            .FirstOrDefaultAsync(x => x.Id == userId);
        if (optionsFromDb == null)
        {
            throw new IdNotFoundInDatabase(userId.ToString());
        }

        settingsUpdates(optionsFromDb);
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task SetBunqLinked(Guid userId, bool value)
    {
        var optionsFromDb = await _userOptionsSet
            .FirstOrDefaultAsync(x => x.Id == userId);
        if (optionsFromDb == null)
        {
            throw new IdNotFoundInDatabase(userId.ToString());
        }

        optionsFromDb.BunqLinked = value;
        await _dbContext.SaveChangesAsync();
    }
}
﻿using BetterExpenses.Common.Database.Sql;
using BetterExpenses.Common.Models.User;
using Microsoft.EntityFrameworkCore;

namespace BetterExpenses.Common.Services.MonetaryAccounts;

public interface IMonetaryAccountService
{
    public Task UpdateMonetaryAccountsForUser(Guid userId, bool overwrite, IEnumerable<UserMonetaryAccount> newList);
    public Task<List<int>> GetAccountIdsToAnalyse(Guid userId);
    public Task<List<UserMonetaryAccount>> GetAccountsToAnalyse(Guid userId);
    public Task SetAccountsToAnalyse(Guid userId, Dictionary<int, bool> accountsAnalyseStatus);
    public Task<List<UserMonetaryAccount>> GetAccounts(Guid userId);
    public Task UpdateFetchedTill(Dictionary<int, DateTime> accountFetchedTillDict);
}

public class MonetaryAccountService(SqlDbContext dbContext) : IMonetaryAccountService
{
    private readonly DbSet<UserMonetaryAccount> _monetaryAccountsDbSet = dbContext.MonetaryAccounts;


    public async Task UpdateMonetaryAccountsForUser(Guid userId, bool overwrite,
        IEnumerable<UserMonetaryAccount> newList)
    {
        var existing = _monetaryAccountsDbSet.Where(x => x.BetterExpensesUserId == userId);
        if (!await existing.AnyAsync())
        {
            await _monetaryAccountsDbSet.AddRangeAsync(newList);
            await dbContext.SaveChangesAsync();
        }
        if (!overwrite)
        {
            return;
        }
        
        _monetaryAccountsDbSet.RemoveRange(existing);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<int>> GetAccountIdsToAnalyse(Guid userId)
    {
        return (await GetAccountsToAnalyse(userId)).Select(x => x.Id).ToList();
    }

    public async Task<List<UserMonetaryAccount>> GetAccountsToAnalyse(Guid userId)
    {
        return await _monetaryAccountsDbSet
            .Where(x => x.BetterExpensesUserId == userId && x.AnalyseExpenses)
            .ToListAsync();
    }

    public async Task SetAccountsToAnalyse(Guid userId, Dictionary<int, bool> accountsAnalyseStatus)
    {
        var accountIds = accountsAnalyseStatus.Keys.ToList();
        var monetaryAccountsFromDb = await _monetaryAccountsDbSet
            .Where(x => x.BetterExpensesUserId == userId && accountIds.Contains(x.Id))
            .ToListAsync();

        foreach (var account in monetaryAccountsFromDb)
        {
            account.AnalyseExpenses = accountsAnalyseStatus[account.Id];
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task<List<UserMonetaryAccount>> GetAccounts(Guid userId)
    {
        return await _monetaryAccountsDbSet
            .Where(x => x.BetterExpensesUserId == userId)
            .OrderBy(x => x.Id)
            .ToListAsync();
    }

    public async Task UpdateFetchedTill(Dictionary<int, DateTime> accountFetchedTillDict)
    {
        var accountIds = accountFetchedTillDict.Keys.ToList();
        var accountsFromDb = await _monetaryAccountsDbSet.Where(x => accountIds.Contains(x.Id)).ToListAsync();
        foreach (var accountFromDb in accountsFromDb)
        {
            accountFromDb.FetchedTill = accountFetchedTillDict[accountFromDb.Id];
        }

        await dbContext.SaveChangesAsync();
    }
}
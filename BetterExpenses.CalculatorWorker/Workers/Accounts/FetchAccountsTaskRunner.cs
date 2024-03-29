﻿using BetterExpenses.CalculatorWorker.Exceptions;
using BetterExpenses.Common.Models.Tasks;
using BetterExpenses.Common.Models.User;
using BetterExpenses.Common.Services.Bunq;
using BetterExpenses.Common.Services.MonetaryAccounts;
using BetterExpenses.Common.Services.Tasks;
using BetterExpenses.Common.Services.User;
using Bunq.Sdk.Model.Generated.Endpoint;

namespace BetterExpenses.CalculatorWorker.Workers.Accounts;

public interface IFetchAccountsTaskRunner : ITaskRunner<FetchAccountsTask>;

public class FetchAccountsTaskRunner(
    ICalculatorTaskService calculatorTaskService,
    IBunqMonetaryAccountService monetaryAccountApiService,
    IMonetaryAccountService monetaryAccountService,
    IUserOptionsService userOptionsService,
    IBunqPublicAttachmentApiService bunqPublicAttachmentApiService,
    ILogger<FetchAccountsTaskRunner> logger) : IFetchAccountsTaskRunner
{
    private readonly ICalculatorTaskService _calculatorTaskService = calculatorTaskService;
    private readonly IBunqMonetaryAccountService _monetaryAccountApiService = monetaryAccountApiService;
    private readonly IMonetaryAccountService _monetaryAccountService = monetaryAccountService;
    private readonly IUserOptionsService _userOptionsService = userOptionsService;
    private readonly IBunqPublicAttachmentApiService _bunqPublicAttachment = bunqPublicAttachmentApiService;

    public async Task<bool> RunCycle(FetchAccountsTask task)
    {
        logger.LogDebug("Fetching accounts for user {UserId}", task.UserId);
        
        var accounts = await GetAllAccounts(task);
        await _monetaryAccountService.UpdateMonetaryAccountsForUser(task.UserId, task.Overwrite, accounts);

        var userOptions = await _userOptionsService.GetOptionsForUser(task.UserId);
        if (userOptions == null)
        {
            throw new WorkerException($"User with id {task.UserId} does not exist");
        }
        
        logger.LogDebug("Fetching accounts completed");
        return accounts.Count != 0;
    }

    private async Task<List<UserMonetaryAccount>> GetAllAccounts(CalculatorTask task)
    {
        var userId = task.UserId;
        var usableAccounts = (await _monetaryAccountApiService.ListMonetaryAccountsAsync(userId))
            .Where(x => x.MonetaryAccountBank != null || x.MonetaryAccountJoint != null);
        
        var accountBanks = new List<UserMonetaryAccount>();
        foreach (var acc in usableAccounts)
        {
            var avatar = GetAvatarImageUrl(userId, acc);
            accountBanks.Add(UserMonetaryAccount.FromMonetaryAccount(acc, userId, avatar));
        }
        
        logger.LogDebug("{AccountCount} accounts fetched", accountBanks.Count);
        
        return accountBanks;
    }

    private string GetAvatarImageUrl(Guid userId, MonetaryAccount account)
    {
        Avatar avatar;
        if (account.MonetaryAccountBank != null)
        {
            avatar = account.MonetaryAccountBank.Avatar;
        }
        else if (account.MonetaryAccountJoint != null)
        {
            avatar = account.MonetaryAccountJoint.Avatar;
        }
        else
        {
            return "";
        }

        var publicUuid = avatar.Image.First().AttachmentPublicUuid;
        return _bunqPublicAttachment.GetPublicAttachmentUrl(userId, publicUuid);
    }
}
using System.Data;
using Bunq.Sdk.Model.Generated.Endpoint;
using Microsoft.EntityFrameworkCore;

namespace BetterExpenses.Common.Models.User;

[PrimaryKey(nameof(Id), nameof(BetterExpensesUserId))]
public class UserMonetaryAccount
{
    public int Id { get; set; }
    
    public Guid BetterExpensesUserId { get; set; }

    public string Name { get; set; }
    
    public bool JointAccount { get; set; }

    public bool AnalyseExpenses { get; set; }

    public static UserMonetaryAccount FromMonetaryAccount(MonetaryAccount ma, Guid userId)
    {
        if (ma.MonetaryAccountBank != null)
        {
            return FromMonetaryAccountBank(ma.MonetaryAccountBank, userId);
        }

        if (ma.MonetaryAccountJoint != null)
        {
            return FromMonetaryAccountJoint(ma.MonetaryAccountJoint, userId);
        }

        throw new NotImplementedException("Type of account not implemented");
    }

    private static UserMonetaryAccount FromMonetaryAccountBank(MonetaryAccountBank mab, Guid userId)
    {
        return new UserMonetaryAccount
        {
            Id = mab.Id ?? throw new DataException($"The id of monetary account {mab.Description} is null"),
            Name = mab.Description,
            BetterExpensesUserId = userId,
            JointAccount = false
        };
    }
    
    private static UserMonetaryAccount FromMonetaryAccountJoint(MonetaryAccountJoint maj, Guid userId)
    {
        return new UserMonetaryAccount
        {
            Id = maj.Id ?? throw new DataException($"The id of monetary account {maj.Description} is null"),
            Name = maj.Description,
            BetterExpensesUserId = userId,
            JointAccount = true
        };
    }
}
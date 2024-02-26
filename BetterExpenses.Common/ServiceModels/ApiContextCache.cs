using Bunq.Sdk.Context;

namespace BetterExpenses.Common.ServiceModels;

public class ApiContextCache : Dictionary<Guid, ApiContext>
{
}
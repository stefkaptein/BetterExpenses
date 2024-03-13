using Microsoft.Extensions.DependencyInjection;

namespace BetterExpenses.Common.Extensions;

public static class ServiceProviderExtensions
{
    public static List<T> GetAllRequiredServicesImplementingType<T>(this IServiceProvider serviceProvider)
    {
        return AppDomain
            .CurrentDomain
            .GetAllImplementingTypesInDomain<T>()
            .Select(serviceProvider.GetRequiredService)
            .Cast<T>()
            .ToList();
    }

    public static List<T> GetRequiredServicesImplementingTypeInOwnScope<T>(
        this IServiceScopeFactory serviceScopeFactory)
    {
        return AppDomain
            .CurrentDomain
            .GetAllImplementingTypesInDomain<T>()
            .Select(x => serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService(x))
            .Cast<T>()
            .ToList();
    }
}
namespace BetterExpenses.Common.Extensions;

public static class AppDomainExtensions
{
    public static List<Type> GetAllImplementingTypesInDomain<T>(this AppDomain appDomain)
    {
        return appDomain.GetAllImplementingTypesInDomain(typeof(T));
    }
    
    public static List<Type> GetAllImplementingTypesInDomain(this AppDomain appDomain, Type assignableType)
    {
        return appDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => assignableType.IsAssignableFrom(type) && type.IsClass)
            .ToList();
    }
}
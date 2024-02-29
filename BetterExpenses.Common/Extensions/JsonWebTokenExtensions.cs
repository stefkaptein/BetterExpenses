using System.Security.Claims;

namespace BetterExpenses.Common.Extensions;

public static class JsonWebTokenExtensions
{
    public static DateTime GetTokenExpiration(this IEnumerable<Claim> claims)
    {
        var expires = Convert.ToInt64(claims.First(x => x.Type == "exp").Value);
        return DateTime.FromFileTimeUtc(expires);
    }

    public static Guid? GetUserId(this IEnumerable<Claim> claims)
    {
        var userIdClaim = claims.First(x => x.Type == ClaimTypes.NameIdentifier);
        return Guid.Parse(userIdClaim.Value);
    }
}
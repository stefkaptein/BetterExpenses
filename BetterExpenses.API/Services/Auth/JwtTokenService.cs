using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BetterExpenses.API.Services.Options;
using BetterExpenses.Common.Models.User;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BetterExpenses.API.Services.Auth;

public record TokenWithExperation(string Token, DateTime Expires);

public interface IJwtTokenService
{
    public TokenWithExperation GenerateToken(BetterExpensesUser user);
    public Task<TokenWithExperation> GenerateRefreshToken(BetterExpensesUser user);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly TimeSpan _tokenExpirationTimeSpan = TimeSpan.FromSeconds(30);
    private readonly TimeSpan _refreshTokenExpirationTimeSpan = TimeSpan.FromDays(7);
    private readonly string _issuer;
    private readonly SigningCredentials _signingCredentials;
    
    public JwtTokenService(IOptions<JwtOptions> config)
    {
        _issuer = config.Value.Issuer;
        var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(config.Value.Secret));
        _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }

    public TokenWithExperation GenerateToken(BetterExpensesUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var expires = DateTime.UtcNow.Add(_tokenExpirationTimeSpan);
        
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _issuer,
            claims: claims, 
            expires: expires, 
            signingCredentials: _signingCredentials);

        return new TokenWithExperation(new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    public async Task<TokenWithExperation> GenerateRefreshToken(BetterExpensesUser user)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expires = DateTime.UtcNow.Add(_refreshTokenExpirationTimeSpan);
        var refreshToken = new TokenWithExperation(token, expires);

        return refreshToken;
    }
}
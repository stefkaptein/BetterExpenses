using System.Data.SqlTypes;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using BetterExpenses.API.Services.Options;
using BetterExpenses.Common.Database.Sql;
using BetterExpenses.Common.DTO.Auth;
using BetterExpenses.Common.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BetterExpenses.API.Services.Auth;

public interface IJwtTokenService
{
    public string GenerateToken(BetterExpensesUser user);
    public Task<bool> ValidateRefreshToken(Guid userId, string refreshToken);
    public Task InvalidateRefreshToken(string token);
    public Task InvalidateRefreshTokensForUser(Guid userId);

    public Task<TokenWithExperation> GenerateRefreshToken(BetterExpensesUser user);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly SqlDbContext _dbContext;
    private readonly DbSet<RefreshToken> _refreshTokens;

    private readonly TimeSpan _tokenExpirationTimeSpan = TimeSpan.FromSeconds(30);
    private readonly TimeSpan _refreshTokenExpirationTimeSpan = TimeSpan.FromDays(7);
    private readonly string _issuer;
    private readonly SigningCredentials _signingCredentials;

    public JwtTokenService(IOptions<JwtOptions> config, SqlDbContext dbContext)
    {
        _dbContext = dbContext;
        _refreshTokens = dbContext.RefreshTokens;
        _issuer = config.Value.Issuer;

        var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(config.Value.Secret));
        _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }

    public string GenerateToken(BetterExpensesUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? "Unknown User"),
        };

        var expires = DateTime.UtcNow.Add(_tokenExpirationTimeSpan);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _issuer,
            claims: claims,
            expires: expires,
            signingCredentials: _signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> ValidateRefreshToken(Guid userId, string refreshToken)
    {
        var currentTimeUtc = DateTime.UtcNow;
        var token = await _refreshTokens.FirstOrDefaultAsync(x =>
            x.UserId == userId && x.Token == refreshToken && x.Expires <= currentTimeUtc);

        if (token == null)
        {
            return false;
        }

        if (!token.Valid)
        {
            await InvalidateRefreshTokensForUser(userId);
            return false;
        }

        return true;
    }

    public async Task InvalidateRefreshToken(string token)
    {
        var refreshToken = await _refreshTokens.FirstAsync(x => x.Token == token);
        refreshToken.Valid = false;
        await _dbContext.SaveChangesAsync();
    }

    public async Task InvalidateRefreshTokensForUser(Guid userId)
    {
        var validRefreshTokens = await _refreshTokens
            .Where(x => x.UserId == userId && x.Valid)
            .ToListAsync();

        foreach (var token in validRefreshTokens)
        {
            token.Valid = false;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<TokenWithExperation> GenerateRefreshToken(BetterExpensesUser user)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expires = DateTime.UtcNow.Add(_refreshTokenExpirationTimeSpan);
        var refreshToken = new TokenWithExperation(token, expires);

        _refreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken.Token,
            Expires = refreshToken.Expires,
            Valid = true
        });
        await _dbContext.SaveChangesAsync();

        return refreshToken;
    }
}
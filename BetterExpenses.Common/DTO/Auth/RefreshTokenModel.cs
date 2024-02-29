namespace BetterExpenses.Common.DTO.Auth;

public class RefreshTokenModel
{
    public required Guid UserId { get; set; }
    public required string RefreshToken { get; set; }
}
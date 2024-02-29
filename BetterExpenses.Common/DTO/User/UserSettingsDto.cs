namespace BetterExpenses.Common.DTO.User;

public class UserSettingsDto
{
    public bool BunqLinked { get; set; }
    public TimeSpan FetchPaymentsFrom { get; set; }
}
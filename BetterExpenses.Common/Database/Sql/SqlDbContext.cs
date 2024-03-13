using BetterExpenses.Common.Models.Tasks;
using BetterExpenses.Common.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BetterExpenses.Common.Database.Sql;

public class SqlDbContext : IdentityDbContext<BetterExpensesUser, IdentityRole<Guid>, Guid>
{
    public DbSet<UserMonetaryAccount> MonetaryAccounts { get; set; }
    public DbSet<UserSettings> UserOptions { get; set; }
    public DbSet<FetchAccountsTask> FetchAccountsTasks { get; set; }
    public DbSet<FetchExpensesTask> FetchExpensesTasks { get; set; }
    public DbSet<ProcessExpensesTask> ProcessExpensesTasks { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            optionsBuilder
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        RenameDefaultTables(modelBuilder);
        ConfigureUserSettingRelation(modelBuilder);
        ConfigureCalculatorTaskTable(modelBuilder);
    }

    private static void ConfigureCalculatorTaskTable(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CalculatorTask>()
            .HasDiscriminator<string>("TaskType")
            .HasValue<FetchExpensesTask>("fetch_expenses")
            .HasValue<FetchAccountsTask>("fetch_accounts")
            .HasValue<ProcessExpensesTask>("process_expenses");
    }

    private static void RenameDefaultTables(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName?.StartsWith("AspNet") ?? false)
            {
                entityType.SetTableName(tableName[6..]);
            }
        }
    }

    private static void ConfigureUserSettingRelation(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BetterExpensesUser>()
            .HasOne(e => e.UserSettings)
            .WithOne(e => e.User)
            .HasForeignKey<UserSettings>();
    }
}
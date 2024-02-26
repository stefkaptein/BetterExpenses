﻿using BetterExpenses.Common.Models.Tasks;
using BetterExpenses.Common.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserOptions = BetterExpenses.Common.Models.User.UserOptions;

namespace BetterExpenses.Common.Database.Sql;

public class SqlDbContext : IdentityDbContext<BetterExpensesUser, IdentityRole<Guid>, Guid>
{
    public DbSet<UserMonetaryAccount> MonetaryAccounts { get; set; }
    public DbSet<UserOptions> UserOptions { get; set; }
    public DbSet<FetchAccountsTask> FetchAccountsTasks { get; set; }
    public DbSet<FetchExpensesTask> FetchExpensesTasks { get; set; }
    
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

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName?.StartsWith("AspNet") ?? false)
            {
                entityType.SetTableName(tableName[6..]);
            }
        }
    }
}
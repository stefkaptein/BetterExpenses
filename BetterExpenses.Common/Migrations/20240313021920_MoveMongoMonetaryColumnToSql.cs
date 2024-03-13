using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetterExpenses.Common.Migrations
{
    /// <inheritdoc />
    public partial class MoveMongoMonetaryColumnToSql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FetchedTill",
                table: "MonetaryAccounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastFetched",
                table: "MonetaryAccounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FetchedTill",
                table: "MonetaryAccounts");

            migrationBuilder.DropColumn(
                name: "LastFetched",
                table: "MonetaryAccounts");
        }
    }
}

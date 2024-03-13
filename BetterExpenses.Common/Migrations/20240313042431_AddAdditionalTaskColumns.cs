using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetterExpenses.Common.Migrations
{
    /// <inheritdoc />
    public partial class AddAdditionalTaskColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FinishDate",
                table: "CalculatorTask",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Result",
                table: "CalculatorTask",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinishDate",
                table: "CalculatorTask");

            migrationBuilder.DropColumn(
                name: "Result",
                table: "CalculatorTask");
        }
    }
}

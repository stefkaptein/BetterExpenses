using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetterExpenses.Common.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessExpensesTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessExpensesTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessExpensesTasks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessExpensesTasks_CreationDate",
                table: "ProcessExpensesTasks",
                column: "CreationDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessExpensesTasks_Id",
                table: "ProcessExpensesTasks",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessExpensesTasks");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetterExpenses.Common.Migrations
{
    /// <inheritdoc />
    public partial class ReconfigureTaskTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FetchAccountsTasks");

            migrationBuilder.DropTable(
                name: "FetchExpensesTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessExpensesTasks",
                table: "ProcessExpensesTasks");

            migrationBuilder.RenameTable(
                name: "ProcessExpensesTasks",
                newName: "CalculatorTask");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessExpensesTasks_Id",
                table: "CalculatorTask",
                newName: "IX_CalculatorTask_Id");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessExpensesTasks_CreationDate",
                table: "CalculatorTask",
                newName: "IX_CalculatorTask_CreationDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "FetchTill",
                table: "CalculatorTask",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Overwrite",
                table: "CalculatorTask",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaskType",
                table: "CalculatorTask",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CalculatorTask",
                table: "CalculatorTask",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CalculatorTask",
                table: "CalculatorTask");

            migrationBuilder.DropColumn(
                name: "FetchTill",
                table: "CalculatorTask");

            migrationBuilder.DropColumn(
                name: "Overwrite",
                table: "CalculatorTask");

            migrationBuilder.DropColumn(
                name: "TaskType",
                table: "CalculatorTask");

            migrationBuilder.RenameTable(
                name: "CalculatorTask",
                newName: "ProcessExpensesTasks");

            migrationBuilder.RenameIndex(
                name: "IX_CalculatorTask_Id",
                table: "ProcessExpensesTasks",
                newName: "IX_ProcessExpensesTasks_Id");

            migrationBuilder.RenameIndex(
                name: "IX_CalculatorTask_CreationDate",
                table: "ProcessExpensesTasks",
                newName: "IX_ProcessExpensesTasks_CreationDate");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessExpensesTasks",
                table: "ProcessExpensesTasks",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FetchAccountsTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Overwrite = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FetchAccountsTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FetchExpensesTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FetchTill = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FetchExpensesTasks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FetchAccountsTasks_CreationDate",
                table: "FetchAccountsTasks",
                column: "CreationDate");

            migrationBuilder.CreateIndex(
                name: "IX_FetchAccountsTasks_Id",
                table: "FetchAccountsTasks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FetchExpensesTasks_CreationDate",
                table: "FetchExpensesTasks",
                column: "CreationDate");

            migrationBuilder.CreateIndex(
                name: "IX_FetchExpensesTasks_Id",
                table: "FetchExpensesTasks",
                column: "Id");
        }
    }
}

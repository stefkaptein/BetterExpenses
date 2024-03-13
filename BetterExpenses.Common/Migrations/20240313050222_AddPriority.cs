using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetterExpenses.Common.Migrations
{
    /// <inheritdoc />
    public partial class AddPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CalculatorTask_CreationDate",
                table: "CalculatorTask");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "CalculatorTask",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CalculatorTask_CreationDate_Priority",
                table: "CalculatorTask",
                columns: new[] { "CreationDate", "Priority" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CalculatorTask_CreationDate_Priority",
                table: "CalculatorTask");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "CalculatorTask");

            migrationBuilder.CreateIndex(
                name: "IX_CalculatorTask_CreationDate",
                table: "CalculatorTask",
                column: "CreationDate");
        }
    }
}

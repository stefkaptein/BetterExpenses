using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetterExpenses.Common.Migrations
{
    /// <inheritdoc />
    public partial class Rename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Result",
                table: "CalculatorTask",
                newName: "Status");

            migrationBuilder.RenameIndex(
                name: "IX_CalculatorTask_Result",
                table: "CalculatorTask",
                newName: "IX_CalculatorTask_Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "CalculatorTask",
                newName: "Result");

            migrationBuilder.RenameIndex(
                name: "IX_CalculatorTask_Status",
                table: "CalculatorTask",
                newName: "IX_CalculatorTask_Result");
        }
    }
}

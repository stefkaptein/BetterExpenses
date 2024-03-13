using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetterExpenses.Common.Migrations
{
    /// <inheritdoc />
    public partial class AddIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CalculatorTask_Result",
                table: "CalculatorTask",
                column: "Result");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CalculatorTask_Result",
                table: "CalculatorTask");
        }
    }
}

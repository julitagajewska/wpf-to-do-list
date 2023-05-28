using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoListApp.Migrations
{
    /// <inheritdoc />
    public partial class NewPlanner10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_PlannerId",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PlannerId",
                table: "Users",
                column: "PlannerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_PlannerId",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PlannerId",
                table: "Users",
                column: "PlannerId");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoListApp.Migrations
{
    /// <inheritdoc />
    public partial class PlannerDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CurrentDate",
                table: "Planners",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannerDate",
                table: "MainTasks",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentDate",
                table: "Planners");

            migrationBuilder.DropColumn(
                name: "PlannerDate",
                table: "MainTasks");
        }
    }
}

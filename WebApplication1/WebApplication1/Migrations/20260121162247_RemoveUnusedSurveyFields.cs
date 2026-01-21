using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedSurveyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowAnonymous",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Surveys");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowAnonymous",
                table: "Surveys",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Surveys",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Surveys",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

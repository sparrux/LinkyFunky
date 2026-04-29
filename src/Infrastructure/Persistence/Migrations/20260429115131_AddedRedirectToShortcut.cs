using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkyFunky.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedRedirectToShortcut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Redirects",
                table: "shortcuts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Redirects",
                table: "shortcuts");
        }
    }
}

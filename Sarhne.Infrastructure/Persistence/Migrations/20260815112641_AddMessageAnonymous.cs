using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sarhne.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageAnonymous : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAnonymous",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAnonymous",
                table: "Messages");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sarhne.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateFollowingToBeAuditableOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Followings");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Followings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Followings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Followings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedById",
                table: "Followings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Followings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

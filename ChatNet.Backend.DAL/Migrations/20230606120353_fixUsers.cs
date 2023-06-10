using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatNet.Backend.DAL.Migrations
{
    /// <inheritdoc />
    public partial class fixUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserBackendId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserBackendId",
                table: "Users",
                column: "UserBackendId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_UserBackendId",
                table: "Users",
                column: "UserBackendId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_UserBackendId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserBackendId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserBackendId",
                table: "Users");
        }
    }
}

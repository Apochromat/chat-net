using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatNet.Call.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddConnectionInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCallerConnected",
                table: "Calls",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReceiverConnected",
                table: "Calls",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCallerConnected",
                table: "Calls");

            migrationBuilder.DropColumn(
                name: "IsReceiverConnected",
                table: "Calls");
        }
    }
}

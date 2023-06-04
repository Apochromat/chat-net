using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatNet.FileStorage.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPrivateFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Files",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Files");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prism.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailToLoginTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Logins",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Logins_Email",
                table: "Logins",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Logins_Email",
                table: "Logins");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Logins");
        }
    }
}

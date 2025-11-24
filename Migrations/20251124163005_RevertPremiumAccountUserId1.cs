using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GearShop.Migrations
{
    /// <inheritdoc />
    public partial class RevertPremiumAccountUserId1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PremiumAccounts_Users_UserId1",
                table: "PremiumAccounts");

            migrationBuilder.DropIndex(
                name: "IX_PremiumAccounts_UserId1",
                table: "PremiumAccounts");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "PremiumAccounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "PremiumAccounts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PremiumAccounts_UserId1",
                table: "PremiumAccounts",
                column: "UserId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PremiumAccounts_Users_UserId1",
                table: "PremiumAccounts",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}

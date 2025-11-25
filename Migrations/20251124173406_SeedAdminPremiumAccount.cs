using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GearShop.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminPremiumAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert a premium account for user with Id = 1 if it doesn't already exist
            migrationBuilder.Sql(@"INSERT INTO `PremiumAccounts` (`CreatedAt`, `EndDate`, `MonthlyAmount`, `NextPaymentDate`, `Notes`, `StartDate`, `Status`, `UpdatedAt`, `UserId`)
    SELECT '2025-11-24 00:00:00', '2026-11-24 00:00:00', 0.00, NULL, NULL, '2025-11-24 00:00:00', 1, NULL, 1
    FROM DUAL
    WHERE NOT EXISTS (SELECT 1 FROM `PremiumAccounts` WHERE `UserId` = 1);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the seeded premium account for user 1
            migrationBuilder.Sql("DELETE FROM `PremiumAccounts` WHERE `UserId` = 1;");
        }
    }
}

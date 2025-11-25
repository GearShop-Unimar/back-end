using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GearShop.Migrations
{
    /// <inheritdoc />
    public partial class MergeSubscriptionIntoPremium : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Subscriptions_SubscriptionId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PremiumAccounts");

            migrationBuilder.RenameColumn(
                name: "SubscriptionId",
                table: "Payments",
                newName: "PremiumAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_SubscriptionId",
                table: "Payments",
                newName: "IX_Payments_PremiumAccountId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "PremiumAccounts",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PremiumAccounts",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyAmount",
                table: "PremiumAccounts",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextPaymentDate",
                table: "PremiumAccounts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "PremiumAccounts",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "PremiumAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "PremiumAccounts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PremiumAccounts_PremiumAccountId",
                table: "Payments",
                column: "PremiumAccountId",
                principalTable: "PremiumAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PremiumAccounts_PremiumAccountId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PremiumAccounts");

            migrationBuilder.DropColumn(
                name: "MonthlyAmount",
                table: "PremiumAccounts");

            migrationBuilder.DropColumn(
                name: "NextPaymentDate",
                table: "PremiumAccounts");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "PremiumAccounts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PremiumAccounts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PremiumAccounts");

            migrationBuilder.RenameColumn(
                name: "PremiumAccountId",
                table: "Payments",
                newName: "SubscriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_PremiumAccountId",
                table: "Payments",
                newName: "IX_Payments_SubscriptionId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "PremiumAccounts",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PremiumAccounts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    MonthlyAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    NextPaymentDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_NextPaymentDate",
                table: "Subscriptions",
                column: "NextPaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ProductId",
                table: "Subscriptions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId_ProductId",
                table: "Subscriptions",
                columns: new[] { "UserId", "ProductId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Subscriptions_SubscriptionId",
                table: "Payments",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

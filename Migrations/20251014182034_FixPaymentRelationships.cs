using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GearShop.Migrations
{
    /// <inheritdoc />
    public partial class FixPaymentRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Subscriptions_OrderId",
                table: "Payments");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Payments",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionId",
                table: "Payments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SubscriptionId",
                table: "Payments",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Subscriptions_SubscriptionId",
                table: "Payments",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Subscriptions_SubscriptionId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_SubscriptionId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "Payments");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Payments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Subscriptions_OrderId",
                table: "Payments",
                column: "OrderId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

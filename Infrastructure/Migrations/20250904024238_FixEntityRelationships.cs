using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixEntityRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Quotes_QuoteRequestId_UserId",
                table: "Quotes");

            migrationBuilder.AddColumn<Guid>(
                name: "DistributorId",
                table: "Quotes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_DistributorId",
                table: "Quotes",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_QuoteRequestId_DistributorId",
                table: "Quotes",
                columns: new[] { "QuoteRequestId", "DistributorId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Users_UserId",
                table: "Carts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Distributors",
                table: "Quotes",
                column: "DistributorId",
                principalTable: "Distributors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Users",
                table: "Quotes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Users_UserId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Distributors",
                table: "Quotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Users",
                table: "Quotes");

            migrationBuilder.DropIndex(
                name: "IX_Quotes_DistributorId",
                table: "Quotes");

            migrationBuilder.DropIndex(
                name: "IX_Quotes_QuoteRequestId_DistributorId",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "DistributorId",
                table: "Quotes");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_QuoteRequestId_UserId",
                table: "Quotes",
                columns: new[] { "QuoteRequestId", "UserId" });
        }
    }
}

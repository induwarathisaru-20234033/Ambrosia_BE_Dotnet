using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMB.Infra.Migrations
{
    /// <inheritdoc />
    public partial class LinkReservationToOrderAndTableQr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "QrIdentifier",
                table: "Tables",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "ReservationId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ReservationId",
                table: "Orders",
                column: "ReservationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Reservations_ReservationId",
                table: "Orders",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Reservations_ReservationId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ReservationId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "QrIdentifier",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "ReservationId",
                table: "Orders");
        }
    }
}

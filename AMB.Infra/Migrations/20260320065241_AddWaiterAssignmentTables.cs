using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMB.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddWaiterAssignmentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedWaiterId",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnline",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ReservationWaiterAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    WaiterId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UnassignedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationWaiterAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationWaiterAssignments_Employees_WaiterId",
                        column: x => x.WaiterId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationWaiterAssignments_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_AssignedWaiterId",
                table: "Reservations",
                column: "AssignedWaiterId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationWaiterAssignments_ReservationId",
                table: "ReservationWaiterAssignments",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationWaiterAssignments_WaiterId",
                table: "ReservationWaiterAssignments",
                column: "WaiterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Employees_AssignedWaiterId",
                table: "Reservations",
                column: "AssignedWaiterId",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Employees_AssignedWaiterId",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "ReservationWaiterAssignments");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_AssignedWaiterId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "AssignedWaiterId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "IsOnline",
                table: "Employees");
        }
    }
}

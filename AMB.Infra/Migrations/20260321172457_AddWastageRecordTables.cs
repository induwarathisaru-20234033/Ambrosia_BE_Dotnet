using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMB.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddWastageRecordTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WastageRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WastageEntryNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntryDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RecordedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneralNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_WastageRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WastageEntryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemNo = table.Column<int>(type: "int", nullable: false),
                    WastageType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<float>(type: "real", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InventoryItemId = table.Column<int>(type: "int", nullable: false),
                    WastageRecordId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_WastageEntryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WastageEntryItems_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WastageEntryItems_WastageRecords_WastageRecordId",
                        column: x => x.WastageRecordId,
                        principalTable: "WastageRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WastageEntryItems_InventoryItemId",
                table: "WastageEntryItems",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WastageEntryItems_WastageRecordId",
                table: "WastageEntryItems",
                column: "WastageRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WastageEntryItems");

            migrationBuilder.DropTable(
                name: "WastageRecords");
        }
    }
}

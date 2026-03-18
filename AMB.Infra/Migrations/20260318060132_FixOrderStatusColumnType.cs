using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMB.Infra.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderStatusColumnType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the index that depends on OrderStatus
            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderStatus",
                table: "Orders");

            // First, add a temporary integer column
            migrationBuilder.AddColumn<int>(
                name: "OrderStatusInt",
                table: "Orders",
                type: "int",
                nullable: true);

            // Convert existing string values to integers
            migrationBuilder.Sql(@"
                UPDATE [Orders] 
                SET [OrderStatusInt] = 
                    CASE TRIM([OrderStatus])
                        WHEN 'Draft' THEN 1
                        WHEN 'Sent to KDS' THEN 2
                        WHEN 'Preparing' THEN 3
                        WHEN 'On Hold' THEN 4
                        WHEN 'Ready' THEN 5
                        WHEN 'Served' THEN 6
                        WHEN 'Cancelled' THEN 7
                        ELSE 1
                    END
                WHERE [OrderStatus] IS NOT NULL
            ");

            // Drop the old string column (now that index is gone)
            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "Orders");

            // Rename the temporary column to OrderStatus
            migrationBuilder.RenameColumn(
                name: "OrderStatusInt",
                table: "Orders",
                newName: "OrderStatus");

            // Make it non-nullable with default
            migrationBuilder.AlterColumn<int>(
                name: "OrderStatus",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 1);

            // Recreate the index on the new integer column
            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderStatus",
                table: "Orders",
                column: "OrderStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the index first
            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderStatus",
                table: "Orders");

            // Add back string column
            migrationBuilder.AddColumn<string>(
                name: "OrderStatusString",
                table: "Orders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            // Convert integers back to strings
            migrationBuilder.Sql(@"
                UPDATE [Orders] 
                SET [OrderStatusString] = 
                    CASE [OrderStatus]
                        WHEN 1 THEN 'Draft'
                        WHEN 2 THEN 'Sent to KDS'
                        WHEN 3 THEN 'Preparing'
                        WHEN 4 THEN 'On Hold'
                        WHEN 5 THEN 'Ready'
                        WHEN 6 THEN 'Served'
                        WHEN 7 THEN 'Cancelled'
                        ELSE 'Draft'
                    END
            ");

            // Drop the integer column
            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "Orders");

            // Rename string column to OrderStatus
            migrationBuilder.RenameColumn(
                name: "OrderStatusString",
                table: "Orders",
                newName: "OrderStatus");

            // Make it non-nullable with default
            migrationBuilder.AlterColumn<string>(
                name: "OrderStatus",
                table: "Orders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Draft");

            // Recreate the index
            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderStatus",
                table: "Orders",
                column: "OrderStatus");
        }
    }
}
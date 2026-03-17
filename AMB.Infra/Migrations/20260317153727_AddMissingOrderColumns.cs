using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMB.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingOrderColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add ItemStatus column to OrderItems if it doesn't exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'OrderItems')
                AND NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'OrderItems' AND COLUMN_NAME = 'ItemStatus')
                BEGIN
                    ALTER TABLE [OrderItems] ADD [ItemStatus] int NULL;
                END
            ");

            // Ensure OrderNumber column has unique index
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Orders')
                AND NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_OrderNumber' AND object_id = OBJECT_ID('Orders'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_Orders_OrderNumber] ON [Orders] ([OrderNumber]);
                END
            ");

            // Ensure all columns have default values if needed
            migrationBuilder.Sql(@"
                -- Set default for Status if not set
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Orders')
                BEGIN
                    DECLARE @defaultExists INT;
                    SELECT @defaultExists = COUNT(*) FROM sys.default_constraints 
                    WHERE parent_object_id = OBJECT_ID('Orders') AND parent_column_id = COLUMNPROPERTY(OBJECT_ID('Orders'), 'Status', 'ColumnId');
                    
                    IF @defaultExists = 0
                    BEGIN
                        ALTER TABLE [Orders] ADD CONSTRAINT [DF_Orders_Status] DEFAULT 1 FOR [Status];
                    END
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No down operation - this is a safe additive migration
        }
    }
}
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMB.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderTables_Final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if Orders table exists and create if not
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Orders')
                BEGIN
                    CREATE TABLE [Orders] (
                        [Id] int NOT NULL IDENTITY(1,1),
                        [OrderNumber] nvarchar(20) NOT NULL,
                        [TableId] int NULL,
                        [OrderStatus] int NOT NULL,
                        [SentToKitchenAt] datetimeoffset NULL,
                        [Status] int NOT NULL,
                        [CreatedDate] datetimeoffset NOT NULL,
                        [UpdatedDate] datetimeoffset NULL,
                        [DeletedDate] datetimeoffset NULL,
                        [CreatedBy] nvarchar(max) NOT NULL,
                        [UpdatedBy] nvarchar(max) NULL,
                        [DeletedBy] nvarchar(max) NULL,
                        CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Orders_Tables_TableId] FOREIGN KEY ([TableId]) REFERENCES [Tables] ([Id]) ON DELETE SET NULL
                    );

                    CREATE UNIQUE INDEX [IX_Orders_OrderNumber] ON [Orders] ([OrderNumber]);
                    CREATE INDEX [IX_Orders_TableId] ON [Orders] ([TableId]);
                END
            ");

            // Check if OrderItems table exists and create if not
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'OrderItems')
                BEGIN
                    CREATE TABLE [OrderItems] (
                        [Id] int NOT NULL IDENTITY(1,1),
                        [OrderId] int NOT NULL,
                        [MenuItemId] int NOT NULL,
                        [SpecialInstructions] nvarchar(500) NULL,
                        [Quantity] int NOT NULL,
                        [UnitPrice] decimal(18,2) NOT NULL,
                        [ItemStatus] int NULL,
                        [Status] int NOT NULL,
                        [CreatedDate] datetimeoffset NOT NULL,
                        [UpdatedDate] datetimeoffset NULL,
                        [DeletedDate] datetimeoffset NULL,
                        [CreatedBy] nvarchar(max) NOT NULL,
                        [UpdatedBy] nvarchar(max) NULL,
                        [DeletedBy] nvarchar(max) NULL,
                        CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_OrderItems_MenuItems_MenuItemId] FOREIGN KEY ([MenuItemId]) REFERENCES [MenuItems] ([Id]) ON DELETE NO ACTION
                    );

                    CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);
                    CREATE INDEX [IX_OrderItems_MenuItemId] ON [OrderItems] ([MenuItemId]);
                END
            ");

            // Add OrderNumber index if it doesn't exist (in case table existed but index missing)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Orders')
                AND NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_OrderNumber' AND object_id = OBJECT_ID('Orders'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_Orders_OrderNumber] ON [Orders] ([OrderNumber]);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Only drop if they exist (safe cleanup)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'OrderItems')
                BEGIN
                    DROP TABLE [OrderItems];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Orders')
                BEGIN
                    DROP TABLE [Orders];
                END
            ");
        }
    }
}
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
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Tables' AND COLUMN_NAME='QrIdentifier')
                BEGIN
                    ALTER TABLE [Tables] ADD [QrIdentifier] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Orders' AND COLUMN_NAME='ReservationId')
                BEGIN
                    ALTER TABLE [Orders] ADD [ReservationId] int NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Orders_ReservationId' AND object_id = OBJECT_ID('Orders'))
                BEGIN
                    CREATE INDEX [IX_Orders_ReservationId] ON [Orders] ([ReservationId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Orders_Reservations_ReservationId')
                BEGIN
                    ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Reservations_ReservationId] FOREIGN KEY ([ReservationId]) REFERENCES [Reservations] ([Id]);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Orders_Reservations_ReservationId')
                BEGIN
                    ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_Reservations_ReservationId];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Orders_ReservationId' AND object_id = OBJECT_ID('Orders'))
                BEGIN
                    DROP INDEX [IX_Orders_ReservationId] ON [Orders];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Orders' AND COLUMN_NAME='ReservationId')
                BEGIN
                    ALTER TABLE [Orders] DROP COLUMN [ReservationId];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Tables' AND COLUMN_NAME='QrIdentifier')
                BEGIN
                    ALTER TABLE [Tables] DROP COLUMN [QrIdentifier];
                END
            ");
        }
    }
}

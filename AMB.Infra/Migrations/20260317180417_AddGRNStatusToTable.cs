using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMB.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddGRNStatusToTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GRNStatus",
                table: "GoodReceiptNotes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GRNStatus",
                table: "GoodReceiptNotes");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMB.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintsToEmployeeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "UX_Employees_EmployeeId_Active",
                table: "Employees",
                column: "EmployeeId",
                unique: true,
                filter: "[Status] = 1");

            migrationBuilder.CreateIndex(
                name: "UX_Employees_MobileNumber_Active",
                table: "Employees",
                column: "MobileNumber",
                unique: true,
                filter: "[Status] = 1");

            migrationBuilder.CreateIndex(
                name: "UX_Employees_UserId_Active",
                table: "Employees",
                column: "UserId",
                unique: true,
                filter: "[Status] = 1");

            migrationBuilder.CreateIndex(
                name: "UX_Employees_Username_Active",
                table: "Employees",
                column: "Username",
                unique: true,
                filter: "[Status] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Employees_EmployeeId_Active",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "UX_Employees_MobileNumber_Active",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "UX_Employees_UserId_Active",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "UX_Employees_Username_Active",
                table: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}

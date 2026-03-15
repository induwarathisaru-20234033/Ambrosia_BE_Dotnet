using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMB.Infra.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeRoleMapTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeRoleMaps_Roles_RoleId",
                table: "EmployeeRoleMaps");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "EmployeeRoleMaps",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CustomRoleId",
                table: "EmployeeRoleMaps",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRoleMaps_CustomRoleId",
                table: "EmployeeRoleMaps",
                column: "CustomRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeRoleMaps_CustomRoles_CustomRoleId",
                table: "EmployeeRoleMaps",
                column: "CustomRoleId",
                principalTable: "CustomRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeRoleMaps_Roles_RoleId",
                table: "EmployeeRoleMaps",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeRoleMaps_CustomRoles_CustomRoleId",
                table: "EmployeeRoleMaps");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeRoleMaps_Roles_RoleId",
                table: "EmployeeRoleMaps");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeRoleMaps_CustomRoleId",
                table: "EmployeeRoleMaps");

            migrationBuilder.DropColumn(
                name: "CustomRoleId",
                table: "EmployeeRoleMaps");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "EmployeeRoleMaps",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeRoleMaps_Roles_RoleId",
                table: "EmployeeRoleMaps",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

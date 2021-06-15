using Microsoft.EntityFrameworkCore.Migrations;

namespace backend_staffdirectory.Migrations
{
    public partial class updatedPrivilegesToRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Privileges",
                table: "usersinfo",
                newName: "Role");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "usersinfo",
                newName: "Privileges");
        }
    }
}

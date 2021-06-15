using Microsoft.EntityFrameworkCore.Migrations;

namespace backend_staffdirectory.Migrations
{
    public partial class fixedroleposition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "usersinfo");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "users",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValueSql: "'0'",
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "usersinfo",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValueSql: "'0'",
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}

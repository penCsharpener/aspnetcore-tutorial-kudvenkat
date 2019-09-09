using Microsoft.EntityFrameworkCore.Migrations;

namespace kudvenkat.DataAccess.Migrations {
    public partial class AddEmployeePhotoPathProperty : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Employees",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Employees");
        }
    }
}

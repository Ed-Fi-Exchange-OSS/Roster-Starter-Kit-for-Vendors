using Microsoft.EntityFrameworkCore.Migrations;

namespace EdFi.Roster.Data.Migrations
{
    public partial class AddResourceId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResourceId",
                table: "Students",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResourceId",
                table: "Staff",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResourceId",
                table: "Sections",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResourceId",
                table: "Schools",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResourceId",
                table: "LocalEducationAgencies",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "LocalEducationAgencies");
        }
    }
}

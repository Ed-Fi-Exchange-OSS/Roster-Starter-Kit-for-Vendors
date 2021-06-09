using Microsoft.EntityFrameworkCore.Migrations;

namespace EdFi.Roster.Data.Migrations.ChangeQueryDb
{
    public partial class CreateChangeQueryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChangeQueries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ResourceType = table.Column<string>(type: "TEXT", nullable: true),
                    ChangeVersion = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeQueries", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChangeQueries");
        }
    }
}

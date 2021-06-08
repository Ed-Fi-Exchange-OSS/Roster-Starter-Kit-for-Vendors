using Microsoft.EntityFrameworkCore.Migrations;

namespace EdFi.Roster.Data.Migrations
{
    public partial class RenameRosterTablesToSpecifyComposites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Staff",
                table: "Staff");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sections",
                table: "Sections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Schools",
                table: "Schools");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LocalEducationAgencies",
                table: "LocalEducationAgencies");

            migrationBuilder.RenameTable(
                name: "Students",
                newName: "RosterStudentsComposite");

            migrationBuilder.RenameTable(
                name: "Staff",
                newName: "RosterStaffComposite");

            migrationBuilder.RenameTable(
                name: "Sections",
                newName: "RosterSectionsComposite");

            migrationBuilder.RenameTable(
                name: "Schools",
                newName: "RosterSchoolsComposite");

            migrationBuilder.RenameTable(
                name: "LocalEducationAgencies",
                newName: "RosterLocalEducationAgenciesComposite");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RosterStudentsComposite",
                table: "RosterStudentsComposite",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RosterStaffComposite",
                table: "RosterStaffComposite",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RosterSectionsComposite",
                table: "RosterSectionsComposite",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RosterSchoolsComposite",
                table: "RosterSchoolsComposite",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RosterLocalEducationAgenciesComposite",
                table: "RosterLocalEducationAgenciesComposite",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RosterStudentsComposite",
                table: "RosterStudentsComposite");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RosterStaffComposite",
                table: "RosterStaffComposite");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RosterSectionsComposite",
                table: "RosterSectionsComposite");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RosterSchoolsComposite",
                table: "RosterSchoolsComposite");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RosterLocalEducationAgenciesComposite",
                table: "RosterLocalEducationAgenciesComposite");

            migrationBuilder.RenameTable(
                name: "RosterStudentsComposite",
                newName: "Students");

            migrationBuilder.RenameTable(
                name: "RosterStaffComposite",
                newName: "Staff");

            migrationBuilder.RenameTable(
                name: "RosterSectionsComposite",
                newName: "Sections");

            migrationBuilder.RenameTable(
                name: "RosterSchoolsComposite",
                newName: "Schools");

            migrationBuilder.RenameTable(
                name: "RosterLocalEducationAgenciesComposite",
                newName: "LocalEducationAgencies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                table: "Students",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Staff",
                table: "Staff",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sections",
                table: "Sections",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schools",
                table: "Schools",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocalEducationAgencies",
                table: "LocalEducationAgencies",
                column: "Id");
        }
    }
}

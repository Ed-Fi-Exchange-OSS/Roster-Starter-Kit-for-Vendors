using EdFi.Roster.Models;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Roster.Data
{
    public class ChangeQueryDbContext : BaseDbContext
    {
        public ChangeQueryDbContext(DbContextOptions<ChangeQueryDbContext> options)
            : base(options)
        {

        }

        public DbSet<RosterLocalEducationAgencyResource> RosterLocalEducationAgenciesResource { get; set; }
        public DbSet<RosterSchoolResource> RosterSchoolsResource { get; set; }
        public DbSet<RosterSectionResource> RosterSectionsResource { get; set; }
        public DbSet<RosterStaffResource> RosterStaffResource { get; set; }
        public DbSet<RosterStudentResource> RosterStudentsResource { get; set; }
        public DbSet<ApiSettings> ApiSettings { get; set; }
        public DbSet<ApiLogEntry> ApiLogEntries { get; set; }
        public DbSet<ChangeQuery> ChangeQueries { get; set; }
    }
}

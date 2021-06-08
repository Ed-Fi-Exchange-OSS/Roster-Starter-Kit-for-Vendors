using EdFi.Roster.Models;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Roster.Data
{
    public class RosterDbContext : DbContext
    {
        public RosterDbContext(DbContextOptions<RosterDbContext> options)
            : base(options)
        {

        }

        public DbSet<RosterLocalEducationAgency> RosterLocalEducationAgenciesComposite { get; set; }
        public DbSet<RosterSchool> RosterSchoolsComposite { get; set; }
        public DbSet<RosterSection> RosterSectionsComposite { get; set; }
        public DbSet<RosterStaff> RosterStaffComposite { get; set; }
        public DbSet<RosterStudent> RosterStudentsComposite { get; set; }
        public DbSet<ApiSettings> ApiSettings { get; set; }
        public DbSet<ApiLogEntry> ApiLogEntries { get; set; }
    }
}
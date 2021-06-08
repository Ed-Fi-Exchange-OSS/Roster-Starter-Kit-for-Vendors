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

        public DbSet<RosterLocalEducationAgencyComposite> RosterLocalEducationAgenciesComposite { get; set; }
        public DbSet<RosterSchoolComposite> RosterSchoolsComposite { get; set; }
        public DbSet<RosterSectionComposite> RosterSectionsComposite { get; set; }
        public DbSet<RosterStaffComposite> RosterStaffComposite { get; set; }
        public DbSet<RosterStudentComposite> RosterStudentsComposite { get; set; }
        public DbSet<ApiSettings> ApiSettings { get; set; }
        public DbSet<ApiLogEntry> ApiLogEntries { get; set; }
    }
}
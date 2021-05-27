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

        public DbSet<RosterLocalEducationAgency> LocalEducationAgencies { get; set; }
        public DbSet<RosterSchool> Schools { get; set; }
        public DbSet<RosterSection> Sections { get; set; }
        public DbSet<RosterStaff> Staff { get; set; }
        public DbSet<RosterStudent> Students { get; set; }
        public DbSet<ApiSettings> ApiSettings { get; set; }
        public DbSet<ApiLogEntry> ApiLogEntries { get; set; }
    }
}
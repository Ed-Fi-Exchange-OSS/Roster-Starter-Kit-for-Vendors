using Microsoft.EntityFrameworkCore;

namespace EdFi.Roster.Data
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions options)
            : base(options)
        {

        }
    }
}



using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Controllers.Models;

namespace RaceDayAPI.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(
            DbContextOptions<RaceDayDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<RaceDayAPI.Controllers.Models.Route> Routes { get; set; }
    }
}
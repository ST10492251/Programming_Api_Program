using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Controllers.Models;

namespace RaceDayAPI
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
        public DbSet<Controllers.Models.Route> Routes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Enrolments table
            modelBuilder.Entity<Enrollment>()
                .ToTable("Enrolments");

            modelBuilder.Entity<Enrollment>()
                .HasKey(e => e.EnrolmentID);

            // Prevent an unintended direct Event relationship.
            // Enrolments are linked to Events through EventCategories.
            modelBuilder.Entity<Event>()
                .Ignore(e => e.Enrollments);

            // Enrolment -> Participant
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Participant)
                .WithMany()
                .HasForeignKey(e => e.ParticipantID);

            // Enrolment -> EventCategory
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.EventCategory)
                .WithMany(ec => ec.Enrolments)
                .HasForeignKey(e => e.EventCategoryID);

            // EventCategory -> Event
            modelBuilder.Entity<EventCategory>()
                .HasOne(ec => ec.Event)
                .WithMany()
                .HasForeignKey(ec => ec.EventID);

            // EventCategory -> Category
            modelBuilder.Entity<EventCategory>()
                .HasOne(ec => ec.Category)
                .WithMany(c => c.EventCategories)
                .HasForeignKey(ec => ec.CategoryID);
        }
    }
}

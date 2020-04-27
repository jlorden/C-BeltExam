using Microsoft.EntityFrameworkCore;

namespace BeltExam.Models 
{
    public class MyContext : DbContext 
    {
        public MyContext (DbContextOptions options) : base (options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<DojoActivity> DojoActivities { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
    }
}
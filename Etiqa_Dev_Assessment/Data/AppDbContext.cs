using Etiqa_Dev_Assessment.Model;
using Microsoft.EntityFrameworkCore;

namespace AppContext.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {}

        public DbSet<Employee> Employees { get; set; }

    }
}

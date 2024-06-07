using Microsoft.EntityFrameworkCore;

namespace WorkerService1.Model;

public class JobsDbContext : DbContext
{
    public JobsDbContext(DbContextOptions<JobsDbContext> options) : base(options)
    {
    }

    public DbSet<Job> Jobs { get; set; }
    public DbSet<JobLog> JobLogs { get; set; }
}
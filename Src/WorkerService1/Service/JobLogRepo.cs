using WorkerService1.Model;

namespace WorkerService1.Service;

public class JobLogRepo : IJobLogRepo
{
    private readonly JobsDbContext _context;

    public JobLogRepo(JobsDbContext context)
    {
        _context = context;
    }

    public async Task AddJobLogAsync(JobLog jobLog)
    {
        await _context.JobLogs.AddAsync(jobLog);
        await _context.SaveChangesAsync();
    }
}

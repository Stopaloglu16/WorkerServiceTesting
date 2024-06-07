using Microsoft.EntityFrameworkCore;
using WorkerService1.Model;

namespace WorkerService1.Service;

public class JobRepo : IJobRepo
{
    private readonly JobsDbContext _context;

    public JobRepo(JobsDbContext context)
    {
        _context = context;
    }

    public async Task AddJobAsync(Job job)
    {
        await _context.Jobs.AddAsync(job);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Job>> GetJobsAsync()
    {
        return await _context.Jobs.ToListAsync();
    }
}

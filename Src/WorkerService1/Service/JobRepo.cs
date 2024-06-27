using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WorkerService1.Aggregates;
using WorkerService1.Model;

namespace WorkerService1.Service;

public class JobRepo : IJobRepo
{
    private readonly JobsDbContext _context;
    private readonly IMapper _mapper;

    public JobRepo(JobsDbContext context, IMapper mapper) 
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task AddJobAsync(Job job)
    {
        await _context.Jobs.AddAsync(job);
        await _context.SaveChangesAsync();
    }

    public async Task<List<JobDto>> GetJobsAsync()
    {
        return await _context.Jobs.AsNoTracking()
                                  .ProjectTo<JobDto>(_mapper.ConfigurationProvider)
                                  .ToListAsync();
    }
}

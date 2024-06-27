using WorkerService1.Aggregates;
using WorkerService1.Model;

namespace WorkerService1.Service;

public interface IJobRepo
{
    Task AddJobAsync(Job job);
    Task<List<JobDto>> GetJobsAsync();
}

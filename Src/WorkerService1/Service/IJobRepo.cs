using WorkerService1.Model;

namespace WorkerService1.Service;

public interface IJobRepo
{
    Task AddJobAsync(Job job);
    Task<List<Job>> GetJobsAsync();
}

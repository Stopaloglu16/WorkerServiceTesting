using WorkerService1.Model;

namespace WorkerService1.Service;

public interface IJobLogRepo
{
    Task AddJobLogAsync(JobLog jobLog);
}

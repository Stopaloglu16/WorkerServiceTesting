using WorkerService1.Model;
using WorkerService1.Service;

namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    IJobRepo scopedJobRepoService = scope.ServiceProvider.GetRequiredService<IJobRepo>();

                    var jobLogService = scope.ServiceProvider.GetRequiredService<IJobLogRepo>();

                    var jobs = await scopedJobRepoService.GetJobsAsync();

                    foreach (var job in jobs)
                    {
                        // Process each job (read files, etc.)
                        _logger.LogInformation($"job name {job.Name}");

                        var jobLog = new JobLog
                        {
                            JobName = job.Name,
                            ProcessedAt = DateTime.UtcNow
                        };
                        await jobLogService.AddJobLogAsync(jobLog);
                    }
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using WorkerService1.Aggregates;
using WorkerService1.Model;
using WorkerService1.Service;

namespace WorkerService1.Test
{
    public class WorkerServiceByMockTests
    {
        private readonly Mock<IJobRepo> _jobRepoMock;
        private readonly Mock<IJobLogRepo> _jobLogRepoMock;
        private readonly Mock<ILogger<Worker>> _loggerMock;
        private readonly Worker _workerService;
        private readonly ServiceProvider _serviceProvider;

        public WorkerServiceByMockTests()
        {
            var serviceCollection = new ServiceCollection();
            _jobRepoMock = new Mock<IJobRepo>();
            _jobLogRepoMock = new Mock<IJobLogRepo>();
            _loggerMock = new Mock<ILogger<Worker>>();

            serviceCollection.AddScoped(_ => _jobRepoMock.Object);
            serviceCollection.AddScoped(_ => _jobLogRepoMock.Object);
           

            _serviceProvider = serviceCollection.BuildServiceProvider();

            _workerService = new Worker(_loggerMock.Object, _serviceProvider);
        }

        [Fact]
        public async Task ExecuteAsync_ProcessesJobs()
        {
            // Arrange
            var jobs = new List<JobDto>
            {
                new JobDto { Id = 1, Name = "Job1" },
                new JobDto { Id = 2, Name = "Job2" }
            };

            _jobRepoMock.Setup(repo => repo.GetJobsAsync())
                        .ReturnsAsync(jobs);

            var jobLogs = new List<JobLog>();
            _jobLogRepoMock.Setup(repo => repo.AddJobLogAsync(It.IsAny<JobLog>()))
                           .Callback<JobLog>(log => jobLogs.Add(log))
                           .Returns(Task.CompletedTask);

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(10000); // Cancel after 3 seconds to stop the background service

            // Act
            await _workerService.StartAsync(cancellationTokenSource.Token);

            await Task.Delay(10000);

            // Assert
            // Validate that jobs were processed, depending on your logic
            _jobRepoMock.Verify(repo => repo.GetJobsAsync(), Times.AtLeastOnce);
            foreach (var job in jobs)
            {
                _loggerMock.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Processing job: {job.Name}")),
                        It.IsAny<Exception>(),
                        It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.AtLeastOnce);

                _jobLogRepoMock.Verify(repo => repo.AddJobLogAsync(
                    It.Is<JobLog>(log => log.JobName == job.Name && log.ProcessedAt <= DateTime.UtcNow)),
                    Times.AtLeastOnce);
            }

            Assert.Equal(jobs.Count, jobLogs.Count);
            foreach (var job in jobs)
            {
                Assert.Contains(jobLogs, log => log.JobName == job.Name);
            }
        }

    }
}

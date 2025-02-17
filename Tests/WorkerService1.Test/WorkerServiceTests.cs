﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WorkerService1.Aggregates;
using WorkerService1.Model;
using WorkerService1.Service;

namespace WorkerService1.Test
{
    public class WorkerServiceTests
    {
        private IConfiguration _configuration;
        private readonly ServiceProvider _serviceProvider;
        private readonly Worker _workerService;
        private readonly AppSetting _appSetting;

        public WorkerServiceTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<JobsDbContext>(options =>
                options.UseInMemoryDatabase("JobsTestDb"));

            serviceCollection.AddScoped<IJobRepo, JobRepo>();
            serviceCollection.AddScoped<IJobLogRepo, JobLogRepo>();
            serviceCollection.AddLogging();

            serviceCollection.AddAutoMapper(cfg =>
            {
                cfg.CreateMap<Job, JobDto>();
            });

            _serviceProvider = serviceCollection.BuildServiceProvider();


            _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //.AddUserSecrets("secret-token")
            .AddEnvironmentVariables()
            .Build();


            _appSetting = new AppSetting()
            {
                SubSetting = _configuration["MainSetting:SubSetting"].ToString()
            };


            var logger = _serviceProvider.GetRequiredService<ILogger<Worker>>();
            _workerService = new Worker(logger, _serviceProvider);
        }

        [Fact]
        public async Task ExecuteAsync_ProcessesJobsAndLogs()
        {
            // Arrange
            var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            //using (var scope = scopeFactory.CreateScope())
            //{
            //    var context = scope.ServiceProvider.GetRequiredService<JobsDbContext>();
            //    context.Jobs.AddRange(
            //        new Job { Id = 1, Name = "Job1" },
            //        new Job { Id = 2, Name = "Job2" }
            //    );
            //    await context.SaveChangesAsync();
            //}

            using (var scope = scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IJobRepo>();
                await context.AddJobAsync(new Job { Id = 1, Name = "Job1" });
                await context.AddJobAsync(new Job { Id = 2, Name = "Job2" });
            }

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(3000); // Cancel after 3 seconds to stop the background service

            // Act
            await _workerService.StartAsync(cancellationTokenSource.Token);

            // Assert
            using (var scope = scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobsDbContext>();
                var jobLogs = await context.JobLogs.ToListAsync();

                Assert.Equal(2, jobLogs.Count);
                Assert.Contains(jobLogs, log => log.JobName == "Job1");
                Assert.Contains(jobLogs, log => log.JobName == "Job2");
            }
        }
    }
}

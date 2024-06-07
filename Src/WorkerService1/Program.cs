using Microsoft.EntityFrameworkCore;
using WorkerService1;
using WorkerService1.Model;
using WorkerService1.Service;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<JobsDbContext>(options =>
{
    options.UseInMemoryDatabase("TestDatabase");
});

builder.Services.AddScoped<IJobRepo, JobRepo>();
builder.Services.AddScoped<IJobLogRepo, JobLogRepo>();
builder.Services.AddHostedService<Worker>();


var host = builder.Build();
host.Run();

using WorkerService1.Model;

namespace WorkerService1.Aggregates
{
    public record JobDto : IMapFrom<Job>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

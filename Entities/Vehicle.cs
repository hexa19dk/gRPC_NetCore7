using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GrpcNet7.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
    }
}

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GrpcNet7.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
    }
}

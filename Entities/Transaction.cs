using System.ComponentModel.DataAnnotations.Schema;

namespace GrpcNet7.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public string? Rental_number { get; set; }
        public DateTime Trans_date { get; set; }
        public string? Destination { get; set; }
        public double Price { get; set; }
        public DateTime Start_date { get; set; }
        public DateTime End_date { get; set; }
        public string? Warranty_type { get; set; }

        [ForeignKey("VehicleId")]
        public int VehicleId { get; set;}

        [ForeignKey("UserID")]
        public int UserId { get; set; }

        public Vehicle? vehicle { get; set; }
        public User? user { get; set; }
    }
}

namespace AirportManagement.Models
{
    public class Bagaj
    {
        public int Id { get; set; }
        public string Tag { get; set; } = string.Empty;
        public int PasagerId { get; set; }
        public double Greutate { get; set; }
    }
}

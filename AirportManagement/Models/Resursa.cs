namespace AirportManagement.Models
{
    public class Resursa
    {
        public int Id { get; set; }
        public string Nume { get; set; } = string.Empty;
        public string Tip { get; set; } = string.Empty;
        public bool Disponibila { get; set; }
    }
}

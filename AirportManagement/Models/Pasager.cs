namespace AirportManagement.Models
{
    public class Pasager
    {
        public int Id { get; set; }
        public string Nume { get; set; } = string.Empty;
        public string TicketNr { get; set; } = string.Empty;
        public int ZborId { get; set; }
        public bool CheckedIn { get; set; }
        public bool Boarded { get; set; }
    }
}

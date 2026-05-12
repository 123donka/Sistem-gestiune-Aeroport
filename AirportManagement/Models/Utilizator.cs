namespace AirportManagement.Models
{
    public class Utilizator
    {
        public int Id { get; set; }
        public string Nume { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Parola { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}

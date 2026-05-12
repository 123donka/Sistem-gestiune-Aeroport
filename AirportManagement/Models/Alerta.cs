using System;

namespace AirportManagement.Models
{
    public class Alerta
    {
        public int Id { get; set; }
        public string Mesaj { get; set; } = string.Empty;
        public bool Citita { get; set; }
        public DateTime Data { get; set; }
    }
}

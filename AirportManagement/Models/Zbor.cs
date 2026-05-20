using System;

namespace AirportManagement.Models
{
    public class Zbor
    {
        public int Id { get; set; }
        public string Cod { get; set; } = string.Empty;
        public string CompanieAeriana { get; set; } = string.Empty;
        public string TipZbor { get; set; } = string.Empty;
        public string Sursa { get; set; } = string.Empty;
        public string Destinatie { get; set; } = string.Empty;
        public DateTime Plecare { get; set; }
        public DateTime Sosire { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

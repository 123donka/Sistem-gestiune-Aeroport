using System.Data;
using AirportManagement.Models;
using AirportManagement.Services;

namespace AirportManagement.Controllers
{
    public class PasageriController
    {
        private readonly PasagerService _service = new PasagerService();
        public DataTable GetAll() => _service.GetAll();
        public bool Create(Pasager p) => _service.Create(p);
        public bool Update(Pasager p) => _service.Update(p);
        public bool Delete(int id) => _service.Delete(id);
        public bool SetCheckIn(int id, bool checkIn) => _service.SetCheckIn(id, checkIn);
        public bool SetBoarded(int id, bool boarded) => _service.SetBoarded(id, boarded);
    }
}

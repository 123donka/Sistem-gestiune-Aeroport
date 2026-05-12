using System.Data;
using AirportManagement.Models;
using AirportManagement.Services;

namespace AirportManagement.Controllers
{
    public class ZboruriController
    {
        private readonly ZborService _service = new ZborService();

        public DataTable GetAll() => _service.GetAll();
        public bool Create(Zbor z) => _service.Create(z);
        public bool Update(Zbor z) => _service.Update(z);
        public bool Delete(int id) => _service.Delete(id);
    }
}

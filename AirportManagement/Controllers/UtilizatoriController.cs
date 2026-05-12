using System.Data;
using AirportManagement.Services;

using AirportManagement.Models;

namespace AirportManagement.Controllers
{
    public class UtilizatoriController
    {
        private readonly UtilizatorService _service = new UtilizatorService();
        public DataTable GetAll() => _service.GetAll();
        public Utilizator? GetById(int id) => _service.GetById(id);
        public bool Create(Utilizator u) => _service.Create(u);
        public bool Update(Utilizator u) => _service.Update(u);
        public bool Delete(int id) => _service.Delete(id);
    }
}

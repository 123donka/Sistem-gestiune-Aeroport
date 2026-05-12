using System.Data;
using AirportManagement.Services;

namespace AirportManagement.Controllers
{
    public class AlerteController
    {
        private readonly AlerteService _service = new AlerteService();
        public DataTable GetAll() => _service.GetAll();
        public bool MarkAsRead(int id) => _service.MarkAsRead(id);
    }
}

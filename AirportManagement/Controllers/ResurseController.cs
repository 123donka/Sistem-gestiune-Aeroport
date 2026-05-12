using System.Data;
using AirportManagement.Services;
using AirportManagement.Models;

namespace AirportManagement.Controllers
{
    public class ResurseController
    {
        private readonly ResurseService _service = new ResurseService();
        public DataTable GetAll() => _service.GetAll();
        public bool Create(Resursa r) => _service.Create(r);
        public bool Update(Resursa r) => _service.Update(r);
        public bool Delete(int id) => _service.Delete(id);
        public bool AssignToZbor(int resId, int zborId) => _service.AssignToZbor(resId, zborId);
        public bool UnassignFromZbor(int resId, int zborId) => _service.UnassignFromZbor(resId, zborId);
    }
}

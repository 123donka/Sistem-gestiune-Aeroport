using AirportManagement.Models;
using AirportManagement.Services;

namespace AirportManagement.Controllers
{
    public class AuthController
    {
        private readonly AuthService _service = new AuthService();

        public Utilizator? Login(string username, string parola)
        {
            return _service.Login(username, parola);
        }

        public bool Register(Utilizator u)
        {
            return _service.Register(u);
        }
    }
}

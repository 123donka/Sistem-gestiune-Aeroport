using System;
using AirportManagement.Data;
using AirportManagement.Models;
using MySql.Data.MySqlClient;

namespace AirportManagement.Services
{
    public class AuthService
    {
        public Utilizator? Login(string username, string parola)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT id,nume,username,parola,rol FROM utilizatori WHERE username=@u LIMIT 1;", conn);
            cmd.Parameters.AddWithValue("@u", username);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            var u = new Utilizator
            {
                Id = reader.GetInt32("id"),
                Nume = reader.GetString("nume"),
                Username = reader.GetString("username"),
                Parola = reader.GetString("parola"),
                Rol = reader.GetString("rol")
            };

            if (u.Parola != parola) return null;
            return u;
        }

        public bool Register(Utilizator user)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            using var check = new MySqlCommand("SELECT COUNT(1) FROM utilizatori WHERE username=@u", conn);
            check.Parameters.AddWithValue("@u", user.Username);
            var exists = Convert.ToInt32(check.ExecuteScalar() ?? 0) > 0;
            if (exists) return false;

            using var cmd = new MySqlCommand("INSERT INTO utilizatori(nume,username,parola,rol) VALUES(@n,@u,@p,@r)", conn);
            cmd.Parameters.AddWithValue("@n", user.Nume);
            cmd.Parameters.AddWithValue("@u", user.Username);
            cmd.Parameters.AddWithValue("@p", user.Parola);
            cmd.Parameters.AddWithValue("@r", user.Rol);
            var r = cmd.ExecuteNonQuery();
            return r > 0;
        }
    }
}

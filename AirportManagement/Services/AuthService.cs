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
            var nameCol = DbContext.NameColumn("utilizatori");
            var pk = DbContext.PrimaryKeyColumnName("utilizatori");
            var sql = $"SELECT `{pk}` AS id,`{nameCol}`,username,parola,rol FROM utilizatori WHERE username=@u LIMIT 1;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", username);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            var u = new Utilizator
            {
                Id = reader.GetInt32("id"),
                Nume = reader.IsDBNull(reader.GetOrdinal(nameCol)) ? string.Empty : reader.GetString(nameCol),
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

            var nameColInsert = DbContext.NameColumn("utilizatori");
            var hasParolaHash = DbContext.ColumnExists("utilizatori", "parola_hash");
            string insertSql;
            if (hasParolaHash)
            {
                insertSql = $"INSERT INTO utilizatori(`{nameColInsert}`,username,parola,parola_hash,rol) VALUES(@n,@u,@p,@ph,@r)";
            }
            else
            {
                insertSql = $"INSERT INTO utilizatori(`{nameColInsert}`,username,parola,rol) VALUES(@n,@u,@p,@r)";
            }

            using var insertCmd = new MySqlCommand(insertSql, conn);
            insertCmd.Parameters.AddWithValue("@n", user.Nume);
            insertCmd.Parameters.AddWithValue("@u", user.Username);
            insertCmd.Parameters.AddWithValue("@p", user.Parola);
            if (hasParolaHash)
                insertCmd.Parameters.AddWithValue("@ph", user.Parola);
            insertCmd.Parameters.AddWithValue("@r", user.Rol);
            var r = insertCmd.ExecuteNonQuery();
            return r > 0;
        }
    }
}

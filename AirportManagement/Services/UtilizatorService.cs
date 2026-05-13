using System.Data;
using AirportManagement.Data;
using AirportManagement.Models;
using MySql.Data.MySqlClient;

namespace AirportManagement.Services
{
    public class UtilizatorService
    {
        public DataTable GetAll()
        {
            var dt = new DataTable();
            using var conn = DbContext.GetConnection();
            conn.Open();
            var nameCol = DbContext.NameColumn("utilizatori");
            var pk = DbContext.PrimaryKeyColumnName("utilizatori");

            // Include last activity from logactivitati (if any) and a simple "activ" flag
            var sql = $@"
SELECT u.`{pk}` AS id,
       u.`{nameCol}` AS nume,
       u.username,
       u.rol,
       (SELECT MAX(data) FROM logactivitati la WHERE la.utilizator = u.username) AS ultima_logare,
       CASE WHEN (SELECT MAX(data) FROM logactivitati la WHERE la.utilizator = u.username) >= DATE_SUB(UTC_TIMESTAMP(), INTERVAL 30 DAY) THEN 1 ELSE 0 END AS activ
FROM utilizatori u";

            using var cmd = new MySqlCommand(sql, conn);
            using var adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        public Utilizator? GetById(int id)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            var nameCol = DbContext.NameColumn("utilizatori");
            var pk = DbContext.PrimaryKeyColumnName("utilizatori");
            using var cmd = new MySqlCommand($"SELECT `{pk}` AS id,`{nameCol}`,username,parola,rol FROM utilizatori WHERE `{pk}`=@id LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            return new Utilizator
            {
                Id = reader.GetInt32("id"),
                Nume = reader.IsDBNull(reader.GetOrdinal(nameCol)) ? string.Empty : reader.GetString(nameCol),
                Username = reader.GetString("username"),
                Parola = reader.GetString("parola"),
                Rol = reader.GetString("rol")
            };
        }

        public bool Create(Utilizator u)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            using var check = new MySqlCommand("SELECT COUNT(1) FROM utilizatori WHERE username=@username", conn);
            check.Parameters.AddWithValue("@username", u.Username);
            var exists = Convert.ToInt32(check.ExecuteScalar() ?? 0) > 0;
            if (exists) return false;
            var nameColInsert = DbContext.NameColumn("utilizatori");
            var hasParolaHash = DbContext.ColumnExists("utilizatori", "parola_hash");
            string sql;
            if (hasParolaHash)
                sql = $"INSERT INTO utilizatori(`{nameColInsert}`,username,parola,parola_hash,rol) VALUES(@n,@u,@p,@ph,@r)";
            else
                sql = $"INSERT INTO utilizatori(`{nameColInsert}`,username,parola,rol) VALUES(@n,@u,@p,@r)";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@n", u.Nume);
            cmd.Parameters.AddWithValue("@u", u.Username);
            cmd.Parameters.AddWithValue("@p", u.Parola);
            if (hasParolaHash)
                cmd.Parameters.AddWithValue("@ph", u.Parola);
            cmd.Parameters.AddWithValue("@r", u.Rol);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Update(Utilizator u)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            if (!string.IsNullOrEmpty(u.Parola))
            {
                var nameColUpdate = DbContext.NameColumn("utilizatori");
                var pk = DbContext.PrimaryKeyColumnName("utilizatori");
                using var cmd = new MySqlCommand($"UPDATE utilizatori SET `{nameColUpdate}`=@n,username=@u,parola=@p,rol=@r WHERE `{pk}`=@id", conn);
                cmd.Parameters.AddWithValue("@n", u.Nume);
                cmd.Parameters.AddWithValue("@u", u.Username);
                cmd.Parameters.AddWithValue("@p", u.Parola);
                cmd.Parameters.AddWithValue("@r", u.Rol);
                cmd.Parameters.AddWithValue("@id", u.Id);
                return cmd.ExecuteNonQuery() > 0;
            }
            else
            {
                var nameColUpdate = DbContext.NameColumn("utilizatori");
                var pk = DbContext.PrimaryKeyColumnName("utilizatori");
                using var cmd = new MySqlCommand($"UPDATE utilizatori SET `{nameColUpdate}`=@n,username=@u,rol=@r WHERE `{pk}`=@id", conn);
                cmd.Parameters.AddWithValue("@n", u.Nume);
                cmd.Parameters.AddWithValue("@u", u.Username);
                cmd.Parameters.AddWithValue("@r", u.Rol);
                cmd.Parameters.AddWithValue("@id", u.Id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int id)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            var pk = DbContext.PrimaryKeyColumnName("utilizatori");
            using var cmd = new MySqlCommand($"DELETE FROM utilizatori WHERE `{pk}`=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}

using System.Data;
using AirportManagement.Data;
using AirportManagement.Models;
using MySql.Data.MySqlClient;

namespace AirportManagement.Services
{
    public class AlerteService
    {
        public DataTable GetAll()
        {
            var dt = new DataTable();
            using var conn = DbContext.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT id,mesaj,citita,data FROM alerte", conn);
            using var adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        public bool MarkAsRead(int id)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("UPDATE alerte SET citita=1 WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}

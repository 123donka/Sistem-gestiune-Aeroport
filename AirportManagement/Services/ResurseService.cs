using System.Data;
using AirportManagement.Data;
using AirportManagement.Models;
using MySql.Data.MySqlClient;

namespace AirportManagement.Services
{
    public class ResurseService
    {
        public DataTable GetAll()
        {
            var dt = new DataTable();
            using var conn = DbContext.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("SELECT id,nume,tip,disponibila FROM resurse", conn);
            using var adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        public bool Create(Resursa r)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("INSERT INTO resurse(nume,tip,disponibila) VALUES(@n,@t,@d)", conn);
            cmd.Parameters.AddWithValue("@n", r.Nume);
            cmd.Parameters.AddWithValue("@t", r.Tip);
            cmd.Parameters.AddWithValue("@d", r.Disponibila);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Update(Resursa r)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("UPDATE resurse SET nume=@n,tip=@t,disponibila=@d WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@n", r.Nume);
            cmd.Parameters.AddWithValue("@t", r.Tip);
            cmd.Parameters.AddWithValue("@d", r.Disponibila);
            cmd.Parameters.AddWithValue("@id", r.Id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("DELETE FROM resurse WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool AssignToZbor(int resId, int zborId)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            using var check = new MySqlCommand("SELECT COUNT(1) FROM resurse_alocari WHERE resursa_id=@r AND zbor_id=@z", conn);
            check.Parameters.AddWithValue("@r", resId);
            check.Parameters.AddWithValue("@z", zborId);
            var exists = Convert.ToInt32(check.ExecuteScalar() ?? 0) > 0;
            if (exists) return false;
            using var cmd = new MySqlCommand("INSERT INTO resurse_alocari(resursa_id,zbor_id) VALUES(@r,@z)", conn);
            cmd.Parameters.AddWithValue("@r", resId);
            cmd.Parameters.AddWithValue("@z", zborId);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool UnassignFromZbor(int resId, int zborId)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("DELETE FROM resurse_alocari WHERE resursa_id=@r AND zbor_id=@z", conn);
            cmd.Parameters.AddWithValue("@r", resId);
            cmd.Parameters.AddWithValue("@z", zborId);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}

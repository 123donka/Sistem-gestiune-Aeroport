using System.Data;
using AirportManagement.Data;
using MySql.Data.MySqlClient;
using AirportManagement.Models;

namespace AirportManagement.Services
{
    public class ZborService
    {
        public DataTable GetAll()
        {
            var dt = new DataTable();
            using var conn = DbContext.GetConnection();
            conn.Open();
            var pk = DbContext.PrimaryKeyColumnName("zboruri");
            using var cmd = new MySqlCommand($"SELECT `{pk}` AS id,cod,sursa,destinatie,plecare,sosire,status FROM zboruri", conn);
            using var adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        public bool Create(Zbor z)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            using var cmd = new MySqlCommand("INSERT INTO zboruri(cod,sursa,destinatie,plecare,sosire,status) VALUES(@cod,@src,@dst,@plec,@sos,@st)", conn);
            cmd.Parameters.AddWithValue("@cod", z.Cod);
            cmd.Parameters.AddWithValue("@src", z.Sursa);
            cmd.Parameters.AddWithValue("@dst", z.Destinatie);
            cmd.Parameters.AddWithValue("@plec", z.Plecare);
            cmd.Parameters.AddWithValue("@sos", z.Sosire);
            cmd.Parameters.AddWithValue("@st", z.Status);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Update(Zbor z)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            var pk = DbContext.PrimaryKeyColumnName("zboruri");
            using var cmd = new MySqlCommand($"UPDATE zboruri SET cod=@cod,sursa=@src,destinatie=@dst,plecare=@plec,sosire=@sos,status=@st WHERE `{pk}`=@id", conn);
            cmd.Parameters.AddWithValue("@cod", z.Cod);
            cmd.Parameters.AddWithValue("@src", z.Sursa);
            cmd.Parameters.AddWithValue("@dst", z.Destinatie);
            cmd.Parameters.AddWithValue("@plec", z.Plecare);
            cmd.Parameters.AddWithValue("@sos", z.Sosire);
            cmd.Parameters.AddWithValue("@st", z.Status);
            cmd.Parameters.AddWithValue("@id", z.Id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            var pk = DbContext.PrimaryKeyColumnName("zboruri");
            using var cmd = new MySqlCommand($"DELETE FROM zboruri WHERE `{pk}`=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}

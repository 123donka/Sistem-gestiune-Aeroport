using System.Data;
using AirportManagement.Data;
using AirportManagement.Models;
using MySql.Data.MySqlClient;

namespace AirportManagement.Services
{
    public class PasagerService
    {
        public DataTable GetAll()
        {
            var dt = new DataTable();
            using var conn = DbContext.GetConnection();
            conn.Open();
            var nameCol = DbContext.NameColumn("pasageri");
            var pk = DbContext.PrimaryKeyColumnName("pasageri");
            using var cmd = new MySqlCommand($"SELECT `{pk}` AS id,`{nameCol}`,ticketnr,zborid,checkedin,boarded FROM pasageri", conn);
            using var adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        public bool Create(Pasager p)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            var nameColInsert = DbContext.NameColumn("pasageri");
            using var cmd = new MySqlCommand($"INSERT INTO pasageri(`{nameColInsert}`,ticketnr,zborid,checkedin,boarded) VALUES(@n,@t,@z,@c,@b)", conn);
            cmd.Parameters.AddWithValue("@n", p.Nume);
            cmd.Parameters.AddWithValue("@t", p.TicketNr);
            cmd.Parameters.AddWithValue("@z", p.ZborId);
            cmd.Parameters.AddWithValue("@c", p.CheckedIn);
            cmd.Parameters.AddWithValue("@b", p.Boarded);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Update(Pasager p)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            var nameColUpdate = DbContext.NameColumn("pasageri");
            var pk = DbContext.PrimaryKeyColumnName("pasageri");
            using var cmd = new MySqlCommand($"UPDATE pasageri SET `{nameColUpdate}`=@n,ticketnr=@t,zborid=@z,checkedin=@c,boarded=@b WHERE `{pk}`=@id", conn);
            cmd.Parameters.AddWithValue("@n", p.Nume);
            cmd.Parameters.AddWithValue("@t", p.TicketNr);
            cmd.Parameters.AddWithValue("@z", p.ZborId);
            cmd.Parameters.AddWithValue("@c", p.CheckedIn);
            cmd.Parameters.AddWithValue("@b", p.Boarded);
            cmd.Parameters.AddWithValue("@id", p.Id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            var pk = DbContext.PrimaryKeyColumnName("pasageri");
            using var cmd = new MySqlCommand($"DELETE FROM pasageri WHERE `{pk}`=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool SetCheckIn(int id, bool checkedIn)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            var pk = DbContext.PrimaryKeyColumnName("pasageri");
            using var cmd = new MySqlCommand($"UPDATE pasageri SET checkedin=@c WHERE `{pk}`=@id", conn);
            cmd.Parameters.AddWithValue("@c", checkedIn ? 1 : 0);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool SetBoarded(int id, bool boarded)
        {
            using var conn = DbContext.GetConnection();
            conn.Open();
            var pk = DbContext.PrimaryKeyColumnName("pasageri");
            using var cmd = new MySqlCommand($"UPDATE pasageri SET boarded=@b WHERE `{pk}`=@id", conn);
            cmd.Parameters.AddWithValue("@b", boarded ? 1 : 0);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}

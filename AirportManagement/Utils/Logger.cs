using System;
using AirportManagement.Data;
using MySql.Data.MySqlClient;

namespace AirportManagement.Utils
{
    public static class Logger
    {
        public static void Log(string utilizator, string actiune)
        {
            try
            {
                using var conn = DbContext.GetConnection();
                conn.Open();
                using var cmd = new MySqlCommand("INSERT INTO logactivitati(utilizator,actiune,data) VALUES(@u,@a,@d)", conn);
                cmd.Parameters.AddWithValue("@u", utilizator);
                cmd.Parameters.AddWithValue("@a", actiune);
                cmd.Parameters.AddWithValue("@d", DateTime.UtcNow);
                cmd.ExecuteNonQuery();
            }
            catch
            {
            }
        }
    }
}

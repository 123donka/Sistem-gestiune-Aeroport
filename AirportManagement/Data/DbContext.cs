using System;
using System.IO;
using System.Text.Json;
using MySql.Data.MySqlClient;

namespace AirportManagement.Data
{
    public static class DbContext
    {
        private static string? _connectionStringCache;

        public static string ConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(_connectionStringCache))
                    return _connectionStringCache!;

                try
                {
                    var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                    if (File.Exists(path))
                    {
                        var json = File.ReadAllText(path);
                        using var doc = JsonDocument.Parse(json);
                        if (doc.RootElement.TryGetProperty("ConnectionStrings", out var cs) &&
                            cs.TryGetProperty("DefaultConnection", out var defaultConn))
                        {
                            _connectionStringCache = defaultConn.GetString();
                            if (!string.IsNullOrEmpty(_connectionStringCache))
                                return _connectionStringCache;
                        }
                    }
                }
                catch
                {
                }

                _connectionStringCache = "server=localhost;user=root;password=yourpassword;database=airportdb;";
                return _connectionStringCache;
            }
        }

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}

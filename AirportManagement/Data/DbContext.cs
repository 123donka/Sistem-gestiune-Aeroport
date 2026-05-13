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

                // 1) Check for explicit environment variable (recommended for secrets)
                try
                {
                    var env = Environment.GetEnvironmentVariable("AIRPORT_CONN");
                    if (!string.IsNullOrEmpty(env))
                    {
                        _connectionStringCache = env;
                        return _connectionStringCache;
                    }
                }
                catch
                {
                }

                // 2) Try reading appsettings.json from the running exe folder
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

                // 3) Fallback (use TCP 127.0.0.1 to avoid named-pipe/socket resolution differences)
                _connectionStringCache = "server=127.0.0.1;user=root;password=denis@;database=airportdb;";
                return _connectionStringCache;
            }
        }

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public static bool ColumnExists(string table, string column)
        {
            try
            {
                using var conn = GetConnection();
                conn.Open();
                var builder = new MySqlConnectionStringBuilder(ConnectionString);
                var dbName = builder.Database;
                using var cmd = new MySqlCommand("SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=@db AND TABLE_NAME=@table AND COLUMN_NAME=@col", conn);
                cmd.Parameters.AddWithValue("@db", dbName);
                cmd.Parameters.AddWithValue("@table", table);
                cmd.Parameters.AddWithValue("@col", column);
                var exists = Convert.ToInt32(cmd.ExecuteScalar() ?? 0) > 0;
                return exists;
            }
            catch
            {
                return false;
            }
        }

        public static string NameColumn(string table)
        {
            try
            {
                if (ColumnExists(table, "nume")) return "nume";
                if (ColumnExists(table, "nume_complet")) return "nume_complet";
                return "nume";
            }
            catch
            {
                return "nume";
            }
        }

        public static string PrimaryKeyColumnName(string table)
        {
            try
            {
                using var conn = GetConnection();
                conn.Open();
                var builder = new MySqlConnectionStringBuilder(ConnectionString);
                var dbName = builder.Database;
                using var cmd = new MySqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=@db AND TABLE_NAME=@table AND COLUMN_KEY='PRI' ORDER BY ORDINAL_POSITION LIMIT 1", conn);
                cmd.Parameters.AddWithValue("@db", dbName);
                cmd.Parameters.AddWithValue("@table", table);
                var res = cmd.ExecuteScalar() as string;
                if (!string.IsNullOrEmpty(res)) return res;

                // Fallback common patterns
                if (ColumnExists(table, "id")) return "id";
                if (ColumnExists(table, $"id_{table}")) return $"id_{table}";
                if (ColumnExists(table, $"{table}_id")) return $"{table}_id";
                if (ColumnExists(table, "id_utilizator")) return "id_utilizator";
                return "id";
            }
            catch
            {
                return "id";
            }
        }
    }
}

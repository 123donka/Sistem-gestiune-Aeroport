using System;
using System.IO;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace AirportManagement.Data
{
    public static class DatabaseInitializer
    {
        public static void EnsureDatabase()
        {
            try
            {
                var cs = DbContext.ConnectionString;
                var builder = new MySqlConnectionStringBuilder(cs);
                var dbName = builder.Database;

                // Remove database from connection string so we can create it if missing
                if (builder.ContainsKey("Database"))
                    builder.Remove("Database");

                using var conn = new MySqlConnection(builder.ConnectionString);
                conn.Open();

                using var check = new MySqlCommand("SELECT COUNT(1) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @db", conn);
                check.Parameters.AddWithValue("@db", dbName);
                var exists = Convert.ToInt32(check.ExecuteScalar() ?? 0) > 0;

                // If the database doesn't exist, try to run the init script (if present)
                if (!exists)
                {
                    var candidate = Path.Combine(AppContext.BaseDirectory, "sql", "init.sql");
                    if (!File.Exists(candidate))
                    {
                        candidate = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "sql", "init.sql"));
                    }

                    if (File.Exists(candidate))
                    {
                        var script = File.ReadAllText(candidate);
                        var mys = new MySqlScript(conn, script);
                        try { mys.Execute(); } catch { }
                    }
                }

                // Now ensure expected columns exist in important tables (fixes existing databases with missing columns)
                try
                {
                    using var dbConn = new MySqlConnection(cs);
                    dbConn.Open();

                    var expected = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["utilizatori"] = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["nume"] = "VARCHAR(255) NULL",
                            ["username"] = "VARCHAR(100) NULL",
                            ["parola"] = "VARCHAR(255) NULL",
                            ["rol"] = "VARCHAR(50) NOT NULL DEFAULT 'operator'"
                        },
                        ["pasageri"] = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["nume"] = "VARCHAR(255) NULL",
                            ["ticketnr"] = "VARCHAR(100) NULL",
                            ["zborid"] = "INT NULL",
                            ["checkedin"] = "TINYINT(1) DEFAULT 0",
                            ["boarded"] = "TINYINT(1) DEFAULT 0"
                        },
                        ["resurse"] = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["nume"] = "VARCHAR(255) NULL",
                            ["tip"] = "VARCHAR(100) NULL",
                            ["disponibila"] = "TINYINT(1) DEFAULT 1"
                        },
                        ["zboruri"] = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["cod"] = "VARCHAR(50) NULL",
                            ["sursa"] = "VARCHAR(255) NULL",
                            ["destinatie"] = "VARCHAR(255) NULL",
                            ["plecare"] = "DATETIME NULL",
                            ["sosire"] = "DATETIME NULL",
                            ["status"] = "VARCHAR(50) NULL"
                        },
                        ["resurse_alocari"] = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["resursa_id"] = "INT NULL",
                            ["zbor_id"] = "INT NULL",
                            ["assigned_at"] = "DATETIME DEFAULT CURRENT_TIMESTAMP"
                        },
                        ["alerte"] = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["mesaj"] = "TEXT NULL",
                            ["citita"] = "TINYINT(1) DEFAULT 0",
                            ["data"] = "DATETIME DEFAULT CURRENT_TIMESTAMP"
                        },
                        ["logactivitati"] = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["utilizator"] = "VARCHAR(255) NULL",
                            ["actiune"] = "TEXT NULL",
                            ["data"] = "DATETIME DEFAULT CURRENT_TIMESTAMP"
                        }
                    };

                    foreach (var tableKvp in expected)
                    {
                        var table = tableKvp.Key;

                        using var tableCheck = new MySqlCommand("SELECT COUNT(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA=@db AND TABLE_NAME=@table", dbConn);
                        tableCheck.Parameters.AddWithValue("@db", dbName);
                        tableCheck.Parameters.AddWithValue("@table", table);
                        var tableExists = Convert.ToInt32(tableCheck.ExecuteScalar() ?? 0) > 0;
                        if (!tableExists) continue;

                        foreach (var colKvp in tableKvp.Value)
                        {
                            var col = colKvp.Key;
                            var def = colKvp.Value;

                            using var colCheck = new MySqlCommand("SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=@db AND TABLE_NAME=@table AND COLUMN_NAME=@col", dbConn);
                            colCheck.Parameters.AddWithValue("@db", dbName);
                            colCheck.Parameters.AddWithValue("@table", table);
                            colCheck.Parameters.AddWithValue("@col", col);
                            var colExists = Convert.ToInt32(colCheck.ExecuteScalar() ?? 0) > 0;
                            if (colExists) continue;

                            var alter = $"ALTER TABLE `{table}` ADD COLUMN `{col}` {def}";
                            using var addCmd = new MySqlCommand(alter, dbConn);
                            try { addCmd.ExecuteNonQuery(); } catch { }
                        }
                    }

                    // If a legacy or external schema added `parola_hash` as NOT NULL without a default,
                    // attempts to INSERT without that column will fail. Make it nullable or give it a safe default.
                    try
                    {
                        using var colInfoCmd = new MySqlCommand(
                            "SELECT COLUMN_TYPE, IS_NULLABLE, COLUMN_DEFAULT FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=@db AND TABLE_NAME='utilizatori' AND COLUMN_NAME='parola_hash'",
                            dbConn);
                        colInfoCmd.Parameters.AddWithValue("@db", dbName);
                        using var colReader = colInfoCmd.ExecuteReader();
                        if (colReader.Read())
                        {
                            var colType = colReader.IsDBNull(0) ? null : colReader.GetString(0);
                            var isNullable = colReader.IsDBNull(1) ? null : colReader.GetString(1);
                            var colDefault = colReader.IsDBNull(2) ? null : colReader.GetString(2);
                            colReader.Close();

                            if (!string.IsNullOrEmpty(colType) && string.Equals(isNullable, "NO", StringComparison.OrdinalIgnoreCase) && colDefault == null)
                            {
                                var alter = $"ALTER TABLE `utilizatori` MODIFY COLUMN `parola_hash` {colType} NULL";
                                using var alterCmd = new MySqlCommand(alter, dbConn);
                                try { alterCmd.ExecuteNonQuery(); }
                                catch
                                {
                                    // If MODIFY fails for some reason, try to set a default empty string instead
                                    try
                                    {
                                        var alter2 = $"ALTER TABLE `utilizatori` MODIFY COLUMN `parola_hash` {colType} NOT NULL DEFAULT ''";
                                        using var alterCmd2 = new MySqlCommand(alter2, dbConn);
                                        alterCmd2.ExecuteNonQuery();
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }

                    // Also ensure any `nume_complet` columns are nullable or have a default to avoid INSERT errors
                    try
                    {
                        var nameTables = new[] { "utilizatori", "pasageri", "resurse" };
                        foreach (var t in nameTables)
                        {
                            using var colInfoCmd2 = new MySqlCommand(
                                "SELECT COLUMN_TYPE, IS_NULLABLE, COLUMN_DEFAULT FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=@db AND TABLE_NAME=@table AND COLUMN_NAME='nume_complet'",
                                dbConn);
                            colInfoCmd2.Parameters.AddWithValue("@db", dbName);
                            colInfoCmd2.Parameters.AddWithValue("@table", t);
                            using var rdr = colInfoCmd2.ExecuteReader();
                            if (rdr.Read())
                            {
                                var colType = rdr.IsDBNull(0) ? null : rdr.GetString(0);
                                var isNullable = rdr.IsDBNull(1) ? null : rdr.GetString(1);
                                var colDefault = rdr.IsDBNull(2) ? null : rdr.GetString(2);
                                rdr.Close();

                                if (!string.IsNullOrEmpty(colType) && string.Equals(isNullable, "NO", StringComparison.OrdinalIgnoreCase) && colDefault == null)
                                {
                                    var alter = $"ALTER TABLE `{t}` MODIFY COLUMN `nume_complet` {colType} NULL";
                                    using var alt = new MySqlCommand(alter, dbConn);
                                    try { alt.ExecuteNonQuery(); }
                                    catch
                                    {
                                        try
                                        {
                                            var alter2 = $"ALTER TABLE `{t}` MODIFY COLUMN `nume_complet` {colType} NOT NULL DEFAULT ''";
                                            using var alt2 = new MySqlCommand(alter2, dbConn);
                                            alt2.ExecuteNonQuery();
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                catch
                {
                }
            }
            catch
            {
                // Don't let initialization break the UI startup; failures can be diagnosed separately.
            }
        }
    }
}

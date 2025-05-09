using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using MyApp.Models;

namespace MyApp.Services
{
    public class UserLoginInfo
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
    }
    public class DatabaseService
    {
        private readonly string _connectionString;
        private const string DB_FILENAME = "webapp_database.sqlite";

        public DatabaseService(IConfiguration configuration)
        {
            string basePath = AppContext.BaseDirectory;

            _connectionString = $"Data Source={Path.Combine(basePath, DB_FILENAME)}";

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                try 
                {
                    connection.Open();

                    string createLoginTableSql = @"
                        CREATE TABLE IF NOT EXISTS Logins(
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Username TEXT NOT NULL UNIQUE,
                            PasswordHash TEXT NOT NULl
                        );
                    ";

                    using (var command = new SqliteCommand(createLoginTableSql, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Logins table successfully created/checked");
                    }

                    string createDataEntriesTableSql = @"
                        CREATE TABLE IF NOT EXISTS DataEntries(
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            TextData TEXT
                        );
                    ";

                    using (var command = new SqliteCommand(createDataEntriesTableSql, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("DataEntries table successfully created/checked");
                    }

                    PopulateInitialData(connection);
                }
                catch (Exception e) 
                {
                    Console.WriteLine("Exception during initializing database");
                }
            }
        }

        private string ComputeMd5Hash(string rawData) 
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder sb = new StringBuilder();

                for (int i=0; i<data.Length; i++) 
                {
                    sb.Append(data[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private void PopulateInitialData(SqliteConnection connection)
        {
            using (var command = new SqliteCommand("SELECT COUNT(*) FROM Logins", connection))
            {
                long count = (long)command.ExecuteScalar();

                if (count == 0)
                {
                    Console.WriteLine("Insering initialize logins data to Logins table");

                    string insertUserSql = "INSERT INTO Logins(Username, PasswordHash) VALUES(@Username, @PasswordHash);";

                    // user, user123
                    using (var insertCommand = new SqliteCommand(insertUserSql, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Username", "user");
                        insertCommand.Parameters.AddWithValue("@PasswordHash", ComputeMd5Hash("user123"));
                        insertCommand.ExecuteNonQuery();
                    }

                    // admin, admin123
                    using (var insertCommand = new SqliteCommand(insertUserSql, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Username", "admin");
                        insertCommand.Parameters.AddWithValue("@PasswordHash", ComputeMd5Hash("admin123"));
                        insertCommand.ExecuteNonQuery();
                    }

                    Console.WriteLine("Inserted user and admin test values to Logins table");
                }
            }


            using (var command = new SqliteCommand("SELECT COUNT(*) FROM DataEntries", connection))
            {
                long count = (long)command.ExecuteScalar();

                if (count == 0)
                {
                    Console.WriteLine("Insering initialize data emtries to DataEntries table");

                    string insertUserSql = "INSERT INTO DataEntries(TextData) VALUES(@TextData);";

                    
                    using (var insertCommand = new SqliteCommand(insertUserSql, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@TextData", "Robert Lewandowski (ur. 21 sierpnia 1988 w Warszawie)");
                        insertCommand.ExecuteNonQuery();
                    }

                    using (var insertCommand = new SqliteCommand(insertUserSql, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@TextData", "Lamine Yamal Nasraoui Ebana (ur. 13 lipca 2007 w Esplugues de Llobregat)");
                        insertCommand.ExecuteNonQuery();
                    }

                    Console.WriteLine("Inserted Lewy and Yamal test values to DataEntries table");
                }
            }
        }

        public UserLoginInfo? GetUserLogin(string username, string plainPassword)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(plainPassword))
            {
                return null;
            }

            string passwordToCheckHash = ComputeMd5Hash(plainPassword);

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT Id, Username, PasswordHash FROM Logins WHERE Username = @Username;";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read()) 
                        {
                            string storedPasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
                            if (storedPasswordHash == passwordToCheckHash) 
                            {
                                UserLoginInfo userLoginInfo = new UserLoginInfo();
                                userLoginInfo.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                userLoginInfo.Username = reader.GetString(reader.GetOrdinal("Username"));
                                return userLoginInfo;
                            }
                        }
                    }
                }
            }
            return null;
        }


        public bool AddDataEntry(string textData)
        {
            if (string.IsNullOrEmpty(textData)) 
            {
                return false;
            }

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO DataEntries(TextData) VALUES(@TextData);";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TextData", textData);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public List<DataEntry> GetAllDataEntries() 
        {
            var entries = new List<DataEntry>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT Id, TextData FROM DataEntries ORDER BY Id DESC";

                using(var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            entries.Add(new DataEntry
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                TextData = reader.GetString(reader.GetOrdinal("TextData"))
                            });
                        }
                    }
                }
            }
            return entries;
        }
        
    }
}
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace FireBrowserMultiCore
{
    public class Sys
    {
        private List<User> users;
        private string usersFilePath;
        private string documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string userBaseDirectory = "FireBrowserUserCore";



        public List<User> Users { get; private set; }

        public Sys()
        {
            usersFilePath = Path.Combine(documentsDirectory, userBaseDirectory, "users.json");
            LoadUsers();
        }

        public void LoadUsers()
        {
            try
            {
                if (File.Exists(usersFilePath))
                {
                    string json = File.ReadAllText(usersFilePath);
                    users = JsonSerializer.Deserialize<List<User>>(json);
                }
                else
                {
                    users = new List<User>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading users: {ex.Message}");
                users = new List<User>();
            }
        }

        public void SaveUsers()
        {
            try
            {
                string json = JsonSerializer.Serialize(users, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(usersFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving users: {ex.Message}");
            }
        }

        public void CreateUser(string username)
        {
            try
            {
                if (users.Exists(u => u.Username == username))
                {
                    Console.WriteLine($"User '{username}' already exists.");
                }
                else
                {
                    var newUser = new User
                    {
                        IsFirstLaunch = true,
                        Username = username,
                        UserId = Guid.NewGuid(),
                        DirectoryPath = Path.Combine(documentsDirectory, userBaseDirectory, "Users", username),
                        DataBasePath = Path.Combine(documentsDirectory, userBaseDirectory, "Users", username, "database", "history.db"), // Use DirectoryPath
                        BrowserPath = Path.Combine(documentsDirectory, userBaseDirectory, "Users", username, "browser", "settings.json")
                    };

                    users.Add(newUser);
                    SaveUsers();

                    // Create user directory and subdirectories
                    Directory.CreateDirectory(newUser.DirectoryPath);
                    Directory.CreateDirectory(Path.Combine(newUser.DirectoryPath, "browser"));
                    Directory.CreateDirectory(Path.Combine(newUser.DirectoryPath, "database"));
                    Directory.CreateDirectory(Path.Combine(newUser.DirectoryPath, "userpicture"));

                    string databaseFolderPath = Path.Combine(newUser.DirectoryPath, "database");

                    // Save default browser settings to Settings.json
                    string settingsFilePath = newUser.BrowserPath;
                    string settingsJson = JsonSerializer.Serialize(new Settings // Create a new Settings object for each user
                    {
                        // ... (default settings as before)
                    }, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    File.WriteAllText(settingsFilePath, settingsJson);

                    string historyDbPath = Path.Combine(databaseFolderPath, "history.db");
                    CreateHistoryDatabase(historyDbPath);

                    Console.WriteLine($"User '{username}' created successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
            }
        }

       

        private void CreateHistoryDatabase(string dbFilePath)
        {
            Batteries_V2.Init();
            try
            {
                using (var connection = new SqliteConnection($"Data Source={dbFilePath}"))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "CREATE TABLE IF NOT EXISTS urlsDb (Url NVARCHAR(2583) PRIMARY KEY NOT NULL, " +
                                              "Title NVARCHAR(2548), " +
                                              "Visit_Count INTEGER, " +
                                              "Last_Visit_Time DATETIME)";
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating history database: {ex.Message}");
            }
        }
        // ... (other methods remain the same)
    }
}

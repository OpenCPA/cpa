using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Config.Net;
using OpenCPA.Data;
using OpenCPA.Security;
using SQLite;

namespace OpenCPA.Database
{
    /// <summary>
    /// The OpenCPA database manager.
    /// </summary>
    public static class DBMan
    {
        public static SQLiteConnection Instance = null;
        public static ICPASettings Settings = null;

        /// <summary>
        /// Initializes the database instance to be used for querying.
        /// </summary>
        public static void Initialize()
        {
            //Initialize the settings.
            Settings = new ConfigurationBuilder<ICPASettings>().UseIniFile("settings.ini").Build();

            //Check if it's already active.
            if (Instance != null) { throw new Exception("Database already initialized."); }

            //Get path to DB.
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.DBName);

            //Does the database exist already?
            bool exists = File.Exists(dbPath);

            //Create instance.
            Instance = new SQLiteConnection(dbPath);

            //If it didn't exist, create the tables.
            if (!exists)
            {
                Instance.CreateTable<Album>();
                Instance.CreateTable<Artist>();
                Instance.CreateTable<Resource>();
                Instance.CreateTable<Track>();
                Instance.CreateTable<User>();

                //Create the default user.
                User defaultUser = new User()
                {
                    GUID = Guid.NewGuid().ToString(),
                    Username = Settings.DefaultUserUsername,
                    HashedPassword = Hash.Create(Settings.DefaultUserPassword, Settings.PasswordHashStrength),
                    Permissions = Settings.DefaultUserPermissions
                };
                Instance.Insert(defaultUser);
            }
        }
    }
}
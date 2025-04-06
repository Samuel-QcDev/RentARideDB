﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentARideDB.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace RentARideDB.Services
{
    public class ApplicationDbContext
    {
        public SQLiteAsyncConnection _dbConnection;
        public SQLiteAsyncConnection DbConnection => _dbConnection;

        // Application Database
        public readonly static string nameSpace = "RentARideDB.Services.";

        public const string DatabaseFilename = "RentARideDB.DbContext.db3";

        public static string databasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        public const SQLite.SQLiteOpenFlags Flags =
            SQLite.SQLiteOpenFlags.ReadWrite |  // open the database in read/write mode
            SQLite.SQLiteOpenFlags.Create |     // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.SharedCache; // enable multi-threaded database access

        private static ApplicationDbContext _instance;
        public ObservableCollection<Reservation> ReservationsResultPast { get; set; }
        public ObservableCollection<Reservation> ReservationsResultCurrent { get; set; }
   
        public ApplicationDbContext()
        {
            // Initialize the database connection here
            //DeleteDatabase();
            _dbConnection = new SQLiteAsyncConnection(databasePath, Flags);
            Console.WriteLine($"Database path: {databasePath}");
        }
        private async Task Init()
        {
            await CreateTablesAsync();  // Create tables asynchronously
            await SeedDataAsync();
        }
        public async Task InitAsync()
        {
            if (_instance == null) return;

            Console.WriteLine("Starting Initialization...");
            TestDatabaseConnection();
            if (await IsDatabaseConnectedAsync())
            {
                await CreateTablesAsync();
                await SeedDataAsync();
            }
            else
            {
                Console.WriteLine("Database connection failed.");
            }
            Console.WriteLine("Initialization Complete.");
        }
        // Static method to access the single instance
        public static ApplicationDbContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ApplicationDbContext();
                }
                return _instance;
            }
        }
        private async Task CreateTablesAsync()
        {
            Console.WriteLine("Starting table creation...");

            var tableNames = new List<string>
    {
        "Vehicule",
        //"Auto",
        //"Moto",
        //"Velo",
        "Station",
        "Login",
        "AutoOption",
        "Reservation",
        "Membre"
    };

            foreach (var tableName in tableNames)
            {
                try
                {
                    Console.WriteLine($"Checking if {tableName} exists...");
                    if (!await TableExistsAsync(tableName))
                    {
                        Console.WriteLine($"Table {tableName} does not exist. Creating the table...");
                        switch (tableName)
                        {
                            case "Vehicule":
                                await _dbConnection.CreateTableAsync<Vehicule>();
                                Console.WriteLine("Vehicule table created.");
                                break;
                            case "Login":
                                await _dbConnection.CreateTableAsync<Login>();
                                Console.WriteLine("Login table created.");
                                break;
                            case "Auto":
                                await _dbConnection.CreateTableAsync<Auto>();
                                Console.WriteLine("Auto table created.");
                                break;
                            case "Moto":
                                await _dbConnection.CreateTableAsync<Moto>();
                                Console.WriteLine("Moto table created.");
                                break;
                            case "Velo":
                                await _dbConnection.CreateTableAsync<Velo>();
                                Console.WriteLine("Velo table created.");
                                break;
                            case "Station":
                                await _dbConnection.CreateTableAsync<Station>();
                                Console.WriteLine("Station table created.");
                                break;
                            case "AutoOption":
                                await _dbConnection.CreateTableAsync<AutoOption>();
                                Console.WriteLine("AutoOption table created.");
                                break;
                            case "Reservation":
                                await _dbConnection.CreateTableAsync<Reservation>();
                                Console.WriteLine("Reservation table created.");
                                break;
                            case "Membre":
                                await _dbConnection.CreateTableAsync<Membre>();
                                Console.WriteLine("Membre table created.");
                                break;
                            case "ReservationResult":
                                await _dbConnection.CreateTableAsync<ReservationResult>();
                                Console.WriteLine("ReservationResult table created.");
                                break;
                            case "ReservationSearch":
                                await _dbConnection.CreateTableAsync<ReservationSearch>();
                                Console.WriteLine("ReservationSearch table created.");
                                break;
                            default:
                                Console.WriteLine($"No logic to create table {tableName}");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Table {tableName} already exists.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating table {tableName}: {ex.Message}");
                }
            }

            Console.WriteLine("Table creation process completed.");
        }

        // Method to clear all data from the tables
        public async Task ClearAllTablesAsync()
        {
            // Clear data from each table (not the structure)
            await _dbConnection.DeleteAllAsync<Login>();
            await _dbConnection.DeleteAllAsync<Auto>();
            await _dbConnection.DeleteAllAsync<AutoOption>();
            await _dbConnection.DeleteAllAsync<Moto>();
            await _dbConnection.DeleteAllAsync<Station>();
            await _dbConnection.DeleteAllAsync<Vehicule>();
            await _dbConnection.DeleteAllAsync<Velo>();
            await _dbConnection.DeleteAllAsync<Reservation>();
            await _dbConnection.DeleteAllAsync<ReservationResult>();
            await _dbConnection.DeleteAllAsync<Membre>();
            await _dbConnection.DeleteAllAsync<ReservationSearch>();

            Console.WriteLine("All tables cleared.");
        }
        public async Task<bool> TableExistsAsync(string tableName)
        {
            try
            {
                var query = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";
                var result = await _dbConnection.ExecuteAsync(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if table {tableName} exists: {ex.Message}");
                return false;
            }
        }
        public async Task ListAllTablesAsync()
        {
            var query = "SELECT name FROM sqlite_master WHERE type='table';";
            var result = await _dbConnection.ExecuteAsync(query);

            if (result > 0)
            {
                Console.WriteLine("Tables in the database:");
                foreach (var table in query)
                {
                    Console.WriteLine(table);
                } // Handle the result accordingly
            }
        }
        public async Task DropAndRecreateTablesAsync()
        {
            try
            {
                // Drop each table if it exists
                await _dbConnection.DropTableAsync<Auto>();
                await _dbConnection.DropTableAsync<Moto>();
                await _dbConnection.DropTableAsync<Velo>();
                await _dbConnection.DropTableAsync<Vehicule>();
                await _dbConnection.DropTableAsync<AutoOption>();
                await _dbConnection.DropTableAsync<Station>();
                await _dbConnection.DropTableAsync<Reservation>();
                await _dbConnection.DropTableAsync<ReservationResult>();
                await _dbConnection.DropTableAsync<Membre>();
                await _dbConnection.DropTableAsync<ReservationSearch>();

                // Now recreate the tables
                await CreateTablesAsync(); // Ensure that CreateTablesAsync creates all the tables again

                Console.WriteLine("All tables dropped and recreated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resetting database: {ex.Message}");
            }
        }
        public async Task ResetDatabaseAsync()
        {
            // Drop tables and recreate them
            await DropAndRecreateTablesAsync();
            Console.WriteLine("Database has been reset.");
        }
        private async void ResetDatabase()
        {
            await _dbConnection.DropTableAsync<Velo>();   // Drop the table if it exists
            await _dbConnection.DropTableAsync<Moto>();   // Drop the table if it exists
            await _dbConnection.DropTableAsync<Auto>();   // Drop the table if it exists
            await _dbConnection.DropTableAsync<Vehicule>(); // Drop Vehicule if exists
        }
        public void DeleteDatabase()
        {
            string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RentARideDB.DbContext.db3");

            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
                Console.WriteLine($"Database file '{databasePath}' deleted successfully.");
            }
            else
            {
                Console.WriteLine($"Database file '{databasePath}' does not exist.");
            }
        }

        public async Task CreateTableForTypeAsync(Type entityType)
        {
            // Check if the type is abstract or lacks a parameterless constructor
            if (entityType.IsAbstract)
            {
                throw new InvalidOperationException("Cannot create a table for an abstract class.");
            }

            var constructorInfo = entityType.GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
            {
                throw new InvalidOperationException("TEntity must have a public parameterless constructor.");
            }

            // Create table for the provided type if valid
            await _dbConnection.CreateTableAsync(entityType);
        }
        public async Task CreateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            try
            {
                await _dbConnection.InsertAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting object: {ex.Message}");
            }
        }
        public async Task<int> AddorUpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            return await _dbConnection.InsertOrReplaceAsync(entity);
        }

        public async Task<int> UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            return await _dbConnection.UpdateAsync(entity);
        }
        public async Task<int> DeleteAsync<TEntity>(TEntity entity) where TEntity : class
        {
            return await _dbConnection.DeleteAsync(entity);
        }

        public async Task<List<T>> GetTableRows<T>(string tableName) where T : new()
        {
            try
            {
                // Get the type of the model dynamically using reflection
                var type = Type.GetType(nameSpace + "." + tableName);
                if (type == null)
                {
                    throw new ArgumentException($"The type for table {tableName} could not be found.");
                }

                // Prepare the SQL query
                string query = $"SELECT * FROM [{tableName}]";

                // Execute the query asynchronously and return the results cast to the desired type
                var result = await _dbConnection.QueryAsync<T>(query); // Pass T as the type argument here

                return result.ToList();
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, for example, log them
                Console.WriteLine($"Error in GetTableRows: {ex.Message}");
                return new List<T>(); // Return an empty list in case of an error
            }
        }
        public object GetTableRow(string tableName, string column, string value)
        {
            object[] obj = new object[] { };
            TableMapping map = new TableMapping(Type.GetType(nameSpace + tableName));
            string query = "SELECT * FROM " + tableName + " WHERE " + column + "='" + value + "'";

            return _dbConnection.QueryAsync(map, query, obj).Result.FirstOrDefault();

        }
        public async Task<List<Vehicule>> GetVehiculesAsync()
        {
            return await _dbConnection.Table<Vehicule>().ToListAsync();
        }

        public async Task<List<Station>> GetStationsAsync()
        {
            return await _dbConnection.Table<Station>().ToListAsync();
        }
        private async Task SeedDataAsync()
        {
            Console.WriteLine("SeedDataAsync() starting...");
            var vehiculeCount = await _dbConnection.Table<Vehicule>().CountAsync();
            if (vehiculeCount == 0)
            {
                await CreerVehicule("Auto", 1, "Essence", []);
                await CreerVehicule("Auto", 1, "Essence", ["MP3", "AC"]);
                await CreerVehicule("Auto", 1, "Essence", ["GPS", "AC", "MP3"]);
                await CreerVehicule("Auto", 1, "Essence", []);
                await CreerVehicule("Auto", 1, "Électrique", []);
                await CreerVehicule("Auto", 1, "Électrique", ["GPS", "MP3"]);
                await CreerVehicule("Auto", 2, "Essence", []);
                await CreerVehicule("Auto", 2, "Électrique", ["GPS", "AC"]);
                await CreerVehicule("Auto", 2, "Essence", ["GPS", "AC"]);
                await CreerVehicule("Auto", 2, "Essence", ["MP3", "AC"]);
                await CreerVehicule("Auto", 2, "Essence", ["GPS", "AC", "MP3"]);
                await CreerVehicule("Auto", 2, "Électrique", []);
                await CreerVehicule("Auto", 3, "Essence", []);
                await CreerVehicule("Auto", 3, "Essence", ["GPS", "MP3"]);
                await CreerVehicule("Auto", 3, "Électrique", []);
                await CreerVehicule("Auto", 4, "Essence", []);
                await CreerVehicule("Auto", 4, "Essence", ["GPS", "AC"]);
                await CreerVehicule("Auto", 4, "Électrique", []);
                await CreerVehicule("Auto", 5, "Essence", ["GPS", "AC", "MP3"]);
                await CreerVehicule("Auto", 5, "Essence", []);
                await CreerVehicule("Auto", 5, "Électrique", []);
                await CreerVehicule("Auto", 6, "Essence", []);
                await CreerVehicule("Auto", 6, "Électrique", ["GPS", "AC"]);
                await CreerVehicule("Auto", 6, "Électrique", ["GPS", "AC"]);
                await CreerVehicule("Auto", 6, "Essence", ["GPS", "AC"]);
                await CreerVehicule("Auto", 6, "Électrique", []);
                await CreerVehicule("Auto", 7, "Essence", []);
                await CreerVehicule("Auto", 7, "Essence", []);
                await CreerVehicule("Auto", 7, "Électrique", ["AC", "ChildSeat"]);
                await CreerVehicule("Auto", 7, "Essence", ["GPS", "MP3"]);
                await CreerVehicule("Auto", 7, "Électrique", ["GPS", "AC"]);
                await CreerVehicule("Auto", 7, "Électrique", []);
                await CreerVehicule("Auto", 8, "Essence", []);
                await CreerVehicule("Auto", 8, "Essence", ["MP3", "AC"]);
                await CreerVehicule("Auto", 8, "Essence", ["GPS", "AC", "MP3"]);
                await CreerVehicule("Auto", 8, "Électrique", []);
                await CreerVehicule("Auto", 9, "Essence", []);
                await CreerVehicule("Auto", 9, "Essence", ["GPS", "MP3"]);
                await CreerVehicule("Auto", 9, "Électrique", ["GPS", "AC"]);
                await CreerVehicule("Auto", 9, "Électrique", ["GPS", "AC"]);
                await CreerVehicule("Auto", 9, "Essence", ["GPS", "AC"]);
                await CreerVehicule("Auto", 9, "Électrique", []);
                await CreerVehicule("Auto", 10, "Essence", []);
                await CreerVehicule("Auto", 10, "Essence", []);
                await CreerVehicule("Auto", 10, "Électrique", ["AC", "ChildSeat"]);
                await CreerVehicule("Auto", 10, "Essence", ["GPS", "MP3"]);
                await CreerVehicule("Auto", 10, "Électrique", ["GPS", "AC"]);
                await CreerVehicule("Auto", 10, "Électrique", []);
                await CreerVehicule("Auto", 11, "Essence", []);
                await CreerVehicule("Auto", 11, "Essence", ["MP3", "AC"]);
                await CreerVehicule("Auto", 11, "Essence", ["GPS", "AC", "MP3"]);
                await CreerVehicule("Auto", 11, "Essence", []);
                await CreerVehicule("Auto", 11, "Électrique", ["AC", "ChildSeat"]);
                await CreerVehicule("Auto", 11, "Essence", ["GPS", "MP3"]);
                await CreerVehicule("Auto", 11, "Électrique", []);
                await CreerVehicule("Auto", 12, "Essence", []);
                await CreerVehicule("Auto", 12, "Essence", ["GPS", "AC"]);
                await CreerVehicule("Auto", 12, "Essence", ["MP3", "AC"]);
                await CreerVehicule("Auto", 12, "Essence", ["GPS", "AC", "MP3"]);
                await CreerVehicule("Auto", 12, "Électrique", []);
                await CreerVehicule("Auto", 13, "Essence", []);
                await CreerVehicule("Auto", 13, "Essence", ["GPS", "MP3"]);
                await CreerVehicule("Auto", 13, "Électrique", ["GPS", "AC"]);
                await CreerVehicule("Auto", 13, "Électrique", ["GPS", "AC"]);
                await CreerVehicule("Auto", 13, "Essence", ["GPS", "AC"]);
                await CreerVehicule("Auto", 13, "Électrique", []);
                await CreerVehicule("Auto", 14, "Essence", []);
                await CreerVehicule("Auto", 14, "Essence", []);
                await CreerVehicule("Auto", 14, "Électrique", []);
                await CreerVehicule("Auto", 14, "Essence", []);
                await CreerVehicule("Auto", 15, "Électrique", ["GPS", "AC"]);
                await CreerVehicule("Auto", 15, "Électrique", ["GPS", "AC"]);
                await CreerVehicule("Auto", 15, "Électrique", ["GPS", "AC"]);
                await CreerVehicule("Auto", 15, "Électrique", []);

                await CreerVehicule("Velo", 1);
                await CreerVehicule("Velo", 2);
                await CreerVehicule("Velo", 3);
                await CreerVehicule("Velo", 4);
                await CreerVehicule("Velo", 5);
                await CreerVehicule("Velo", 6);
                await CreerVehicule("Velo", 7);
                await CreerVehicule("Velo", 8);
                await CreerVehicule("Velo", 9);
                await CreerVehicule("Velo", 10);
                await CreerVehicule("Velo", 11);
                await CreerVehicule("Velo", 12);
  
                await CreerVehicule("Moto", 1);
                await CreerVehicule("Moto", 1);
                await CreerVehicule("Moto", 2);
                await CreerVehicule("Moto", 2);
                await CreerVehicule("Moto", 3);
                await CreerVehicule("Moto", 5);
                await CreerVehicule("Moto", 6);
                await CreerVehicule("Moto", 7);
                await CreerVehicule("Moto", 7);
                await CreerVehicule("Moto", 9);
                await CreerVehicule("Moto", 11);
                await CreerVehicule("Moto", 13);
                await CreerVehicule("Moto", 13);
                await CreerVehicule("Moto", 15);
                await CreerVehicule("Moto", 15);
                await CreerVehicule("Moto", 15);
            }
            var stationCount = await _dbConnection.Table<Station>().CountAsync();
            if (stationCount == 0)
            {
                await CreerStation("Dorchester-Charest", 10, 2);
                await CreerStation("Carre D'Youville", 10, 2);
                await CreerStation("Limoilou", 5, 1);
                await CreerStation("Saint-Roch", 4, 1);
                await CreerStation("Beauport", 5, 1);
                await CreerStation("Vanier", 8, 2);
                await CreerStation("Vieux-Quebec - Plaines d'Abraham", 10, 2);
                await CreerStation("Vieux-Quebec - St-Jean", 6, 2);
                await CreerStation("Charlesbourg", 9, 2);
                await CreerStation("ULaval", 8, 2);
                await CreerStation("Sainte-Foy", 9, 1);
                await CreerStation("Sillery", 8, 3);
                await CreerStation("Levis", 10, 2);
                await CreerStation("Cap-Rouge", 6, 3);
                await CreerStation("Chutes Montmorency", 10, 1);
            }
            var reservationCount = await _dbConnection.Table<Reservation>().CountAsync();
            if (reservationCount == 0)
            {
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0).AddMinutes(0), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0).AddMinutes(0), DateTime.Today.AddDays(0).AddHours(3).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), DateTime.Today.AddDays(0).AddHours(5).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(3).AddMinutes(30), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(5).AddMinutes(0), DateTime.Today.AddDays(0).AddHours(7).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(3), DateTime.Today.AddDays(0).AddHours(4).AddMinutes(30), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(1).AddHours(1), DateTime.Today.AddDays(1).AddHours(3).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(2).AddHours(3), DateTime.Today.AddDays(2).AddHours(4).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(30), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0).AddHours(30), DateTime.Today.AddDays(0).AddHours(1).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(1).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(1).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0), DateTime.Today.AddDays(0).AddHours(1).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(30), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(2), DateTime.Today.AddDays(0).AddHours(5).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(1).AddHours(0), DateTime.Today.AddDays(1).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(1).AddHours(2), DateTime.Today.AddDays(1).AddHours(3).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0), DateTime.Today.AddDays(0).AddHours(3).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(5).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(7).AddMinutes(30), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(5), DateTime.Today.AddDays(0).AddHours(8).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(3).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(5), DateTime.Today.AddDays(0).AddHours(7).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(3), DateTime.Today.AddDays(0).AddHours(4).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(1).AddHours(2), DateTime.Today.AddDays(1).AddHours(5).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(5).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", DateTime.Today.AddDays(2).AddHours(0), DateTime.Today.AddDays(2).AddHours(2).AddMinutes(0), new Vehicule("Auto", 1, "Essence", []));
                // Past Reservations
                await CreerReservation("MEM007", new DateTime(2025, 03, 11, 10, 30, 0), new DateTime(2025, 03, 11, 11, 30, 0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM007", new DateTime(2025, 03, 15, 14, 0, 0), new DateTime(2025, 03, 15, 16, 30, 0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM007", new DateTime(2025, 03, 17, 10, 00, 0), new DateTime(2025, 03, 17, 11, 30, 0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM001", new DateTime(2025, 03, 19, 10, 30, 0), new DateTime(2025, 03, 11, 11, 30, 0), new Vehicule("Auto", 1, "Essence", []));
                await CreerReservation("MEM005", new DateTime(2025, 03, 20, 10, 30, 0), new DateTime(2025, 03, 11, 11, 30, 0), new Vehicule("Auto", 1, "Essence", []));
            }
            Console.WriteLine("SeedDataAsync() completed!");
        }
        public async Task<bool> IsDatabaseConnectedAsync()
        {
            try
            {
                // Test the connection with a simple query
                var result = await _dbConnection.ExecuteScalarAsync<int>("SELECT 1");
                return result == 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection error: {ex.Message}");
                return false;
            }
        }
        public async Task TestDatabaseConnection()
        {
            try
            {
                Console.WriteLine("Testing database connection...");
                var result = await _dbConnection.ExecuteScalarAsync<int>("SELECT 1");
                Console.WriteLine($"Database test successful. Result: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error testing database connection: {ex.Message}");
            }
        }
        public async Task CreerVehicule(string type, int stationID, string categorie = null, List<string> carOptions = null)
        {
            if (type == "Auto")
            {
                // Create the vehicule
                var vehicule = new Vehicule(type, stationID, categorie, carOptions);
                Console.WriteLine(string.Join(", ", carOptions));
                // Create the auto
                //var vehicule1 = new Auto(stationID, categorie, carOptions);
                // Insert the auto in the database
                await CreateAsync(vehicule);
                Console.WriteLine($"Inserted {vehicule.type} {vehicule.categorieAuto} with Id: {vehicule.vehiculeId}, at station : {vehicule.vehiculeStationId}");
                if (carOptions != null && carOptions.Count > 0)
                {
                    foreach (var option in carOptions)
                    {
                        var autoOption = new AutoOption
                        {
                            Option = option,
                            AutoId = vehicule.vehiculeId  // Foreign key reference to the Auto
                        };
                        try
                        {
                            Console.WriteLine($"Inserted the option {autoOption.Id}: {autoOption.Option} for the auto with ID : {autoOption.AutoId}");
                            await CreateAsync(autoOption);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error inserting {vehicule.type}: {ex.Message}");
                        }
                    }
                }
                else
                Console.WriteLine($"with no options."); 
            }
            else
            {
                var vehicule = new Vehicule(type, stationID);
                await CreateAsync(vehicule);
                Console.WriteLine($"Inserted {vehicule.type} with Id: {vehicule.vehiculeId} at station {vehicule.vehiculeStationId}");
            }
        }
        public async Task creerMembre(string name, string password, string email)
        {
            await CreateAsync(new Membre(name, password, email));
        }
        public async Task CreerStation(string address, int spaces, int bikeSpaces)
        {
            var station = new Station(address, spaces, bikeSpaces);
            await CreateAsync(station);
            Console.WriteLine($"Inserted station with Id: {station.StationId} with address : {station.StationAddress}");
        }
        public async Task CreerReservation(string memberid, DateTime requestedStartTime, DateTime requestedEndTime, Vehicule vehicule)
        {
            var reservation = new Reservation(memberid, requestedStartTime, requestedEndTime, vehicule);
            await CreateAsync(reservation);
            // Format the AutoOptions list to show their details
            // Check if AutoOptions is not null and contains items
            if (vehicule.AutoOptions != null && vehicule.AutoOptions.Any())
            {
                // Format the AutoOptions list to show their details
                string optionsString = string.Join(", ", vehicule.AutoOptions.Select(option => option.ToString()));

                Console.WriteLine($"Inserted reservation with Id: {reservation.ReservationID}, Start: {reservation.StartTime}, End: {reservation.EndTime}, at station {reservation.StationId} for a {vehicule.type}, {vehicule.categorieAuto}, with options: {vehicule.AutoOptions}");
            }
            else
            {
                Console.WriteLine($"Inserted reservation with Id: {reservation.ReservationID}, Start: {reservation.StartTime}, End: {reservation.EndTime}, at station {reservation.StationId} for a {vehicule.type}, {vehicule.categorieAuto}, with no options.");
            }
        }
    }
}


using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentARideDB.Models;
using System.Collections.ObjectModel;


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
            if (_dbConnection == null) return;

            await CreateTablesAsync();  // Create tables asynchronously
            await SeedDataAsync();      // Optionally seed data if required
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
            await _dbConnection.CreateTableAsync<Vehicule>(); // Create the Vehicule table
            await _dbConnection.CreateTableAsync<Auto>(); // Create the Auto table
            await _dbConnection.CreateTableAsync<Moto>(); // Create the Moto table
            await _dbConnection.CreateTableAsync<Velo>(); // Create the Velo table
            await _dbConnection.CreateTableAsync<Station>();
            await _dbConnection.CreateTableAsync<Login>();
            await _dbConnection.CreateTableAsync<AutoOption>();
            await _dbConnection.CreateTableAsync<Reservation>();
            await _dbConnection.CreateTableAsync<ReservationResult>();
            await _dbConnection.CreateTableAsync<Membre>();
            await _dbConnection.CreateTableAsync<ReservationSearch>();
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
            var query = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";
            var result = await _dbConnection.ExecuteAsync(query);
            return result > 0;
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
        public async Task<int> CreateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            return await _dbConnection.InsertAsync(entity);

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
            var autoCount = await _dbConnection.Table<Auto>().CountAsync();
            if (autoCount == 0)
            {
                CreerVehicule("Auto", 001, 001, "Essence", []);
                CreerVehicule("Auto", 001, 001, "Essence", ["MP3", "AC"]);
                CreerVehicule("Auto", 001, 001, "Essence", ["GPS", "AC", "MP3"]);
                CreerVehicule("Auto", 001, 001, "Essence", []);
                CreerVehicule("Auto", 001, 001, "Électrique", []);
                CreerVehicule("Auto", 001, 001, "Électrique", ["GPS", "MP3"]);
                CreerVehicule("Auto", 001, 002, "Essence", []);
                CreerVehicule("Auto", 001, 002, "Électrique", ["GPS", "AC"]);
                CreerVehicule("Auto", 001, 002, "Essence", ["GPS", "AC"]);
                CreerVehicule("Auto", 001, 002, "Essence", ["MP3", "AC"]);
                CreerVehicule("Auto", 001, 002, "Essence", ["GPS", "AC", "MP3"]);
                CreerVehicule("Auto", 001, 002, "Électrique", []);
                CreerVehicule("Auto", 001, 003, "Essence", []);
                CreerVehicule("Auto", 001, 003, "Essence", ["GPS", "MP3"]);
                CreerVehicule("Auto", 001, 003, "Électrique", []);
                CreerVehicule("Auto", 001, 004, "Essence", []);
                CreerVehicule("Auto", 001, 004, "Essence", ["GPS", "AC"]);
                CreerVehicule("Auto", 001, 004, "Électrique", []);
                CreerVehicule("Auto", 001, 005, "Essence", ["GPS", "AC", "MP3"]);
                CreerVehicule("Auto", 001, 005, "Essence", []);
                CreerVehicule("Auto", 001, 005, "Électrique", []);
                CreerVehicule("Auto", 001, 006, "Essence", []);
                CreerVehicule("Auto", 001, 006, "Électrique", ["GPS", "AC"]);
                CreerVehicule("Auto", 001, 006, "Électrique", ["GPS", "AC"]);
                CreerVehicule("Auto", 001, 006, "Essence", ["GPS", "AC"]);
                CreerVehicule("Auto", 001, 006, "Électrique", []);
                CreerVehicule("Auto", 001, 007, "Essence", []);
                CreerVehicule("Auto", 001, 007, "Essence", []);
                CreerVehicule("Auto", 001, 007, "Électrique", ["AC", "ChildSeat"]);
                CreerVehicule("Auto", 001, 007, "Essence", ["GPS", "MP3"]);
                CreerVehicule("Auto", 001, 007, "Électrique", ["GPS", "AC"]);
                CreerVehicule("Auto", 001, 007, "Électrique", []);
                CreerVehicule("Auto", 001, 008, "Essence", []);
                CreerVehicule("Auto", 001, 008, "Essence", ["MP3", "AC"]);
                CreerVehicule("Auto", 001, 008, "Essence", ["GPS", "AC", "MP3"]);
                CreerVehicule("Auto", 001, 008, "Électrique", []);
                CreerVehicule("Auto", 001, 009, "Essence", []);
                CreerVehicule("Auto", 001, 009, "Essence", ["GPS", "MP3"]);
                CreerVehicule("Auto", 001, 009, "Électrique", ["GPS", "AC"]);
                CreerVehicule("Auto", 001, 009, "Électrique", ["GPS", "AC"]);
                CreerVehicule("Auto", 001, 009, "Essence", ["GPS", "AC"]);
                CreerVehicule("Auto", 001, 009, "Électrique", []);
                CreerVehicule("Auto", 001, 010, "Essence", []);
                CreerVehicule("Auto", 001, 010, "Essence", []);
                CreerVehicule("Auto", 001, 010, "Électrique", ["AC", "ChildSeat"]);
                CreerVehicule("Auto", 001, 010, "Essence", ["GPS", "MP3"]);
                CreerVehicule("Auto", 001, 010, "Électrique", ["GPS", "AC"]);
                CreerVehicule("Auto", 001, 010, "Électrique", []);
                CreerVehicule("Auto", 001, 011, "Essence", []);
                CreerVehicule("Auto", 001, 011, "Essence", ["MP3", "AC"]);
                CreerVehicule("Auto", 001, 011, "Essence", ["GPS", "AC", "MP3"]);
                CreerVehicule("Auto", 001, 011, "Essence", []);
                CreerVehicule("Auto", 001, 011, "Électrique", ["AC", "ChildSeat"]);
                CreerVehicule("Auto", 001, 011, "Essence", ["GPS", "MP3"]);
                CreerVehicule("Auto", 001, 011, "Électrique", []);
                CreerVehicule("Auto", 001, 012, "Essence", []);
                CreerVehicule("Auto", 001, 012, "Essence", ["GPS", "AC"]);
                CreerVehicule("Auto", 001, 012, "Essence", ["MP3", "AC"]);
                CreerVehicule("Auto", 001, 012, "Essence", ["GPS", "AC", "MP3"]);
                CreerVehicule("Auto", 001, 012, "Électrique", []);
                CreerVehicule("Auto", 001, 013, "Essence", []);
                CreerVehicule("Auto", 001, 013, "Essence", ["GPS", "MP3"]);
                CreerVehicule("Auto", 001, 013, "Électrique", ["GPS", "AC"]);
                CreerVehicule("Auto", 001, 013, "Électrique", ["GPS", "AC"]);
                CreerVehicule("Auto", 001, 013, "Essence", ["GPS", "AC"]);
                CreerVehicule("Auto", 001, 013, "Électrique", []);
                CreerVehicule("Auto", 001, 014, "Essence", []);
                CreerVehicule("Auto", 001, 014, "Essence", []);
                CreerVehicule("Auto", 001, 014, "Électrique", []);
                CreerVehicule("Auto", 001, 014, "Essence", []);
                CreerVehicule("Auto", 001, 015, "Électrique", ["GPS", "AC"]);
                CreerVehicule("Auto", 001, 015, "Électrique", ["GPS", "AC"]);
                CreerVehicule("Auto", 001, 015, "Électrique", ["GPS", "AC"]);
                CreerVehicule("Auto", 001, 015, "Électrique", []);
            }
            var veloCount = await _dbConnection.Table<Velo>().CountAsync();
            if (veloCount == 0)
            {
                CreerVehicule("Velo", 001, 001);
                CreerVehicule("Velo", 002, 002);
                CreerVehicule("Velo", 003, 003);
                CreerVehicule("Velo", 004, 004);
                CreerVehicule("Velo", 005, 005);
                CreerVehicule("Velo", 006, 006);
                CreerVehicule("Velo", 007, 007);
                CreerVehicule("Velo", 008, 008);
                CreerVehicule("Velo", 009, 009);
                CreerVehicule("Velo", 010, 010);
                CreerVehicule("Velo", 011, 011);
                CreerVehicule("Velo", 012, 012);
            }
            var motoCount = await _dbConnection.Table<Moto>().CountAsync();
            if (motoCount == 0)
            {
                CreerVehicule("Moto", 001, 001);
                CreerVehicule("Moto", 002, 001);
                CreerVehicule("Moto", 003, 002);
                CreerVehicule("Moto", 004, 002);
                CreerVehicule("Moto", 005, 003);
                CreerVehicule("Moto", 006, 005);
                CreerVehicule("Moto", 007, 006);
                CreerVehicule("Moto", 008, 007);
                CreerVehicule("Moto", 009, 007);
                CreerVehicule("Moto", 010, 009);
                CreerVehicule("Moto", 011, 011);
                CreerVehicule("Moto", 012, 013);
                CreerVehicule("Moto", 013, 013);
                CreerVehicule("Moto", 014, 015);
                CreerVehicule("Moto", 015, 015);
                CreerVehicule("Moto", 016, 015);
            }
            var stationCount = await _dbConnection.Table<Station>().CountAsync();
            if (stationCount == 0)
            {
                CreerStation("Dorchester-Charest", 10, 2);
                CreerStation("Carre D'Youville", 10, 2);
                CreerStation("Limoilou", 5, 1);
                CreerStation("Saint-Roch", 4, 1);
                CreerStation("Beauport", 5, 1);
                CreerStation("Vanier", 8, 2);
                CreerStation("Vieux-Quebec - Plaines d'Abraham", 10, 2);
                CreerStation("Vieux-Quebec - St-Jean", 6, 2);
                CreerStation("Charlesbourg", 9, 2);
                CreerStation("ULaval", 8, 2);
                CreerStation("Sainte-Foy", 9, 1);
                CreerStation("Sillery", 8, 3);
                CreerStation("Levis", 10, 2);
                CreerStation("Cap-Rouge", 6, 3);
                CreerStation("Chutes Montmorency", 10, 1);
            }
            var reservationCount = await _dbConnection.Table<Reservation>().CountAsync();
            if (reservationCount == 0)
            {
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0).AddMinutes(0), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(001, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0).AddMinutes(0), DateTime.Today.AddDays(0).AddHours(3).AddMinutes(0), new Auto(001, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), DateTime.Today.AddDays(0).AddHours(5).AddMinutes(0), new Auto(001, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(3).AddMinutes(30), new Auto(002, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(5).AddMinutes(0), DateTime.Today.AddDays(0).AddHours(7).AddMinutes(0), new Auto(006, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(3), DateTime.Today.AddDays(0).AddHours(4).AddMinutes(30), new Auto(006, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(1).AddHours(1), DateTime.Today.AddDays(1).AddHours(3).AddMinutes(0), new Auto(006, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(2).AddHours(3), DateTime.Today.AddDays(2).AddHours(4).AddMinutes(0), new Auto(006, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(007, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(30), new Auto(008, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0).AddHours(30), DateTime.Today.AddDays(0).AddHours(1).AddMinutes(0), new Auto(009, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(1).AddMinutes(0), new Auto(013, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(1).AddMinutes(0), new Auto(015, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(011, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(011, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(012, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(009, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(009, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(006, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(006, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(006, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(007, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(007, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(009, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(010, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(010, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(010, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(010, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(010, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0), DateTime.Today.AddDays(0).AddHours(1).AddMinutes(0), new Auto(001, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(30), new Auto(015, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(001, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(2), DateTime.Today.AddDays(0).AddHours(5).AddMinutes(0), new Auto(003, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(1).AddHours(0), DateTime.Today.AddDays(1).AddHours(2).AddMinutes(0), new Auto(003, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(1).AddHours(2), DateTime.Today.AddDays(1).AddHours(3).AddMinutes(0), new Auto(013, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(015, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0), DateTime.Today.AddDays(0).AddHours(3).AddMinutes(0), new Auto(001, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(5).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(7).AddMinutes(30), new Auto(001, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(003, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(5), DateTime.Today.AddDays(0).AddHours(8).AddMinutes(0), new Auto(003, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(3).AddMinutes(0), new Auto(006, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(007, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(008, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(009, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(5), DateTime.Today.AddDays(0).AddHours(7).AddMinutes(0), new Auto(009, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto(010, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(3), DateTime.Today.AddDays(0).AddHours(4).AddMinutes(0), new Auto(010, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(1).AddHours(2), DateTime.Today.AddDays(1).AddHours(5).AddMinutes(0), new Auto(011, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(5).AddMinutes(0), new Auto(012, "Essence", []));
                CreerReservation("MEM001", DateTime.Today.AddDays(2).AddHours(0), DateTime.Today.AddDays(2).AddHours(2).AddMinutes(0), new Auto(012, "Essence", []));
                // Past Reservations
                CreerReservation("MEM007", new DateTime(2025, 03, 11, 10, 30, 0), new DateTime(2025, 03, 11, 11, 30, 0), new Auto(001, "Essence", []));
                CreerReservation("MEM007", new DateTime(2025, 03, 15, 14, 0, 0), new DateTime(2025, 03, 15, 16, 30, 0), new Auto(002, "Essence", []));
                CreerReservation("MEM007", new DateTime(2025, 03, 17, 10, 00, 0), new DateTime(2025, 03, 17, 11, 30, 0), new Auto(015, "Essence", []));
                CreerReservation("MEM001", new DateTime(2025, 03, 19, 10, 30, 0), new DateTime(2025, 03, 11, 11, 30, 0), new Auto(001, "Essence", []));
                CreerReservation("MEM005", new DateTime(2025, 03, 20, 10, 30, 0), new DateTime(2025, 03, 11, 11, 30, 0), new Auto(001, "Essence", []));
            }
        }
        public async void CreerVehicule(string type, int vehiculeID, int stationID, string categorie = null, List<string> carOptions = null)
        {
            if (type == "Auto")
            {
                // Create the auto
                var vehicule = new Auto(stationID, categorie, carOptions);
                // Insert the auto in the database
                await CreateAsync(vehicule);
                Console.WriteLine($"Inserted Vehicule with Id: {vehicule.vehiculeId}");
                // Insert AutoOptions into the AutoOption table
                foreach (var option in carOptions)
                {
                    var autoOption = new AutoOption
                    {
                        Option = option,
                        AutoId = vehicule.vehiculeId  // Foreign key reference to the Auto
                    };
                    await CreateAsync(autoOption);
                }
            }
            else if (type == "Velo")
            {
                var vehicule = new Velo(stationID);
                await CreateAsync(vehicule);
                Console.WriteLine($"Inserted Vehicule with Id: {vehicule.vehiculeId}");
            }
            else if (type == "Moto")
            {
                var vehicule = new Moto(stationID);
                await CreateAsync(vehicule);
                Console.WriteLine($"Inserted Vehicule with Id: {vehicule.vehiculeId}");

            }
        }
        public async void creerMembre(string name, string password, string email)
        {
            await CreateAsync(new Membre(name, password, email));
        }
        public async void CreerStation(string address, int spaces, int bikeSpaces)
        {
            await CreateAsync(new Station(address, spaces, bikeSpaces));
        }
        public async void CreerReservation(string memberid, DateTime requestedStartTime, DateTime requestedEndTime, Vehicule vehicule)
        {
            await CreateAsync(new Reservation(memberid, requestedStartTime, requestedEndTime, vehicule));
        }
    }
}


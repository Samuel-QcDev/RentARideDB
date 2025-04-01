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

        // Application Database
        public readonly static string nameSpace = "RentARideApp.DbContext.";

        public const string DatabaseFilename = "RentARideApp.DbContext.db3";

        public static string dtabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        private static ApplicationDbContext _instance;
        public ObservableCollection<Reservation> ReservationsResultPast { get; set; }
        public ObservableCollection<Reservation> ReservationsResultCurrent { get; set; }


        public ApplicationDbContext()
        {


            if (_dbConnection == null)
            {
                _dbConnection = new SQLiteAsyncConnection(dtabasePath, Flags);
                CreateTablesAsync();  // Create tables asynchronously
            }
            
        }
        private async void CreateTablesAsync()
        {
            await _dbConnection.CreateTableAsync<Login>();
            await _dbConnection.CreateTableAsync<Auto>();
            await _dbConnection.CreateTableAsync<AutoOption>();
            await _dbConnection.CreateTableAsync<Moto>();
            //await _dbConnection.CreateTableAsync<Station>();  // Need to create a new table for the list of selectedStationId
            await _dbConnection.CreateTableAsync<Vehicule>();
            await _dbConnection.CreateTableAsync<Velo>();
            await _dbConnection.CreateTableAsync<Reservation>();
            //await _dbConnection.CreateTableAsync<ReservationResult>();
            await _dbConnection.CreateTableAsync<Membre>();
            await _dbConnection.CreateTableAsync<ReservationSearch>();
        }
        private async Task Init()
        {

        }
        public async Task<SQLite.CreateTableResult> CreateTableAsync<TEntity>() where TEntity : class
        {
            return await _dbConnection.CreateTableAsync(typeof(TEntity));
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

        public List<T> GetTableRows<T>(string tableName)
        {
            object[] obj = new object[] { };
            TableMapping map = new TableMapping(Type.GetType(nameSpace + tableName));
            string query = "SELECT * FROM [" + tableName + "]";

            return _dbConnection.QueryAsync(map, query, obj).Result.Cast<T>().ToList();
        }

        public object GetTableRow(string tableName, string column, string value)
        {
            object[] obj = new object[] { };
            TableMapping map = new TableMapping(Type.GetType(nameSpace + tableName));
            string query = "SELECT * FROM " + tableName + " WHERE " + column + "='" + value + "'";

            return _dbConnection.QueryAsync(map, query, obj).Result.FirstOrDefault();

        }
    }
}


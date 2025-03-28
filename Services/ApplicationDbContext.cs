using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//namespace RentARide.DbContext
//{
//    public class ApplicationDbContext
//    {
//        public SQLiteAsyncConnection _dbConnection;

//        // Application Database
//        public readonly static string nameSpace = "RentARideApp.DbContext.";

//        public const string DatabaseFilename = "RentARideApp.DbContext.db3";

//        public static string dtabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

//        public const SQLite.SQLiteOpenFlags Flags =
//            // open the database in read/write mode
//            SQLite.SQLiteOpenFlags.ReadWrite |
//            // create the database if it doesn't exist
//            SQLite.SQLiteOpenFlags.Create |
//            // enable multi-threaded database access
//            SQLite.SQLiteOpenFlags.SharedCache;

//        public ApplicationDbContext()
//        {
//            {
//                if (_dbConnection == null)
//                {
//                    _dbConnection = new SQLiteAsyncConnection(dtabasePath, Flags);
//                    //_dbConnection.CreateTableAsync<Login>();
//                    //_dbConnection.CreateTableAsync<Auto>();
//                    //_dbConnection.CreateTableAsync<Moto>();
//                    //_dbConnection.CreateTableAsync<Station>();
//                    //_dbConnection.CreateTableAsync<Vehicule>();
//                    //_dbConnection.CreateTableAsync<Velo>();

//                }

//            }

//        }

//        private async Task Init()
//        {

//        }
//        //public async Task<int> CreateTableAsync<TEntity>(TEntity entity) where TEntity : class
//        //{
//        //    return await _dbConnection.CreateTable(entity);
//        //}
//        public async Task<int> CreateAsync<TEntity>(TEntity entity) where TEntity : class
//        {
//            return await _dbConnection.InsertAsync(entity);
//        }

//        public async Task<int> AddorUpdateAsync<TEntity>(TEntity entity) where TEntity : class
//        {
//            return await _dbConnection.InsertOrReplaceAsync(entity);
//        }

//        public async Task<int> UpdateAsync<TEntity>(TEntity entity) where TEntity : class
//        {
//            return await _dbConnection.UpdateAsync(entity);
//        }
//        public async Task<int> DeleteAsync<TEntity>(TEntity entity) where TEntity : class
//        {
//            return await _dbConnection.DeleteAsync(entity);
//        }

//        public List<T> GetTableRows<T>(string tableName)
//        {
//            object[] obj = new object[] { };
//            TableMapping map = new TableMapping(Type.GetType(nameSpace + tableName));
//            string query = "SELECT * FROM [" + tableName + "]";

//            return _dbConnection.QueryAsync(map, query, obj).Result.Cast<T>().ToList();
//        }

//        public object GetTableRow(string tableName, string column, string value)
//        {
//            object[] obj = new object[] { };
//            TableMapping map = new TableMapping(Type.GetType(nameSpace + tableName));
//            string query = "SELECT * FROM " + tableName + " WHERE " + column + "='" + value + "'";

//            return _dbConnection.QueryAsync(map, query, obj).Result.FirstOrDefault();

//        }
//    }
//}


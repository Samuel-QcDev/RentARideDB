using System.Runtime.Intrinsics.X86;
using RentARideDB.ViewModel;
using RentARideDB.Services;
using RentARideDB.Models;
using SQLite;
using Xunit;

namespace RentARideDBTest
{
    public class UnitTest2
    {
    
        private SQLiteAsyncConnection _db;

        public UnitTest2()
        {
            // Setup in-memory DB
            _db = new SQLiteAsyncConnection(":memory:");
            _db.CreateTableAsync<Vehicule>().Wait();
            _db.CreateTableAsync<AutoOption>().Wait();
        }

        private async Task CreateAsync<T>(T item)
        {
            await _db.InsertAsync(item);
        }

        [Fact]
        public async Task CreerVehicule_InsertsVehiculeAndOptions()
        {
            // Arrange
            var type = "Auto";
            var stationID = 1;
            var categorie = "Essence";
            var options = new List<string> { "GPS", "MP3" };

            var vehicule = new Vehicule(type, stationID, categorie, options);
            await CreateAsync(vehicule);

            foreach (var opt in options)
            {
                var autoOption = new AutoOption
                {
                    Option = opt,
                    AutoId = vehicule.vehiculeId
                };
                await CreateAsync(autoOption);
            }

            // Act
            var insertedVehicule = await _db.Table<Vehicule>().FirstOrDefaultAsync();
            var insertedOptions = await _db.Table<AutoOption>().ToListAsync();

            // Assert
            Assert.NotNull(insertedVehicule);
            Assert.Equal("Auto", insertedVehicule.type);
            Assert.Equal(2, insertedOptions.Count);
            Assert.Contains(insertedOptions, o => o.Option == "GPS");
            Assert.Contains(insertedOptions, o => o.Option == "MP3");
        }
    }
}

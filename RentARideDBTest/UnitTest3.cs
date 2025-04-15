using Xunit;
using RentARideDB.Models;
using RentARideDB.ViewModel;
using SQLite;
using RentARideDB.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace RentARideDBTest
{
    public class ReservationSearchViewModelTests
    {
        private SQLiteAsyncConnection _dbConnection;
        private ApplicationDbContext _dbContext;
        private ReservationSearchViewModel _viewModel;

        public ReservationSearchViewModelTests()
        {
            // Set up in-memory database
            _dbConnection = new SQLiteAsyncConnection(":memory:");
            _dbConnection.CreateTableAsync<Vehicule>().Wait();
            _dbConnection.CreateTableAsync<AutoOption>().Wait();
            _dbConnection.CreateTableAsync<Station>().Wait();
            _dbConnection.CreateTableAsync<Reservation>().Wait();

            // Set up fake DbContext with our in-memory DB
            _dbContext = new ApplicationDbContext(_dbConnection);

            // Set up ViewModel
            _viewModel = new ReservationSearchViewModel(_dbContext)
            {
                ReservationSearchDetails = new ReservationSearch
                {
                    TypeVehicule = "Auto",
                    StationAddress = "All Stations",
                    RequestedStartTime = DateTime.Now,
                    RequestedEndTime = DateTime.Now.AddHours(2)
                },
                CategorieAuto = "Essence",
                IsCheckedGPS = true,
                IsCheckedMP3 = true
            };

            // Insert seed data
            SetupTestData().Wait();
        }

        private async Task SetupTestData()
        {
            var station = new Station { StationId = 1, StationAddress = "All Stations" };
            await _dbConnection.InsertAsync(station);

            var vehicule = new Vehicule
            {
                vehiculeId = 1,
                type = "Auto",
                categorieAuto = "Essence",
                vehiculeStationId = 1
            };
            await _dbConnection.InsertAsync(vehicule);

            var options = new[]
            {
            new AutoOption { AutoId = vehicule.vehiculeId, Option = "GPS" },
            new AutoOption { AutoId = vehicule.vehiculeId, Option = "MP3" }
        };
            foreach (var option in options)
            {
                await _dbConnection.InsertAsync(option);
            }
        }

        [Fact]
        public async Task AddVehiculesBasedOnAllUserInputs_AddsMatchingVehicule()
        {
            // Act
            var result = await _viewModel.AddVehiculesBasedOnAllUserInputs();


            // Assert
            Assert.True(result);
            Assert.Single(_viewModel.Vehicules);
            Assert.Equal("Auto", _viewModel.Vehicules.First().type);
        }
    }
}

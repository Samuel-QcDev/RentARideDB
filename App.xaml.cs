using RentARideDB.Models;
using RentARideDB.Services;

namespace RentARideDB;

public partial class App : Application
{
    private readonly ApplicationDbContext _dbContext;
    public App()
    {

        InitializeComponent();
        // Initialize database and create tables before setting MainPage
        var dbContext = ApplicationDbContext.Instance;
        dbContext.InitAsync().ConfigureAwait(false);  // Ensure tables are created asynchronously
        //_dbContext = ApplicationDbContext.Instance;
        dbContext.OnReservationAdded();

        MainPage = new AppShell();
    }
}

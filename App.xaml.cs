using RentARideDB.Models;
using RentARideDB.Services;

namespace RentARideDB;

public partial class App : Application
{
    private readonly ApplicationDbContext _dbContext;
    public App()
    {

        InitializeComponent();
        InitializeAsync();

        MainPage = new AppShell();
    }
    private async void InitializeAsync()
    {
        var dbContext = ApplicationDbContext.Instance;
        await dbContext.InitAsync().ConfigureAwait(false);
        //await dbContext.OnReservationAdded();
    }
}

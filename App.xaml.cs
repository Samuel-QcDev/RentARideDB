using RentARideDB.Models;

namespace RentARideDB;

public partial class App : Application
{
    
    public App()
    {
        InitializeComponent();

        // Database = new ApplicationDbContext();
        
        MainPage = new AppShell();
    }


    // public static ApplicationDbContext Database { get; set; }
}

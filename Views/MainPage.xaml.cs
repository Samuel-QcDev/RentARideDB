using RentARideDB.ViewModel;
using RentARideDB.Services;

namespace RentARideDB.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
	{
        InitializeComponent();

        // Create the ReservationService instance
        var dbContext = new ApplicationDbContext();
        MainViewModel vm = new MainViewModel(dbContext);
        BindingContext = vm;
    }
}


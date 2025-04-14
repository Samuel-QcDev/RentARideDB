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
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Tell the ViewModel to refresh the data
        if (BindingContext is MainViewModel vm)
        {
             vm.LoadReservations(); // Or async: await vm.LoadReservationsAsync();
        }
    }
}


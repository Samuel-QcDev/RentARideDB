using CommunityToolkit.Mvvm.ComponentModel;
using RentARideDB.ViewModel;
using RentARideDB.Services;

namespace RentARideDB.Views;

public partial class HistoriqueReservationPage : ContentPage
{
    private readonly ApplicationDbContext _dbContext;

    public HistoriqueReservationPage()

    {
		InitializeComponent();
        _dbContext = ApplicationDbContext.Instance;
        HistoriqueReservationViewModel vm = new HistoriqueReservationViewModel(_dbContext);
        BindingContext = vm;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Refresh welcome message when page appears
        await _dbContext.SetWelcomeMessageAsync();

        // Now manually update the ViewModel value
        var vm = BindingContext as HistoriqueReservationViewModel;
        if (vm != null)
        {
            vm.WelcomeMessage = _dbContext.WelcomeMessage;
        }
    }
}
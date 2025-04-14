using RentARideDB.ViewModel;
using RentARideDB.Services;

namespace RentARideDB.Views;

public partial class MainPage : ContentPage
{
    private readonly ApplicationDbContext _dbContext;
    public MainPage()
	{
        InitializeComponent();
        _dbContext = ApplicationDbContext.Instance;
        MainViewModel vm = new MainViewModel(_dbContext);

        BindingContext = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();


        // Refresh welcome message when page appears
        await _dbContext.SetWelcomeMessageAsync();

        // Now manually update the ViewModel value
        var vm = BindingContext as MainViewModel;
        if (vm != null)
        {
            vm.WelcomeMessage = _dbContext.WelcomeMessage;
        }
    }
}


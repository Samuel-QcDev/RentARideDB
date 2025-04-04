using RentARideDB.ViewModel;
using RentARideDB.Services;

namespace RentARideDB.Views;

public partial class MainPage : ContentPage
{
    private readonly ApplicationDbContext _dbContext;

    public MainPage()
	{

        InitializeComponent();
        // Access the singleton instance of ApplicationDbContext
        _dbContext = ApplicationDbContext.Instance;

        MainViewModel vm = new MainViewModel(_dbContext);
        BindingContext = vm;
    }

        //  private void CreateReservation_Clicked(object sender, EventArgs e)
        //  {
        //      Navigation.PushAsync(new ReservationPage());
        //  }
        //  private void History_Clicked(object sender, EventArgs e)
        //  {
        //Navigation.PushAsync(new HistoriqueReservationPage());
        //  }
    }


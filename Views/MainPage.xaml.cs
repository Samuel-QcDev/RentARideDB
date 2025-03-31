using RentARideDB.ViewModel;
using RentARideDB.Services;

namespace RentARideDB.Views;

public partial class MainPage : ContentPage
{
    private readonly ApplicationDbContext _dbContext;

    public MainPage(ApplicationDbContext dbContext)
	{

        InitializeComponent();
        _dbContext = dbContext;

        //// Create the ReservationService instance
        //var reservationService = new ReservationService();

        //MainViewModel vm = new MainViewModel(reservationService);
        //BindingContext = vm;
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


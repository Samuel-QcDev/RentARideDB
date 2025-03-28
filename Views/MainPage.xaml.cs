using RentARideDB.ViewModel;
using RentARideDB.Services;

namespace RentARideDB.Views;

public partial class MainPage : ContentPage
{

    public MainPage()
	{

        InitializeComponent();

        // Create the ReservationService instance
        var reservationService = new ReservationService();
        MainViewModel vm = new MainViewModel(reservationService);
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


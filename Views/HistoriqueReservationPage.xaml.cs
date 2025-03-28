using CommunityToolkit.Mvvm.ComponentModel;
using RentARideDB.ViewModel;
using RentARideDB.Services;

namespace RentARideDB.Views;

public partial class HistoriqueReservationPage : ContentPage
{
    private readonly ReservationService _reservationService;

    public HistoriqueReservationPage()
	{
		InitializeComponent();

		var reservationService = ReservationService.Instance;
        HistoriqueReservationViewModel vm = new HistoriqueReservationViewModel(_reservationService);
        BindingContext = vm;
	}

}
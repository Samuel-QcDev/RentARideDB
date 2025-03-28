using CommunityToolkit.Mvvm.Input;
using RentARideDB.Models;
using RentARideDB.Services;
using System.Collections.ObjectModel;

namespace RentARideDB.ViewModel;

public partial class HistoriqueReservationViewModel : LocalBaseViewModel
{
    private readonly ReservationService _reservationService;

    private ObservableCollection<Reservation> _reservationsResult;
    public ObservableCollection<Reservation> ReservationsResultPast => _reservationService.ReservationsResultPast;

    public HistoriqueReservationViewModel(ReservationService reservationService)
    {
        _reservationService = ReservationService.Instance;
    }

    [RelayCommand]
    private async Task BackToMainPage()
    {

        await Shell.Current.GoToAsync("Mainpage");
    }



}
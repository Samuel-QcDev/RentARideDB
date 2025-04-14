using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentARideDB.Models;
using RentARideDB.Services;
using System.Collections.ObjectModel;

namespace RentARideDB.ViewModel;

public partial class HistoriqueReservationViewModel : LocalBaseViewModel
{
    private readonly ApplicationDbContext _dbContext;
    public ObservableCollection<Reservation> ReservationsResultPast => _dbContext.ReservationsResultPast;
    public IRelayCommand<Reservation> CancelCommand { get; }
    [ObservableProperty]
    private string welcomeMessage;
    public HistoriqueReservationViewModel(ApplicationDbContext dbContext)
    {
        _dbContext = ApplicationDbContext.Instance;
        SetWelcomeMessage();
        CancelCommand = new RelayCommand<Reservation>(Cancel);
    }
    private async void SetWelcomeMessage()
    {
        await ApplicationDbContext.Instance.SetWelcomeMessageAsync();
        WelcomeMessage = ApplicationDbContext.Instance.WelcomeMessage;
    }
    public async Task LoadReservations()
    {
        await _dbContext.OnReservationAdded();
    }
    private async void Cancel(Reservation reservation)
    {
        await _dbContext.CancelReservationAsync(reservation.ReservationID);
    }
    [RelayCommand]
    private async Task BackToMainPage()
    {
        await Shell.Current.GoToAsync("Mainpage");
    }
    [RelayCommand]
    private async Task Logout()
    {
        var ActiveMemberID = await _dbContext.GetLoggedInMemberIdAsync();
        _dbContext.LogoutAsync(ActiveMemberID.Value);
        await Shell.Current.GoToAsync("Loginpage");
    }
}
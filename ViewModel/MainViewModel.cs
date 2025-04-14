using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using RentARideDB.Views;
using RentARideDB.Models;
using RentARideDB.ViewModel;
using RentARideDB.Services;
using System.Collections.ObjectModel;

namespace RentARideDB.ViewModel;

[QueryProperty(nameof(MemberUserName), "memberEmail")]
[QueryProperty(nameof(MemberPassword), "memberPassword")]
[QueryProperty(nameof(MemberFirstName), "memberFirstName")]

public partial class MainViewModel : LocalBaseViewModel
    {
    private readonly ApplicationDbContext _dbContext;

    public IRelayCommand<Reservation> CancelCommand { get; }

    public ObservableCollection<Reservation> ReservationsResultCurrent => _dbContext.ReservationsResultCurrent;


    [ObservableProperty] private string memberUserName;
    [ObservableProperty] private string memberPassword;
    [ObservableProperty] private string memberFirstName;
    [ObservableProperty] private string welcomeMessage;

    public MainViewModel(ApplicationDbContext dbContext)
    {
        _dbContext = ApplicationDbContext.Instance;
        WelcomeMessage = _dbContext.WelcomeMessage;
        CancelCommand = new RelayCommand<Reservation>(Cancel);
        //ReservationsResultCurrent = _dbContext.ReservationsResultCurrent;

        //_dbContext.ReservationsResultCurrent.Add(new Reservation
        //{
        //    TypeVehicule = "Test Type",
        //    VehiculeID = 2,
        //    StartTime = DateTime.Now,
        //    EndTime = DateTime.Now.AddHours(2),
        //    StationId = 1,
        //    CategorieAuto = "Essence"
        //});

    }
    private async Task SetWelcomeMessageAsync()
    {
        int? memberId = await _dbContext.GetLoggedInMemberIdAsync();

        if (!memberId.HasValue)
        {
            WelcomeMessage = "Bonjour Invité";
            return;
        }

        var member = await _dbContext.DbConnection
            .Table<Membre>()
            .FirstOrDefaultAsync(m => m.MemberID == memberId.Value);

        WelcomeMessage = member != null && !string.IsNullOrWhiteSpace(member.FirstName)
            ? $"Bonjour {member.FirstName}"
            : "Bienvenue";
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
    private async Task Reservation()
    {
        await _dbContext.SetWelcomeMessageAsync();
        await Shell.Current.GoToAsync($"Reservationpage?memberUserName={memberUserName}&memberPassword={memberPassword}&memberFirstName={memberFirstName}");
    }
    [RelayCommand]
    private async Task ConsultHistory()
    {
        await _dbContext.SetWelcomeMessageAsync();
        await Shell.Current.GoToAsync("Historiquereservationpage");
    }
    [RelayCommand]
    private async Task Logout()
    {
        var ActiveMemberID = await _dbContext.GetLoggedInMemberIdAsync();
        _dbContext.LogoutAsync(ActiveMemberID.Value);
        await Shell.Current.GoToAsync("Loginpage");
    }

}


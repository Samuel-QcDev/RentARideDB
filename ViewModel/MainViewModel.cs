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

[QueryProperty(nameof(MemberEmail), "memberEmail")]
[QueryProperty(nameof(MemberPassword), "memberPassword")]
[QueryProperty(nameof(MemberFirstName), "memberFirstName")]

public partial class MainViewModel : LocalBaseViewModel
    {
    private readonly ApplicationDbContext _dbContext;



    public ObservableCollection<Reservation> ReservationsResultCurrent => _dbContext.ReservationsResultCurrent;


    [ObservableProperty] private string memberEmail;
    [ObservableProperty] private string memberPassword;
    [ObservableProperty] private string memberFirstName;

    public MainViewModel(ApplicationDbContext dbContext)
    {
        _dbContext = ApplicationDbContext.Instance;
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
    //public ReservationResult ResultDetails { get; set; }
    //public ReservationSearchViewModel SearchViewModel { get; set; }
    //public ObservableCollection<Reservation> ReservationsResult { get; } = new();

    //public void RefreshReservationsResultCurrent()
    //{
    //    ReservationsResultCurrent = _dbContext.ReservationsResultCurrent;
    //}
    public async Task LoadReservations()
    {
        await _dbContext.OnReservationAdded();
    }
    [RelayCommand]
        private async Task Reservation()
        {
            await Shell.Current.GoToAsync($"Reservationpage?memberEmail={memberEmail}&memberPassword={memberPassword}&memberFirstName={memberFirstName}");
        }
        [RelayCommand]
        private async Task ConsultHistory()
        {

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


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

    private ObservableCollection<Reservation> _reservationsResult;
    public ObservableCollection<Reservation> ReservationsResultCurrent => _dbContext.ReservationsResultCurrent;


    [ObservableProperty] private string memberEmail;
    [ObservableProperty] private string memberPassword;
    [ObservableProperty] private string memberFirstName;

    public MainViewModel(ApplicationDbContext dbContext)
    {

    }
    public ReservationResult ResultDetails { get; set; }
    public ReservationSearchViewModel SearchViewModel { get; set; }
    public ObservableCollection<Reservation> ReservationsResult { get; } = new();


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
}


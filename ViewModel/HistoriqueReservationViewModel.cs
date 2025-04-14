﻿using CommunityToolkit.Mvvm.Input;
using RentARideDB.Models;
using RentARideDB.Services;
using System.Collections.ObjectModel;

namespace RentARideDB.ViewModel;

public partial class HistoriqueReservationViewModel : LocalBaseViewModel
{
    //private readonly ReservationService _reservationService;
    private readonly ApplicationDbContext _dbContext;

    public ObservableCollection<Reservation> ReservationsResultPast => _dbContext.ReservationsResultPast;

    public HistoriqueReservationViewModel(ApplicationDbContext dbContext)
    {
        _dbContext = ApplicationDbContext.Instance;
    }
        public async Task LoadReservations()
    {
        await _dbContext.OnReservationAdded();
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
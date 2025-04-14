using System.Dynamic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Maui.Core.Platform;
using System.Windows.Input;
using RentARideDB.Views;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using RentARideDB.Models;
using RentARideDB.Services;
using RentARideDB.ViewModel;
using System.Runtime.CompilerServices;

namespace RentARideDB.ViewModel;

[QueryProperty(nameof(MemberUserName), "memberUserName")]
[QueryProperty(nameof(MemberPassword), "memberPassword")]
[QueryProperty(nameof(MemberFirstName), "memberFirstName")]
public partial class LoginViewModel : LocalBaseViewModel
{
    private readonly ApplicationDbContext _dbContext;

    [ObservableProperty] private string memberUserName;
    [ObservableProperty] private string memberPassword;
    [ObservableProperty] private string memberFirstName;
    public LoginViewModel(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        LoginDetails = new Login();
        SessionDetails = new Session();
        MemberDetail = new Membre();
    }
    public Login LoginDetails { get; set; }
    public Session SessionDetails { get; set; }
    public Membre MemberDetail { get; set; }
    public async Task LoginAsync(int memberId)
    {
        var existingSession = await _dbContext.GetSessionByMemberIdAsync(memberId);
        if (existingSession != null)
        {
            // Update the session if it exists
            existingSession.IsActive = true;
            existingSession.LastLogin = DateTime.Now;
            await _dbContext.UpdateSessionAsync(existingSession);  // Assuming you have an async update method
        }
        else
        {
            // Create a new session if one does not exist
            var newSession = new Session(memberId);
            await _dbContext.InsertNewSessionAsync(newSession);  // Assuming you have an async insert method
        }
    }

    [RelayCommand]
    private async Task Submit()
    {
        if (LoginDetails == null)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Account invalid",
                $"Please create an account.",
                "OK");
        }
        else
        {
            // Fetch vehicles from the database
            var AllMembers = await _dbContext.GetMembresAsync();

            //selectedReservation = AllReservations.Where(v => v.MemberID == ReservationSearchDetails.TypeVehicule).ToList();

            if (AllMembers.Count > 0)
            {
                foreach (Membre membre in AllMembers)
                {
                    if (((LoginDetails.EmailAddress == membre.MemberUserName) && (LoginDetails.Password == membre.MemberPassword)))
                    {
                        await LoginAsync(membre.MemberID);
                        await _dbContext.OnReservationAdded();
                        await Shell.Current.GoToAsync($"Mainpage?memberEmail={MemberUserName}&memberPassword={MemberPassword}&memberFirstName={MemberFirstName}");
                        return;
                    }
                }
                await Application.Current.MainPage.DisplayAlert(
             "Account invalid",
             $"Please create an account or use the correct credentials.",
             "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(
                                "No members created",
                                $"Please create an account!",
                                "OK");
            }
        }
    }
    [RelayCommand]
    private static async Task CreateAccount()
    {

        await Shell.Current.GoToAsync("MembreDetailspage");
    }
}

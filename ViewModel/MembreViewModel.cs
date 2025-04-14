using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentARideDB;
using RentARideDB.Models;
using RentARideDB.ViewModel;
using RentARideDB.Views;
using RentARideDB.Services;

namespace RentARideDB.ViewModel;

public partial class MembreViewModel : LocalBaseViewModel
{
    private readonly ApplicationDbContext _dbContext;
    public MembreViewModel(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;

        MembreDetails = new Membre();
        CreerMembre(0, "Julie", "Or");
        CreerMembre(1, "Tom", "Bronze");
        CreerMembre(2, "Sam", "Libre");
        CreerMembre(3, "Max", "Or");
        for (int i = 0; i < 4; i++)
        {
            Debug.WriteLine(myMembres[i].GetValue(myMembres[i]));
        }
    }

    public override string ToString()
    {
        return "id + name + level".ToString();
    }

    public Membre MembreDetails { get; set; }
    public string Name
    {
        get { return Name; }
    }

    MembreDetails[] myMembres = new MembreDetails[10];
    public async void CreerMembre(int id, string name, string level)
    {
        myMembres[id] = new MembreDetails(id, name, level);
        await _dbContext.CreateAsync(new MembreDetails(id, name, level));
    }


    [RelayCommand]
    public async Task AddMembre()
    {
        var memberDetails = this.MembreDetails;
        var memberFirstName = memberDetails.FirstName;
        var memberUserName = memberDetails.MemberUserName;
        var memberPassword = memberDetails.MemberPassword;

        //var navigationParameter = new Dictionary<string, object> { { "member", memberDetails}};

        // await Shell.Current.DisplayAlert("Record Added", "Employee Details Successfully submitted", "OK");
        if (memberFirstName == null || memberUserName == null || memberPassword == null)
        {
            await Application.Current.MainPage.DisplayAlert("Missing Required Fields", $"Please fill all required fields.", "OK");
        }
        else
        {
            await _dbContext.CreateAsync(new Membre(memberFirstName, memberPassword, memberUserName));

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var message = "Your account was created!";
            ToastDuration duration = ToastDuration.Short;
            var fontSize = 14;
            var toast = Toast.Make(message, duration, fontSize);
            await toast.Show(cancellationTokenSource.Token);
            await Shell.Current.GoToAsync($"Loginpage?memberUserName={memberUserName}&memberPassword={memberPassword}&memberFirstName={memberFirstName}");
        }

        //await Shell.Current.GoToAsync($"Loginpage, navigationParameter");
    }
    [RelayCommand]
    private async Task BackToLogin()
    {
        await Shell.Current.GoToAsync("Loginpage");
    }
}

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
        var memberEmail = memberDetails.MemberEmail;
        var memberPassword = memberDetails.MemberPassword;
        await _dbContext.CreateAsync(new Membre(memberFirstName, memberPassword, memberEmail));

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        var message = "Your account was created!";
        ToastDuration duration = ToastDuration.Short;
        var fontSize = 14;
        var toast = Toast.Make(message, duration, fontSize);
        await toast.Show(cancellationTokenSource.Token);

        //var navigationParameter = new Dictionary<string, object> { { "member", memberDetails}};

        // await Shell.Current.DisplayAlert("Record Added", "Employee Details Successfully submitted", "OK");

        await Shell.Current.GoToAsync($"Loginpage?memberEmail={memberEmail}&memberPassword={memberPassword}&memberFirstName={memberFirstName}");
        //await Shell.Current.GoToAsync($"Loginpage, navigationParameter");
    }
}

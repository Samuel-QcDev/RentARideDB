using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
using System.Runtime.Intrinsics.X86;
using RentARideDB.ViewModel;
using RentARideDB.Models;
using RentARideDB.Services;

namespace RentARideDB.Views;

public partial class ReservationSearchPage : ContentPage
{
    private ApplicationDbContext _dbContext;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public string TypeVehicule { get; set; }
    public string CategorieAuto { get; set; }
    public string StationId { get; set; }
    //public Enum Options { get; set; }

  
    public ReservationSearchPage(ApplicationDbContext dbContext)
        {
        InitializeComponent();
        _dbContext = dbContext;  // Dependency Injection automatically provides the instance
        var vm = new ReservationSearchViewModel(_dbContext);
        BindingContext = vm;
            
        }

    // This method is called when the page is about to appear
    //protected override void OnAppearing()
    //{
    //    base.OnAppearing();

    //    // Retrieve the query parameter 'name' passed in the navigation
    //    var name = Shell.Current?.CurrentState?.QueryParameters["name"];

        //public ReservationSearchPage(DateTime startDate, DateTime endDate, DateTime startTime, DateTime endTime, string type, string station,
        //        [Optional] string categorie, [Optional] Enum options)
        //{
        //    StartDate = DateTime.Now;
        //    BindingContext = vm;
        //    InitializeComponent();

        //    StartTime = startTime;
        //    EndTime = endTime;
        //    StartDate = startDate;
        //    EndDate = endDate;
        //    TypeVehicule = type;
        //    CategorieAuto = categorie;
        //    StationId = station;
        //    Options = options;
        //}

        //private void VehicleType_OnSelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (VehicleType.SelectedItem.ToString() == "Auto")
        //    {
        //        OptionsLayout.IsVisible = true;

        //    }
        //    else
        //    {
        //        OptionsLayout.IsVisible = false;
        //    }
        //}
    }
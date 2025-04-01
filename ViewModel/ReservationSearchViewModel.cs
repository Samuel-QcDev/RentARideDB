
using RentARideDB.ViewModel;
using RentARideDB.Services;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Runtime.Intrinsics.X86;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentARideDB.Models;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using RentARideDB.Controls;
using RentARideDB.Tools;

namespace RentARideDB.ViewModel;

[QueryProperty(nameof(MemberEmail), "memberEmail")]
[QueryProperty(nameof(MemberPassword), "memberPassword")]
[QueryProperty(nameof(MemberFirstName), "memberFirstName")]
public partial class ReservationSearchViewModel : LocalBaseViewModel
{
    private readonly ApplicationDbContext _dbContext;

    [ObservableProperty] private string memberEmail;
    [ObservableProperty] private string memberPassword;
    [ObservableProperty] private string memberFirstName;

    [ObservableProperty]
    private bool isCheckedMP3;
    [ObservableProperty]
    private bool isCheckedGPS;
    [ObservableProperty]
    private bool isCheckedAC;
    [ObservableProperty]
    private bool isCheckedChildSeat;
    [ObservableProperty]
    private bool isCheckedEssence;
    [ObservableProperty]
    private bool isCheckedElectric;
    [ObservableProperty]
    private bool isAutoSelected;
    [ObservableProperty]
    private bool isVeloSelected;
    [ObservableProperty]
    private string categorieAuto;

    private ReservationSearch _reservationSearchDetails;
    public ReservationSearch ReservationSearchDetails
    {
        get => _reservationSearchDetails;
        set
        {
            if (_reservationSearchDetails != value)
            {
                _reservationSearchDetails = value;
                OnPropertyChanged();
            }
        }
    }
    public Auto AutoDetails { get; set; }
    public Station StationDetails { get; set; }
    public Reservation ReservationDetails {  get; set; }
    public Membre MembreDetails { get; set; }
    public ReservationResult ResultDetails { get; set; }
    public int DateChangedFlag { get; set; } = 0;



    //public ReservationService ReservationService => ReservationService.Instance;

    public Vehicule VehiculeDetails { get; set; }
    public ICommand OnVehicleTypeChangedCommand { get; }
    public ICommand OnStationChangedCommand { get; }
    public ICommand OnStartTimeChangedCommand { get; }
    public ICommand OnEndTimeChangedCommand { get; }
    public ICommand OnStartDateChangedCommand { get; }
    public ICommand OnEndDateChangedCommand { get; }
    public IRelayCommand<Vehicule> ReserveCommand { get; }
    public IRelayCommand<Reservation> CancelCommand { get; }
    private TimeSpan _startTime;
    //public ObservableCollection<Reservation> ReservationsResultPast { get; set; }
    //public ObservableCollection<Reservation> ReservationsResultCurrent { get; set; }
    public ObservableCollection<Vehicule> Vehicules { get; } = new();
    public ObservableCollection<Station> Stations { get; } = new();

    public TimeSpan StartTime
    {
        get => _startTime;
        set
        {
            if (_startTime != value)
            {
                _startTime = value;
                OnPropertyChanged();
                OnStartTimeChangedCommand?.Execute(value);  // This notifies the UI that the property has changed
            }
        }
    }
    private TimeSpan _endTime;
    public TimeSpan EndTime
    {
        get => _endTime;
        set
        {
            if (_endTime != value)
            {
                _endTime = value;
                OnPropertyChanged();
                OnEndTimeChangedCommand?.Execute(value);  // This notifies the UI that the property has changed
            }
        }
    }
    private DateTime _startDate;

    public DateTime StartDate
    {
        get => _startDate;
        set
        {
            if (_startDate != value)
            {
                _startDate = value;
                OnPropertyChanged();
                OnStartDateChangedCommand?.Execute(value);  // This notifies the UI that the property has changed
                StartDate = StartDate.Date;
            }
        }
    }
    private DateTime _endDate;
    public DateTime EndDate
    {
        get => _endDate;
        set
        {
            if (_endDate != value)
            {
                _endDate = value;
                OnPropertyChanged();
                OnEndDateChangedCommand?.Execute(value);// This notifies the UI that the property has changed
            }
        }
    }
    public bool IsCarAvailable(ObservableCollection<Reservation> reservations, string vehiculeID, DateTime newStartTime, DateTime newEndTime)
    {
        // Check for overlap with existing reservations for the same car
        foreach (var reservation in reservations)
        {
            if (reservation.VehiculeID == vehiculeID)
            {
                // Check if the new reservation time overlaps with an existing one
                if ((newStartTime < reservation.EndTime) && (newEndTime > reservation.StartTime))
                {
                    return false; // Car is not available
                }
            }
        }
        return true; // Car is available
    }
    public ReservationSearchViewModel(ApplicationDbContext dbContext)
    {
        //Console.WriteLine($"MemberEmail: {MemberEmail}, MemberPassword: {MemberPassword}, MemberFirstName: {MemberFirstName}");

        // Initialize the Commands for UI inputs
        OnVehicleTypeChangedCommand = new RelayCommand(OnVehicleTypeChanged);
        OnStationChangedCommand = new RelayCommand(OnStationChanged);
        OnStartTimeChangedCommand = new RelayCommand(OnStartTimeChanged);
        OnEndTimeChangedCommand = new RelayCommand(OnEndTimeChanged);
        OnStartDateChangedCommand = new RelayCommand(OnStartDateChanged);
        OnEndDateChangedCommand = new RelayCommand(OnEndDateChanged);
        ReserveCommand = new RelayCommand<Vehicule>(Reserve);
        CancelCommand = new RelayCommand<Reservation>(Cancel);

        // Create an instance of some required Classes
        ReservationSearchDetails = new ReservationSearch();
        AutoDetails = new Auto();
        VehiculeDetails = new Vehicule();
        StationDetails = new Station();
        ReservationDetails = new Reservation();
        ResultDetails = new ReservationResult();

        _dbContext.ReservationsResultPast = new ObservableCollection<Reservation>();
        _dbContext.ReservationsResultCurrent = new ObservableCollection<Reservation>();

        // Initialize some options
        StartDate = DateTime.Now.Date;
        EndDate = DateTime.Now.Date;
        ReservationSearchDetails.RequestedStartTime = StartDate.Add(StartTime);
        ReservationSearchDetails.RequestedEndTime = EndDate.Add(EndTime);
        ReservationSearchDetails.TypeVehicule = "Auto";
        IsAutoSelected = true;
        ReservationSearchDetails.StationAddress = "All Stations";
        ReservationSearchDetails.CategorieAuto = "Essence";

        LoadData();  // Load all the data (Vehicules, stations, reservations)
        AddVehiculesBasedOnAllUserInputs();     // Populate the CollectionView with vehicules according to initial conditions
        OnReservationAdded(); // Add the reservations to the correct CollectionView to display on MainPage or HistoriquePage 

        //Console.WriteLine(MemberFirstName);

        //if (MemberFirstName != null)
        //{
        //    Greeting = $"Hello, {MemberFirstName}!";
        //}
        //else
        //{
        //    Greeting = "Hello User!";
        //}

        //Console.WriteLine(Greeting);

        //Console.WriteLine(Vehicules.Count);
        //DateChangedFlag = 0;
    }
    public void  LoadData()
    {
        Vehicules.Clear();
        //Total # of Vehicules : 102
        //# of Autos : 74
        CreerVehicule("Auto", "AU001","P001", "Essence", []);
        CreerVehicule("Auto", "AU001", "P001", "Essence", ["MP3", "AC"]);
        CreerVehicule("Auto", "AU001", "P001", "Essence", ["GPS", "AC", "MP3"]);
        CreerVehicule("Auto", "AU001", "P001", "Essence", []);
        CreerVehicule("Auto", "AU001", "P001", "Électrique", []);
        CreerVehicule("Auto", "AU001", "P001", "Électrique", ["GPS", "MP3"]);
        CreerVehicule("Auto", "AU001", "P002", "Essence", []);
        CreerVehicule("Auto", "AU001", "P002", "Électrique", ["GPS", "AC"]);
        CreerVehicule("Auto", "AU001", "P002", "Essence", ["GPS", "AC"]);
        CreerVehicule("Auto", "AU001", "P002", "Essence", ["MP3", "AC"]);
        CreerVehicule("Auto", "AU001", "P002", "Essence", ["GPS", "AC", "MP3"]);
        CreerVehicule("Auto", "AU001", "P002", "Électrique", []);
        CreerVehicule("Auto", "AU001", "P003", "Essence", []);
        CreerVehicule("Auto", "AU001", "P003", "Essence", ["GPS", "MP3"]);
        CreerVehicule("Auto", "AU001", "P003", "Électrique", []);
        CreerVehicule("Auto", "AU001", "P004", "Essence", []);
        CreerVehicule("Auto", "AU001", "P004", "Essence", ["GPS", "AC"]);
        CreerVehicule("Auto", "AU001", "P004", "Électrique", []);
        CreerVehicule("Auto", "AU001", "P005", "Essence", ["GPS", "AC", "MP3"]);
        CreerVehicule("Auto", "AU001", "P005", "Essence", []);
        CreerVehicule("Auto", "AU001", "P005", "Électrique", []);
        CreerVehicule("Auto", "AU001", "P006", "Essence", []);
        CreerVehicule("Auto", "AU001", "P006", "Électrique", ["GPS", "AC"]);
        CreerVehicule("Auto", "AU001", "P006", "Électrique", ["GPS", "AC"]);
        CreerVehicule("Auto", "AU001", "P006", "Essence", ["GPS", "AC"]);
        CreerVehicule("Auto", "AU001", "P006", "Électrique", []);
        CreerVehicule("Auto", "AU001", "P007", "Essence", []);
        CreerVehicule("Auto", "AU001", "P007", "Essence", []);
        CreerVehicule("Auto", "AU001", "P007", "Électrique", ["AC", "ChildSeat"]);
        CreerVehicule("Auto", "AU001", "P007", "Essence", ["GPS", "MP3"]);
        CreerVehicule("Auto", "AU001", "P007", "Électrique", ["GPS", "AC"]);
        CreerVehicule("Auto", "AU001", "P007", "Électrique", []);
        CreerVehicule("Auto", "AU001", "P008", "Essence", []);
        CreerVehicule("Auto", "AU001", "P008", "Essence", ["MP3", "AC"]);
        CreerVehicule("Auto", "AU001", "P008", "Essence", ["GPS", "AC", "MP3"]);
        CreerVehicule("Auto", "AU001", "P008", "Électrique", []);
        CreerVehicule("Auto", "AU001", "P009", "Essence", []);
        CreerVehicule("Auto", "AU001", "P009", "Essence", ["GPS", "MP3"]);
        CreerVehicule("Auto", "AU001", "P009", "Électrique", ["GPS", "AC"]);
        CreerVehicule("Auto", "AU001", "P009", "Électrique", ["GPS", "AC"]);
        CreerVehicule("Auto", "AU001", "P009", "Essence", ["GPS", "AC"]);
        CreerVehicule("Auto", "AU001", "P009", "Électrique", []);
        CreerVehicule("Auto", "AU001", "P010", "Essence", []);
        CreerVehicule("Auto", "AU001", "P010", "Essence", []);
        CreerVehicule("Auto", "AU001", "P010", "Électrique", ["AC", "ChildSeat"]);
        CreerVehicule("Auto", "AU001", "P010", "Essence", ["GPS", "MP3"]);
        CreerVehicule("Auto", "AU001", "P010", "Électrique", ["GPS", "AC"]);
        CreerVehicule("Auto", "AU001", "P010", "Électrique", []);
        CreerVehicule("Auto", "AU001", "P011", "Essence", []);
        CreerVehicule("Auto", "AU001", "P011", "Essence", ["MP3", "AC"]);
        CreerVehicule("Auto", "AU001", "P011", "Essence", ["GPS", "AC", "MP3"]);
        CreerVehicule("Auto", "AU001", "P011", "Essence", []);
        CreerVehicule("Auto", "AU001", "P011", "Électrique", ["AC", "ChildSeat"]);
        CreerVehicule("Auto", "AU001", "P011", "Essence", ["GPS", "MP3"]);
        CreerVehicule("Auto", "AU001", "P011", "Électrique", []);
        CreerVehicule("Auto", "AU001", "P012", "Essence", []);
        CreerVehicule("Auto", "AU001", "P012", "Essence", ["GPS", "AC"]);
        CreerVehicule("Auto", "AU001", "P012", "Essence", ["MP3", "AC"]);
        CreerVehicule("Auto", "AU001", "P012", "Essence", ["GPS", "AC", "MP3"]);
        CreerVehicule("Auto", "AU001", "P012", "Électrique", []);
        CreerVehicule("Auto", "AU001", "P013", "Essence", []);
        CreerVehicule("Auto", "AU001", "P013", "Essence", ["GPS", "MP3"]);
        CreerVehicule("Auto", "AU001", "P013", "Électrique", ["GPS", "AC"]);
        CreerVehicule("Auto", "AU001", "P013", "Électrique", ["GPS", "AC"]);
        CreerVehicule("Auto", "AU001", "P013", "Essence", ["GPS", "AC"]);
        CreerVehicule("Auto", "AU001", "P013", "Électrique", []);
        CreerVehicule("Auto", "AU001", "P014", "Essence", []);
        CreerVehicule("Auto", "AU001", "P014", "Essence", []);
        CreerVehicule("Auto", "AU001", "P014", "Électrique", []);
        CreerVehicule("Auto", "P015", "AU001", "Essence", []);
        CreerVehicule("Auto", "P015", "AU001", "Électrique", ["GPS", "AC"]);
        CreerVehicule("Auto", "P015", "AU001", "Électrique", ["GPS", "AC"]);
        CreerVehicule("Auto", "P015", "AU001", "Électrique", ["GPS", "AC"]);
        CreerVehicule("Auto", "P015", "AU001", "Électrique", []);

        //# of Motos : 16
        CreerVehicule("Moto", "M001", "P001");
        CreerVehicule("Moto", "M002", "P001");
        CreerVehicule("Moto", "M003", "P002");
        CreerVehicule("Moto", "M004", "P002");
        CreerVehicule("Moto", "M005", "P003");
        CreerVehicule("Moto", "M006", "P005");
        CreerVehicule("Moto", "M007", "P006");
        CreerVehicule("Moto", "M008", "P007");
        CreerVehicule("Moto", "M009", "P007");
        CreerVehicule("Moto", "M010", "P009");
        CreerVehicule("Moto", "M011", "P011");
        CreerVehicule("Moto", "M012", "P013");
        CreerVehicule("Moto", "M013", "P013");
        CreerVehicule("Moto", "M014", "P015");
        CreerVehicule("Moto", "M015", "P015");
        CreerVehicule("Moto", "M016", "P015");

        //# of Velos : 12
        CreerVehicule("Velo", "V001","P001");
        CreerVehicule("Velo", "V002", "P002");
        CreerVehicule("Velo", "V003", "P003");
        CreerVehicule("Velo", "V004", "P004");
        CreerVehicule("Velo", "V005", "P005");
        CreerVehicule("Velo", "V006", "P006");
        CreerVehicule("Velo", "V007", "P007");
        CreerVehicule("Velo", "V008", "P008");
        CreerVehicule("Velo", "V009", "P009");
        CreerVehicule("Velo", "V010", "P010");
        CreerVehicule("Velo", "V011", "P011");
        CreerVehicule("Velo", "V012", "P012");

        //# of Stations : 15
        CreerStation(0, "P001", "Dorchester-Charest", 10, 2);
        CreerStation(1, "P002", "Carre D'Youville", 10, 2);
        CreerStation(2, "P003", "Limoilou", 5, 1);
        CreerStation(3, "P004", "Saint-Roch", 4, 1);
        CreerStation(4, "P005", "Beauport", 5, 1);
        CreerStation(5, "P006", "Vanier", 8, 2);
        CreerStation(6, "P007", "Vieux-Quebec - Plaines d'Abraham", 10, 2);
        CreerStation(7, "P008", "Vieux-Quebec - St-Jean", 6, 2);
        CreerStation(8, "P009", "Charlesbourg", 9, 2);
        CreerStation(9, "P010", "ULaval", 8, 2);
        CreerStation(10, "P011", "Sainte-Foy", 9, 1);
        CreerStation(11, "P012", "Sillery", 8, 3);
        CreerStation(12, "P013", "Levis", 10, 2);
        CreerStation(13, "P014", "Cap-Rouge", 6, 3);
        CreerStation(14, "P015", "Chutes Montmorency", 10, 1);

        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0).AddMinutes(0), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("AU002", "P001", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0).AddMinutes(0), DateTime.Today.AddDays(0).AddHours(3).AddMinutes(0), new Auto("AU005", "P001", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), DateTime.Today.AddDays(0).AddHours(5).AddMinutes(0), new Auto("AU002", "P001", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(3).AddMinutes(30), new Auto("AU011", "P002", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(5).AddMinutes(0), DateTime.Today.AddDays(0).AddHours(7).AddMinutes(0), new Auto("AU023", "P006", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(3), DateTime.Today.AddDays(0).AddHours(4).AddMinutes(30), new Auto("AU023", "P006", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(1).AddHours(1), DateTime.Today.AddDays(1).AddHours(3).AddMinutes(0), new Auto("AU023", "P006", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(2).AddHours(3), DateTime.Today.AddDays(2).AddHours(4).AddMinutes(0), new Auto("AU023", "P006", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("AU027", "P007", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(30), new Auto("AU036", "P008", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0).AddHours(30), DateTime.Today.AddDays(0).AddHours(1).AddMinutes(0), new Auto("AU041", "P009", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(1).AddMinutes(0), new Auto("AU062", "P013", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(1).AddMinutes(0), new Auto("AU074", "P015", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("AU049", "P011", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("AU0055", "P011", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("AU056", "P012", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("AU038", "P009", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("AU039", "P009", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("AU024", "P006", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("AU025", "P006", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("AU026", "P006", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("AU028", "P007", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("AU029", "P007", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("AU042", "P009", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("AU043", "P010", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("AU044", "P010", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("AU045", "P010", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("AU046", "P010", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("AU047", "P010", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0), DateTime.Today.AddDays(0).AddHours(1).AddMinutes(0), new Auto("M01", "P001", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(30), new Auto("M16", "P015", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("M02", "P001", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(2), DateTime.Today.AddDays(0).AddHours(5).AddMinutes(0), new Auto("M05", "P003", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(1).AddHours(0), DateTime.Today.AddDays(1).AddHours(2).AddMinutes(0), new Auto("M05", "P003", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(1).AddHours(2), DateTime.Today.AddDays(1).AddHours(3).AddMinutes(0), new Auto("M12", "P013", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("M15", "P015", "Essence", []    ));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0), DateTime.Today.AddDays(0).AddHours(3).AddMinutes(0), new Auto("V01", "P001", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(5).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(7).AddMinutes(30), new Auto("V01", "P001", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("V03", "P003", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(5), DateTime.Today.AddDays(0).AddHours(8).AddMinutes(0), new Auto("V03", "P003", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(3).AddMinutes(0), new Auto("V06", "P006", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("V07", "P007", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(0), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("V08", "P008", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("V09", "P009", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(5), DateTime.Today.AddDays(0).AddHours(7).AddMinutes(0), new Auto("V09", "P009", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), new Auto("V10", "P010", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(3), DateTime.Today.AddDays(0).AddHours(4).AddMinutes(0), new Auto("V10", "P010", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(1).AddHours(2), DateTime.Today.AddDays(1).AddHours(5).AddMinutes(0), new Auto("V11", "P011", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(5).AddMinutes(0), new Auto("V12", "P012", "Essence", []));
        CreerReservation("MEM001", DateTime.Today.AddDays(2).AddHours(0), DateTime.Today.AddDays(2).AddHours(2).AddMinutes(0), new Auto("V12", "P012", "Essence", []));

        // Past Reservations
        CreerReservation("MEM007", new DateTime(2025,03,11,10,30,0), new DateTime(2025, 03, 11, 11, 30, 0), new Auto("AU005", "P001", "Essence", []));
        CreerReservation("MEM007", new DateTime(2025, 03, 15, 14, 0, 0), new DateTime(2025, 03, 15, 16, 30, 0), new Auto("AU005", "P002", "Essence", []));
        CreerReservation("MEM007", new DateTime(2025, 03, 17, 10, 00, 0), new DateTime(2025, 03, 17, 11, 30, 0), new Auto("AU005", "P015", "Essence", []));
        CreerReservation("MEM001", new DateTime(2025, 03, 19, 10, 30, 0), new DateTime(2025, 03, 11, 11, 30, 0), new Auto("AU005", "P001", "Essence", []));
        CreerReservation("MEM005", new DateTime(2025, 03, 20, 10, 30, 0), new DateTime(2025, 03, 11, 11, 30, 0), new Auto("AU005", "P001", "Essence", []));
    }

   //List of all vehicules
    Vehicule[] myVehicules = new Vehicule[110];

     // Method Create a Vehicule and add it to list of all vehicules
    public async void CreerVehicule(string type, string vehiculeID, string stationID, string categorie = null, List<string> carOptions=null)
    {
        if (type == "Auto")
        {
            // Create the auto
            var vehicule = new Auto(vehiculeID, stationID, categorie, carOptions);
            // Insert the auto in the database
            await _dbContext.CreateAsync(vehicule);
            // Insert AutoOptions into the AutoOption table
            foreach (var option in carOptions)
            {
                var autoOption = new AutoOption
                {
                    Option = option,
                    AutoId = vehicule.autoId  // Foreign key reference to the Auto
                };

                await _dbContext.CreateAsync(autoOption);
            }
        }
        else if (type == "Velo")
        {
            var vehicule = new Velo(stationID);
            await _dbContext.CreateAsync(vehicule);
        }
        else if(type == "Moto")
        {
            var vehicule = new Moto(stationID);
            await _dbContext.CreateAsync(vehicule);
        }

        //Vehicules.Add(myVehicules[index]);
    }
    public async void creerMembre(string name, string password, string email)
    {
        await _dbContext.CreateAsync(new Membre(name, password, email));
    }
    //List of all stations
    Station[] myStations = new Station[20];
    // Method to create a Station and add it to list of all stations
    public async void CreerStation(int index, string id, string address, int spaces, int bikeSpaces)
    {
        myStations[index] = new Station(index, id, address, spaces, bikeSpaces);
        await _dbContext.CreateAsync(new Station(index, id, address, spaces, bikeSpaces));
    }
    public async void CreerReservation(string memberid, DateTime requestedStartTime, DateTime requestedEndTime, Vehicule vehicule)
    {
        await _dbContext.CreateAsync(new Reservation(memberid, requestedStartTime, requestedEndTime, vehicule));
    }
    //public void CreerReservation(string memberid, DateTime requestedStartTime, DateTime requestedEndTime, Vehicule vehicule)
    //{
    //    myReservations[index] = reservation;
    //    Reservations.Add(myReservations[index]);
    //}
    partial void OnCategorieAutoChanged(string value)
    {
        AddVehiculesBasedOnAllUserInputs();
    }
    partial void OnIsCheckedMP3Changed(bool value)
    {
        if (!IsCheckedMP3) AddVehiculesBasedOnAllUserInputs("MP3");
        else AddVehiculesBasedOnAllUserInputs();
    }
    partial void OnIsCheckedACChanged(bool value)
    {
        if (!IsCheckedAC) AddVehiculesBasedOnAllUserInputs("AC");
        else AddVehiculesBasedOnAllUserInputs();
    }
    partial void OnIsCheckedGPSChanged(bool value)
    {
        if (!IsCheckedGPS) AddVehiculesBasedOnAllUserInputs("GPS");
        else AddVehiculesBasedOnAllUserInputs();
    }
    partial void OnIsCheckedChildSeatChanged(bool value)
    {
        if (!IsCheckedChildSeat) AddVehiculesBasedOnAllUserInputs("ChildSeat");
        else AddVehiculesBasedOnAllUserInputs();
    }
    private void OnStationChanged()
    {
        AddVehiculesBasedOnAllUserInputs();
    }
    private void OnVehicleTypeChanged()
    {
        AddVehiculesBasedOnAllUserInputs();
    }

    private async void OnStartTimeChanged()
    {
        //Console.WriteLine(StartTime);
        ReservationSearchDetails.RequestedStartTime = StartDate.Add(StartTime);
        await Task.Yield();
        if (ReservationSearchDetails.RequestedStartTime < DateTime.Now)
        {
            string message = "The Start time cannot be before now! \n\n Please enter a valid time.";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
    }
    private async void OnEndTimeChanged()
    {
        Console.WriteLine(EndTime);
        ReservationSearchDetails.RequestedEndTime = EndDate.Add(EndTime);
        TimeSpan interval = ReservationSearchDetails.RequestedEndTime - ReservationSearchDetails.RequestedStartTime;
        TimeSpan threshold6Hours = TimeSpan.FromHours(6);
        TimeSpan threshold30Mins = TimeSpan.FromMinutes(30);
        await Task.Yield();
        if (DateChangedFlag != 1)
            DateChangedFlag = 1;
        if (ReservationSearchDetails.RequestedEndTime < ReservationSearchDetails.RequestedStartTime)
        {
            string message = "The End Time cannot be BEFORE the Start Time! \n\n Please enter a valid time.";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
        else if (ReservationSearchDetails.RequestedStartTime < DateTime.Now)
        {
            string message = "The Start time cannot be before now! \n\n Please enter a valid time.";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
        else if (EndDate < StartDate)
        {
            string message = "The End Date cannot be BEFORE the Start Date! \n\n Please enter a valid Date.";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
        else if (interval > threshold6Hours)
        {
            {
                string message = "RentARide only provides short-time rentals! \n\n Please enter a time interval of less than 6 hours.";
                await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
            }
        }
        else if (interval < threshold30Mins)
        {
            string message = "The minimum rental period is 30 minutes! \n\n Please enter a valid interval.";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
        else
        {
            AddVehiculesBasedOnAllUserInputs();
        }
    }
    private async void OnStartDateChanged()
    {
        Console.WriteLine(StartDate);
        ReservationSearchDetails.RequestedStartTime = StartDate.Add(StartTime);
        await Task.Yield();
        if (StartDate < DateTime.Now.Date)
        {
            string message = "You cannot enter a Date before Today! \n\n Please enter a valid Date.";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
    }
    private async void OnEndDateChanged()
    {
        Console.WriteLine(EndDate);
        ReservationSearchDetails.RequestedEndTime = EndDate.Add(EndTime);
        await Task.Yield();
        if (DateChangedFlag != 1)
            DateChangedFlag = 1;
        if (EndDate < StartDate)
        {
            string message = "The End Date cannot be BEFORE the Start Date! \n\n Please enter a valid Date.";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
    }

    // Main method to filter vehicules based on criteria
    private void AddVehiculesBasedOnAllUserInputs(string optionsChecked = "")
    {
        Vehicules.Clear(); // Clear the Vehicules CollectionView
        StationDetails.selectedStationID.Clear();
        // Iterate over all stations to verify selected station(s)
        for (int i = myStations.Length - 1; i >= 0; i--)
        {
            if ((myStations[i] != null) && (!Stations.Contains(myStations[i])) && (ReservationSearchDetails.StationAddress == "All Stations"))
            {
                StationDetails.selectedStationID.Add(myStations[i].StationId);
            }
            else if ((myStations[i] != null) && (!Stations.Contains(myStations[i])) && (myStations[i].StationAddress == ReservationSearchDetails.StationAddress))
            {
                StationDetails.selectedStationID.Add(myStations[i].StationId);
            }
        }

        // Iterate over all vehhicules to check if it meets the criteria set by user
        for (int i = myVehicules.Length - 1; i >= 0; i--)
        {
            if (myVehicules[i] != null)
            {
                if (ReservationSearchDetails.TypeVehicule == "Auto")
                {
                    IsAutoSelected = true;
                    if (myVehicules[i].type == "Auto")
                    {
                        if (CheckCategorieAuto(myVehicules[i]))
                        {
                            if (CheckOptions(myVehicules[i], i, optionsChecked))
                            {
                                CheckStation(myVehicules[i]);
                            }
                        }
                    }
                }
                else
                {
                    IsAutoSelected = false;
                    if (ReservationSearchDetails.TypeVehicule == myVehicules[i].type)
                    {
                        CheckStation(myVehicules[i]);
                    }
                }
            }
        }
    }
    private bool CheckCategorieAuto(Vehicule vehicule)
    {
        string selectedCategory = CategorieAuto;
        if (selectedCategory == "Essence")
        {
            IsCheckedEssence = true;
            IsCheckedElectric = false;
        }
        else
        {
            IsCheckedEssence = false;
            IsCheckedElectric = true;
        }
        if (vehicule is Auto autoVehicule)
            // Returns true if CategorieAuto of vehicule is same as selected one
            return (autoVehicule.categorieAuto == selectedCategory);
        return false;
    }
    private bool CheckOptions(Vehicule vehicule, int index, string checkedOption = "")
    {
        ReservationSearchDetails.indexVehiculesToBeRemoved.Clear();

        HashSet<string> addOptions = new HashSet<string> { };
        HashSet<string> removalOptions = new HashSet<string> { };

        if (IsCheckedMP3) addOptions.Add("MP3");
        if (IsCheckedAC) addOptions.Add("AC");
        if (IsCheckedGPS) addOptions.Add("GPS");
        if (IsCheckedChildSeat) addOptions.Add("ChildSeat");

        if (checkedOption != "")
            removalOptions.Add(checkedOption);

        return true;

        //bool allValuesInList = addOptions.All(item => vehicule.AutoOptions.Contains(item));
        //bool AnyValueInList = addOptions.Any(item => vehicule.AutoOptions.Contains(item));
        //bool containsAnyValue = removalOptions.Any(item => vehicule.AutoOptions.Contains(item));

        //bool containsValueChecked = vehicule.AutoOptions.Contains(optionsChecked);
        //bool containsNoOption = vehicule.AutoOptions.Count == 0;

        // If option was just unchecked, don't add cars with that option
        //if (containsAnyValue)
        //    return false;

        //// If vehicule has no options and no option is checked, it is added
        //if (addOptions.Count == 0)
        //{
        //    if (containsNoOption) return true;
        //    return false;

        //}
        //// allValuesInList : each vehicule must have ALL checked options, or
        //// AnyValueInList : each vehicule must have ONE of the checked options
        //if (allValuesInList && !containsNoOption)
        //{
        //    return true;
        //}
        //return false;
    }
    private async void CheckStation(Vehicule vehicule)
    {
        // Check if vehicule (from myVehicules[i] above) is at a selected station & if it is available
        foreach (string station in StationDetails.selectedStationID)
        {
            if (vehicule.vehiculeStationId == station)
            {
                if (IsCarAvailable(ReservationDetails.Reservations, vehicule.vehiculeId, ReservationSearchDetails.RequestedStartTime, ReservationSearchDetails.RequestedEndTime))
                {
                    // Add the vehicule directly to the CollectionView
                    Vehicules.Add(vehicule);
                    //await _dbContext.CreateAsync(vehicule);
                    //ReservationSearchDetails.indexVehiculesToBeAdded.Add(i);
                }
            }
        }
    }

    // Unused
    //[RelayCommand]
    //private static async Task Search()
    //{
    //    await Shell.Current.GoToAsync("Resultpage");
    //}
    private void OnReservationAdded()
    {
        _dbContext.ReservationsResultPast.Clear();
        _dbContext.ReservationsResultCurrent.Clear();
        foreach (Reservation reservation in ReservationDetails.Reservations)
        {
            if ((reservation != null) && (reservation.MemberID == "MEM007") && (!(_dbContext.ReservationsResultPast.Contains(reservation))|| !(_dbContext.ReservationsResultCurrent.Contains(reservation))))
            {
                if (reservation.EndTime < DateTime.Now)
                {
                    _dbContext.ReservationsResultPast.Add(reservation);
                }
                else
                {
                    _dbContext.ReservationsResultCurrent.Add(reservation);
                }
            }
        }
    }
    private async void Reserve(Vehicule vehicule)
    {
        Console.WriteLine("Reserve called for vehicule: " + vehicule.vehiculeId);
        Console.WriteLine("# of items in ReservationResults collection: " );
        Console.WriteLine("# of items in Reservations collection: " + ReservationDetails.Reservations.Count);
        //int indexRes = ReservationDetails.myReservations.Length-1;
        //string resID;
        // ID for Logged in member, will need to be changed to retrieve MemberID from MembreDetails
        string currentMemberID = "MEM007";

        //if (indexRes < 100)
        //{
        //    resID = "RES00" + (indexRes).ToString();
        //}
        //else
        //{
        //    resID = "RES0" + (indexRes).ToString();
        //}
        //_reservationService.AddReservation(new Reservation(resID, currentMemberID, ReservationSearchDetails.RequestedStartTime, ReservationSearchDetails.RequestedEndTime, vehicule));
        //ReservationSearchDetails.RequestedEndTime = EndDate.Add(EndTime);
        TimeSpan interval = ReservationSearchDetails.RequestedEndTime - ReservationSearchDetails.RequestedStartTime;
        TimeSpan threshold6Hours = TimeSpan.FromHours(6);
        TimeSpan threshold30Mins = TimeSpan.FromMinutes(30);
        await Task.Yield();
        if (ReservationSearchDetails.RequestedEndTime < ReservationSearchDetails.RequestedStartTime)
        {
            string message = "The End Time cannot be BEFORE the Start Time! \n\n Please enter a valid time.";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
        else if (ReservationSearchDetails.RequestedStartTime < DateTime.Now)
        {
            string message = "The Start time cannot be before now! \n\n Please enter a valid time.";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
        else if (EndDate < StartDate)
        {
            string message = "The End Date cannot be BEFORE the Start Date! \n\n Please enter a valid Date.";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
        else if (interval > threshold6Hours)
        {
            {
                string message = "RentARide only provides short-time rentals! \n\n Please enter a time interval of less than 6 hours.";
                await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
            }
        }
        else if (interval < threshold30Mins)
        {
            string message = "The minimum rental period is 30 minutes! \n\n Please enter a valid time interval.";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
        else
        {
            CreerReservation(currentMemberID, ReservationSearchDetails.RequestedStartTime, ReservationSearchDetails.RequestedEndTime,vehicule);
            OnReservationAdded();
            AddVehiculesBasedOnAllUserInputs();
            await Shell.Current.GoToAsync("Mainpage");
        }
    }
    private void Cancel(Reservation reservation)
    {
        //Console.WriteLine(ReservationService.ReservationsResultCurrent.Count);
        //ReservationService.CancelReservation(reservation);
        //Console.WriteLine(ReservationService.ReservationsResultCurrent.Count);
    }
    [RelayCommand]
    private async Task BackToMainPage()
    {

        await Shell.Current.GoToAsync("Mainpage");
    }
}
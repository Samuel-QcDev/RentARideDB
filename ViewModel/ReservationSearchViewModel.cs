
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
        _dbContext.Vehicules.Clear();
        //Total # of Vehicules : 102
        //# of Autos : 74
        CreerVehicule(0, new Auto("AU001", "P001", "Essence", []));
        CreerVehicule(1, new Auto("AU002", "P001", "Essence", ["MP3", "AC"]));
        CreerVehicule(2, new Auto("AU003", "P001", "Essence", ["GPS", "AC", "MP3"]));
        CreerVehicule(3, new Auto("AU004", "P001", "Essence", []));
        CreerVehicule(4, new Auto("AU005", "P001", "Électrique", []));
        CreerVehicule(5, new Auto("AU006", "P001", "Électrique", ["GPS", "MP3"]));
        CreerVehicule(6, new Auto("AU007", "P002", "Essence", []));
        CreerVehicule(7, new Auto("AU008", "P002", "Électrique", ["GPS", "AC"]));
        CreerVehicule(8, new Auto("AU009", "P002", "Essence", ["GPS", "AC"]));
        CreerVehicule(9, new Auto("AU010", "P002", "Essence", ["MP3", "AC"]));
        CreerVehicule(10, new Auto("AU011", "P002", "Essence", ["GPS", "AC", "MP3"]));
        CreerVehicule(11, new Auto("AU012", "P002", "Électrique", []));
        CreerVehicule(12, new Auto("AU013", "P003", "Essence", []));
        CreerVehicule(13, new Auto("AU014", "P003", "Essence", ["GPS", "MP3"]));
        CreerVehicule(14, new Auto("AU015", "P003", "Électrique", []));
        CreerVehicule(15, new Auto("AU016", "P004", "Essence", []));
        CreerVehicule(16, new Auto("AU017", "P004", "Essence", ["GPS", "AC"]));
        CreerVehicule(17, new Auto("AU018", "P004", "Électrique", []));
        CreerVehicule(18, new Auto("AU019", "P005", "Essence", ["GPS", "AC", "MP3"]));
        CreerVehicule(19, new Auto("AU020", "P005", "Essence", []));
        CreerVehicule(20, new Auto("AU021", "P005", "Électrique", []));
        CreerVehicule(21, new Auto("AU022", "P006", "Essence", []));
        CreerVehicule(22, new Auto("AU023", "P006", "Électrique", ["GPS", "AC"]));
        CreerVehicule(23, new Auto("AU024", "P006", "Électrique", ["GPS", "AC"]));
        CreerVehicule(24, new Auto("AU025", "P006", "Essence", ["GPS", "AC"]));
        CreerVehicule(25, new Auto("AU026", "P006", "Électrique", []));
        CreerVehicule(26, new Auto("AU027", "P007", "Essence", []));
        CreerVehicule(27, new Auto("AU028", "P007", "Essence", []));
        CreerVehicule(28, new Auto("AU029", "P007", "Électrique", ["AC", "ChildSeat"]));
        CreerVehicule(29, new Auto("AU030", "P007", "Essence", ["GPS", "MP3"]));
        CreerVehicule(30, new Auto("AU031", "P007", "Électrique", ["GPS", "AC"]));
        CreerVehicule(31, new Auto("AU032", "P007", "Électrique", []));
        CreerVehicule(32, new Auto("AU033", "P008", "Essence", []));
        CreerVehicule(33, new Auto("AU034", "P008", "Essence", ["MP3", "AC"]));
        CreerVehicule(34, new Auto("AU035", "P008", "Essence", ["GPS", "AC", "MP3"]));
        CreerVehicule(35, new Auto("AU036", "P008", "Électrique", []));
        CreerVehicule(36, new Auto("AU037", "P009", "Essence", []));
        CreerVehicule(37, new Auto("AU038", "P009", "Essence", ["GPS", "MP3"]));
        CreerVehicule(38, new Auto("AU039", "P009", "Électrique", ["GPS", "AC"]));
        CreerVehicule(39, new Auto("AU040", "P009", "Électrique", ["GPS", "AC"]));
        CreerVehicule(40, new Auto("AU041", "P009", "Essence", ["GPS", "AC"]));
        CreerVehicule(41, new Auto("AU042", "P009", "Électrique", []));
        CreerVehicule(42, new Auto("AU043", "P010", "Essence", []));
        CreerVehicule(43, new Auto("AU044", "P010", "Essence", []));
        CreerVehicule(44, new Auto("AU045", "P010", "Électrique", ["AC", "ChildSeat"]));
        CreerVehicule(45, new Auto("AU046", "P010", "Essence", ["GPS", "MP3"]));
        CreerVehicule(46, new Auto("AU047", "P010", "Électrique", ["GPS", "AC"]));
        CreerVehicule(47, new Auto("AU048", "P010", "Électrique", []));
        CreerVehicule(48, new Auto("AU049", "P011", "Essence", []));
        CreerVehicule(49, new Auto("AU050", "P011", "Essence", ["MP3", "AC"]));
        CreerVehicule(50, new Auto("AU051", "P011", "Essence", ["GPS", "AC", "MP3"]));
        CreerVehicule(51, new Auto("AU052", "P011", "Essence", []));
        CreerVehicule(52, new Auto("AU053", "P011", "Électrique", ["AC", "ChildSeat"]));
        CreerVehicule(53, new Auto("AU054", "P011", "Essence", ["GPS", "MP3"]));
        CreerVehicule(54, new Auto("AU055", "P011", "Électrique", []));
        CreerVehicule(55, new Auto("AU056", "P012", "Essence", []));
        CreerVehicule(56, new Auto("AU057", "P012", "Essence", ["GPS", "AC"]));
        CreerVehicule(57, new Auto("AU058", "P012", "Essence", ["MP3", "AC"]));
        CreerVehicule(58, new Auto("AU059", "P012", "Essence", ["GPS", "AC", "MP3"]));
        CreerVehicule(59, new Auto("AU060", "P012", "Électrique", []));
        CreerVehicule(60, new Auto("AU061", "P013", "Essence", []));
        CreerVehicule(61, new Auto("AU062", "P013", "Essence", ["GPS", "MP3"]));
        CreerVehicule(62, new Auto("AU063", "P013", "Électrique", ["GPS", "AC"]));
        CreerVehicule(63, new Auto("AU064", "P013", "Électrique", ["GPS", "AC"]));
        CreerVehicule(64, new Auto("AU065", "P013", "Essence", ["GPS", "AC"]));
        CreerVehicule(65, new Auto("AU066", "P013", "Électrique", []));
        CreerVehicule(66, new Auto("AU067", "P014", "Essence", []));
        CreerVehicule(67, new Auto("AU068", "P014", "Essence", []));
        CreerVehicule(68, new Auto("AU069", "P014", "Électrique", []));
        CreerVehicule(69, new Auto("AU070", "P015", "Essence", []));
        CreerVehicule(70, new Auto("AU071", "P015", "Électrique", ["GPS", "AC"]));
        CreerVehicule(71, new Auto("AU072", "P015", "Électrique", ["GPS", "AC"]));
        CreerVehicule(72, new Auto("AU073", "P015", "Électrique", ["GPS", "AC"]));
        CreerVehicule(73, new Auto("AU074", "P015", "Électrique", []));

        //# of Motos : 16
        CreerVehicule(74, new Moto("M01", "P001"));
        CreerVehicule(75, new Moto("M02", "P001"));
        CreerVehicule(76, new Moto("M03", "P002"));
        CreerVehicule(77, new Moto("M04", "P002"));
        CreerVehicule(78, new Moto("M05", "P003"));
        CreerVehicule(79, new Moto("M06", "P005"));
        CreerVehicule(80, new Moto("M07", "P006"));
        CreerVehicule(81, new Moto("M08", "P007"));
        CreerVehicule(82, new Moto("M09", "P007"));
        CreerVehicule(83, new Moto("M10", "P009"));
        CreerVehicule(84, new Moto("M11", "P011"));
        CreerVehicule(85, new Moto("M12", "P013"));
        CreerVehicule(86, new Moto("M13", "P013"));
        CreerVehicule(87, new Moto("M14", "P015"));
        CreerVehicule(88, new Moto("M15", "P015"));
        CreerVehicule(89, new Moto("M16", "P015"));

        //# of Velos : 12
        CreerVehicule(90, new Velo("V01", "P001"));
        CreerVehicule(91, new Velo("V02", "P002"));
        CreerVehicule(92, new Velo("V03", "P003"));
        CreerVehicule(93, new Velo("V04", "P004"));
        CreerVehicule(94, new Velo("V05", "P005"));
        CreerVehicule(95, new Velo("V06", "P006"));
        CreerVehicule(96, new Velo("V07", "P007"));
        CreerVehicule(97, new Velo("V08", "P008"));
        CreerVehicule(98, new Velo("V09", "P009"));
        CreerVehicule(99, new Velo("V10", "P010"));
        CreerVehicule(100, new Velo("V11", "P011"));
        CreerVehicule(101, new Velo("V12", "P012"));

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

        ReservationDetails.CreerReservation(0, new Reservation("RES0001", "MEM001", DateTime.Today.AddDays(0).AddHours(0).AddMinutes(0), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Auto", "P001", "AU001"));
        ReservationDetails.CreerReservation(1, new Reservation("RES0002", "MEM001", DateTime.Today.AddDays(0).AddHours(0).AddMinutes(0), DateTime.Today.AddDays(0).AddHours(3).AddMinutes(0), "Auto", "P001", "AU005"));
        ReservationDetails.CreerReservation(2, new Reservation("RES0003", "MEM001", DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), DateTime.Today.AddDays(0).AddHours(5).AddMinutes(0), "Auto", "P001", "AU002"));
        ReservationDetails.CreerReservation(3, new Reservation("RES0004", "MEM001", DateTime.Today.AddDays(0).AddHours(1).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(3).AddMinutes(30), "Auto", "P002", "AU011"));
        ReservationDetails.CreerReservation(4, new Reservation("RES0005", "MEM001", DateTime.Today.AddDays(0).AddHours(5).AddMinutes(0), DateTime.Today.AddDays(0).AddHours(7).AddMinutes(0), "Auto", "P006", "AU023"));
        ReservationDetails.CreerReservation(5, new Reservation("RES0006", "MEM001", DateTime.Today.AddDays(0).AddHours(3), DateTime.Today.AddDays(0).AddHours(4).AddMinutes(30), "Auto", "P006", "AU023"));
        ReservationDetails.CreerReservation(6, new Reservation("RES0007", "MEM001", DateTime.Today.AddDays(1).AddHours(1), DateTime.Today.AddDays(1).AddHours(3).AddMinutes(0), "Auto", "P006", "AU023"));
        ReservationDetails.CreerReservation(7, new Reservation("RES0008", "MEM001", DateTime.Today.AddDays(2).AddHours(3), DateTime.Today.AddDays(2).AddHours(4).AddMinutes(0), "Auto", "P006", "AU023"));
        ReservationDetails.CreerReservation(8, new Reservation("RES0009", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Auto", "P007", "AU027"));
        ReservationDetails.CreerReservation(9, new Reservation("RES0010", "MEM001", DateTime.Today.AddDays(0).AddHours(1).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(30), "Auto", "P008", "AU036"));
        ReservationDetails.CreerReservation(10, new Reservation("RES0011", "MEM001", DateTime.Today.AddDays(0).AddHours(0).AddHours(30), DateTime.Today.AddDays(0).AddHours(1).AddMinutes(0), "Auto", "P009", "AU041"));
        ReservationDetails.CreerReservation(11, new Reservation("RES0012", "MEM001", DateTime.Today.AddDays(0).AddHours(0).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(1).AddMinutes(0), "Auto", "P013", "AU062"));
        ReservationDetails.CreerReservation(12, new Reservation("RES0013", "MEM001", DateTime.Today.AddDays(0).AddHours(0).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(1).AddMinutes(0), "Auto", "P015", "AU074"));
        ReservationDetails.CreerReservation(13, new Reservation("RES0014", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Auto", "P011", "AU049"));
        ReservationDetails.CreerReservation(14, new Reservation("RES0015", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Auto", "P011", "AU0055"));
        ReservationDetails.CreerReservation(15, new Reservation("RES0016", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Auto", "P012", "AU056"));
        ReservationDetails.CreerReservation(16, new Reservation("RES0017", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Auto", "P009", "AU038"));
        ReservationDetails.CreerReservation(17, new Reservation("RES0018", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Auto", "P009", "AU039"));
        ReservationDetails.CreerReservation(18, new Reservation("RES0019", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Auto", "P006", "AU024"));
        ReservationDetails.CreerReservation(19, new Reservation("RES0020", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Auto", "P006", "AU025"));
        ReservationDetails.CreerReservation(20, new Reservation("RES0021", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Auto", "P006", "AU026"));
        ReservationDetails.CreerReservation(21, new Reservation("RES0022", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Auto", "P007", "AU028"));
        ReservationDetails.CreerReservation(22, new Reservation("RES0023", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Auto", "P007", "AU029"));
        ReservationDetails.CreerReservation(23, new Reservation("RES0024", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Auto", "P009", "AU042"));
        ReservationDetails.CreerReservation(24, new Reservation("RES0025", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Auto", "P010", "AU043"));
        ReservationDetails.CreerReservation(25, new Reservation("RES0026", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Auto", "P010", "AU044"));
        ReservationDetails.CreerReservation(26, new Reservation("RES0027", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Auto", "P010", "AU045"));
        ReservationDetails.CreerReservation(27, new Reservation("RES0028", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Auto", "P010", "AU046"));
        ReservationDetails.CreerReservation(28, new Reservation("RES0029", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Auto", "P010", "AU047"));
        ReservationDetails.CreerReservation(29, new Reservation("RES0030", "MEM001", DateTime.Today.AddDays(0).AddHours(0), DateTime.Today.AddDays(0).AddHours(1).AddMinutes(0), "Moto", "P001", "M01"));
        ReservationDetails.CreerReservation(30, new Reservation("RES0031", "MEM001", DateTime.Today.AddDays(0).AddHours(0), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(30), "Moto", "P015", "M16"));
        ReservationDetails.CreerReservation(31, new Reservation("RES0032", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Moto", "P001", "M02"));
        ReservationDetails.CreerReservation(32, new Reservation("RES0033", "MEM001", DateTime.Today.AddDays(0).AddHours(2), DateTime.Today.AddDays(0).AddHours(5).AddMinutes(0), "Moto", "P003", "M05"));
        ReservationDetails.CreerReservation(33, new Reservation("RES0034", "MEM001", DateTime.Today.AddDays(1).AddHours(0), DateTime.Today.AddDays(1).AddHours(2).AddMinutes(0), "Moto", "P003", "M05"));
        ReservationDetails.CreerReservation(34, new Reservation("RES0035", "MEM001", DateTime.Today.AddDays(1).AddHours(2), DateTime.Today.AddDays(1).AddHours(3).AddMinutes(0), "Moto", "P013", "M12"));
        ReservationDetails.CreerReservation(35, new Reservation("RES0036", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Moto", "P015", "M15"));
        ReservationDetails.CreerReservation(36, new Reservation("RES0037", "MEM001", DateTime.Today.AddDays(0).AddHours(0), DateTime.Today.AddDays(0).AddHours(3).AddMinutes(0), "Velo", "P001", "V01"));
        ReservationDetails.CreerReservation(37, new Reservation("RES0038", "MEM001", DateTime.Today.AddDays(0).AddHours(5).AddMinutes(30), DateTime.Today.AddDays(0).AddHours(7).AddMinutes(30), "Velo", "P001", "V01"));
        ReservationDetails.CreerReservation(38, new Reservation("RES0039", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Velo", "P003", "V03"));
        ReservationDetails.CreerReservation(39, new Reservation("RES0040", "MEM001", DateTime.Today.AddDays(0).AddHours(5), DateTime.Today.AddDays(0).AddHours(8).AddMinutes(0), "Velo", "P003", "V03"));
        ReservationDetails.CreerReservation(40, new Reservation("RES0041", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(3).AddMinutes(0), "Velo", "P006", "V06"));
        ReservationDetails.CreerReservation(41, new Reservation("RES0042", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Velo", "P007", "V07"));
        ReservationDetails.CreerReservation(42, new Reservation("RES0043", "MEM001", DateTime.Today.AddDays(0).AddHours(0), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Velo", "P008", "V08"));
        ReservationDetails.CreerReservation(43, new Reservation("RES0044", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Velo", "P009", "V09"));
        ReservationDetails.CreerReservation(44, new Reservation("RES0045", "MEM001", DateTime.Today.AddDays(0).AddHours(5), DateTime.Today.AddDays(0).AddHours(7).AddMinutes(0), "Velo", "P009", "V09"));
        ReservationDetails.CreerReservation(45, new Reservation("RES0046", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(2).AddMinutes(0), "Velo", "P010", "V10"));
        ReservationDetails.CreerReservation(46, new Reservation("RES0047", "MEM001", DateTime.Today.AddDays(0).AddHours(3), DateTime.Today.AddDays(0).AddHours(4).AddMinutes(0), "Velo", "P010", "V10"));
        ReservationDetails.CreerReservation(47, new Reservation("RES0048", "MEM001", DateTime.Today.AddDays(1).AddHours(2), DateTime.Today.AddDays(1).AddHours(5).AddMinutes(0), "Velo", "P011", "V11"));
        ReservationDetails.CreerReservation(48, new Reservation("RES0049", "MEM001", DateTime.Today.AddDays(0).AddHours(1), DateTime.Today.AddDays(0).AddHours(5).AddMinutes(0), "Velo", "P012", "V12"));
        ReservationDetails.CreerReservation(49, new Reservation("RES0050", "MEM001", DateTime.Today.AddDays(2).AddHours(0), DateTime.Today.AddDays(2).AddHours(2).AddMinutes(0), "Velo", "P012", "V12"));

        // Past Reservations
        ReservationDetails.CreerReservation(0, new Reservation("RES0051", "MEM007", new DateTime(2025,03,11,10,30,0), new DateTime(2025, 03, 11, 11, 30, 0), "Auto", "P001", "AU001"));
        ReservationDetails.CreerReservation(0, new Reservation("RES0052", "MEM007", new DateTime(2025, 03, 15, 14, 0, 0), new DateTime(2025, 03, 15, 16, 30, 0), "Velo", "P002", "V02"));
        ReservationDetails.CreerReservation(0, new Reservation("RES0053", "MEM007", new DateTime(2025, 03, 17, 10, 00, 0), new DateTime(2025, 03, 17, 11, 30, 0), "Moto", "P015", "M15"));
        ReservationDetails.CreerReservation(0, new Reservation("RES0054", "MEM001", new DateTime(2025, 03, 19, 10, 30, 0), new DateTime(2025, 03, 11, 11, 30, 0), "Auto", "P001", "AU001"));
        ReservationDetails.CreerReservation(0, new Reservation("RES0055", "MEM005", new DateTime(2025, 03, 20, 10, 30, 0), new DateTime(2025, 03, 11, 11, 30, 0), "Auto", "P001", "AU001"));
    }

   //List of all vehicules
    Vehicule[] myVehicules = new Vehicule[110];

     // Method Create a Vehicule and add it to list of all vehicules
    public async void CreerVehicule(int index, Vehicule vehicule)
    {
        myVehicules[index] = vehicule;
        await _dbContext.CreateAsync(vehicule);
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
        _dbContext.Vehicules.Clear(); // Clear the Vehicules CollectionView
        StationDetails.selectedStationID.Clear();
        // Iterate over all stations to verify selected station(s)
        for (int i = myStations.Length - 1; i >= 0; i--)
        {
            if ((myStations[i] != null) && (!_dbContext.Stations.Contains(myStations[i])) && (ReservationSearchDetails.StationAddress == "All Stations"))
            {
                StationDetails.selectedStationID.Add(myStations[i].StationId);
            }
            else if ((myStations[i] != null) && (!_dbContext.Stations.Contains(myStations[i])) && (myStations[i].StationAddress == ReservationSearchDetails.StationAddress))
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

        bool allValuesInList = addOptions.All(item => vehicule.AutoOptions.Contains(item));
        bool AnyValueInList = addOptions.Any(item => vehicule.AutoOptions.Contains(item));
        bool containsAnyValue = removalOptions.Any(item => vehicule.AutoOptions.Contains(item));

        //bool containsValueChecked = vehicule.AutoOptions.Contains(optionsChecked);
        bool containsNoOption = vehicule.AutoOptions.Count == 0;

        // If option was just unchecked, don't add cars with that option
        if (containsAnyValue)
            return false;

        // If vehicule has no options and no option is checked, it is added
        if (addOptions.Count == 0)
        {
            if (containsNoOption) return true;
            return false;

        }
        // allValuesInList : each vehicule must have ALL checked options, or
        // AnyValueInList : each vehicule must have ONE of the checked options
        if (allValuesInList && !containsNoOption)
        {
            return true;
        }
        return false;
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
                    _dbContext.Vehicules.Add(vehicule);
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
        int indexRes = ReservationDetails.myReservations.Length-1;
        string resID;
        // ID for Logged in member, will need to be changed to retrieve MemberID from MembreDetails
        string currentMemberID = "MEM007";

        if (indexRes < 100)
        {
            resID = "RES00" + (indexRes).ToString();
        }
        else
        {
            resID = "RES0" + (indexRes).ToString();
        }
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
            ReservationDetails.CreerReservation(indexRes, new Reservation(resID, currentMemberID, ReservationSearchDetails.RequestedStartTime, ReservationSearchDetails.RequestedEndTime, vehicule));
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
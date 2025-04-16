
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
    private string welcomeMessage;
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
    private bool isOptions;
    [ObservableProperty]
    private string categorieAuto;
    private bool isLoading = false;

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
    public Login LoginDetails { get; set; }
    public ReservationResult ResultDetails { get; set; }
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
    protected virtual async Task<bool> IsCarAvailable(ObservableCollection<Reservation> reservations, int vehiculeID, DateTime newStartTime, DateTime newEndTime)
    {
        var AllReservations = await _dbContext.GetReservationsAsync();
        // Check for overlap with existing reservations for the same car
        foreach (Reservation reservation in AllReservations)
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
        _dbContext = dbContext;
        //SetWelcomeMessage();

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
        LoginDetails = new Login();
        MembreDetails = new Membre();



        // Initialize some options
        StartDate = DateTime.Now.Date;
        EndDate = DateTime.Now.Date;
        ReservationSearchDetails.RequestedStartTime = StartDate.Add(StartTime);
        ReservationSearchDetails.RequestedEndTime = EndDate.Add(EndTime);
        ReservationSearchDetails.TypeVehicule = "Auto";
        IsAutoSelected = true;
        IsOptions = false;
        ReservationSearchDetails.StationAddress = "All Stations";
        ReservationSearchDetails.CategorieAuto = "Essence";

        InitRes();
    }
    public async Task InitRes()
    {
        await AddVehiculesBasedOnAllUserInputs();   // Populate the CollectionView with vehicules according to initial conditions
    }

    public async void CreerVehicule(string type, string vehiculeID, int stationID, string categorie = null, List<string> carOptions = null)
    {
        if (type == "Auto")
        {
            // Create the auto
            var vehicule = new Auto(stationID, categorie, carOptions);
            // Insert the auto in the database
            await _dbContext.CreateAsync(vehicule);
            Console.WriteLine($"Inserted Vehicule with Id: {vehicule.vehiculeId}");
            // Insert AutoOptions into the AutoOption table
            foreach (var option in carOptions)
            {
                var autoOption = new AutoOption
                {
                    Option = option,
                    AutoId = vehicule.vehiculeId  // Foreign key reference to the Auto
                };
                await _dbContext.CreateAsync(autoOption);
            }
        }
        else if (type == "Velo")
        {
            var vehicule = new Velo(stationID);
            await _dbContext.CreateAsync(vehicule);
            Console.WriteLine($"Inserted Vehicule with Id: {vehicule.vehiculeId}");
        }
        else if (type == "Moto")
        {
            var vehicule = new Moto(stationID);
            await _dbContext.CreateAsync(vehicule);
            Console.WriteLine($"Inserted Vehicule with Id: {vehicule.vehiculeId}");
        }
    }
    //List of all stations
    Station[] myStations = new Station[20];
    // Method to create a Station and add it to list of all stations
    public async void CreerStation(string address, int spaces, int bikeSpaces)
    {
        //myStations[index] = new Station(index, id, address, spaces, bikeSpaces);
        await _dbContext.CreateAsync(new Station(address, spaces, bikeSpaces));
    }
    public async Task CreerReservation(int memberid, DateTime requestedStartTime, DateTime requestedEndTime, Vehicule vehicule)
    {
        var reservation = new Reservation(memberid, requestedStartTime, requestedEndTime, vehicule);
        await _dbContext.CreateAsync(reservation);
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
        if (!value) AddVehiculesBasedOnAllUserInputs("MP3");
        else AddVehiculesBasedOnAllUserInputs();
    }
    partial void OnIsCheckedACChanged(bool value)
    {
        if (!value) AddVehiculesBasedOnAllUserInputs("AC");
        else AddVehiculesBasedOnAllUserInputs();
    }
    partial void OnIsCheckedGPSChanged(bool value)
    {
        if (!value) AddVehiculesBasedOnAllUserInputs("GPS");
        else AddVehiculesBasedOnAllUserInputs();
    }
    partial void OnIsCheckedChildSeatChanged(bool value)
    {
        if (!value) AddVehiculesBasedOnAllUserInputs("ChildSeat");
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

        if (ReservationSearchDetails.RequestedEndTime < ReservationSearchDetails.RequestedStartTime)
        {
            if (IsBusy == true)
                IsBusy = false;
            string message = "The End Time cannot be BEFORE the Start Time! \n\n Please enter a valid time.";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
        else if (ReservationSearchDetails.RequestedStartTime < DateTime.Now)
        {
            if (IsBusy == true)
                IsBusy = false;
            string message = "The Start time cannot be before now! \n\n Please enter a valid time.";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
        else if (EndDate < StartDate)
        {
            if (IsBusy == true)
                IsBusy = false;
            string message = "The End Date cannot be BEFORE the Start Date! \n\n Please enter a valid Date.";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
        else if (interval > threshold6Hours)
        {
            if (IsBusy == true)
                IsBusy = false;
            string message = "RentARide only provides short-time rentals! \n\n Please enter a time interval of less than 6 hours.";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
        else if (interval < threshold30Mins)
        {
            if (IsBusy == true)
                IsBusy = false;
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
        //if (StartDate < DateTime.Now.Date)
        //{
        //    string message = "You cannot enter a Date before Today! \n\n Please enter a valid Date.";
        //    await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        //}
    }
    private async void OnEndDateChanged()
    {
        Console.WriteLine(EndDate);
        ReservationSearchDetails.RequestedEndTime = EndDate.Add(EndTime);
        await Task.Yield();
        //if (EndDate < StartDate)
        //{
        //    string message = "The End Date cannot be BEFORE the Start Date! \n\n Please enter a valid Date.";
        //    await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        //}
    }

    // Main method to filter vehicules based on criteria
    public async Task<bool> AddVehiculesBasedOnAllUserInputs(string optionsChecked = "")
    {
        IsBusy = true;
        Console.WriteLine("AddVehiculesBasedOnAllUserInputs() called");
        if (isLoading)
        {
            Console.WriteLine("Skipping because method is already running.");
            return false;
        }
        Vehicules.Clear(); // Clear the Vehicules CollectionView

        isLoading = true;

        Console.WriteLine("AddVehiculesBasedOnAllUserInputs() started");
        // Fetch stations from the database based on the user's selected station address

        var stations = await _dbContext.GetStationsAsync();
        if (ReservationSearchDetails.StationAddress != "All Stations")
        {
            stations = stations.Where(station => station.StationAddress == ReservationSearchDetails.StationAddress).ToList();
        }
        var selectedStations = stations.ToList();
        _dbContext.selectedStationID.Clear(); // Clear selected stations
                                                
        foreach (var station in selectedStations)
        {
            _dbContext.selectedStationID.Add(station.StationId); // Add the selected stations to the StationDetails list
        }
        List<Vehicule> selectedVehicles;
        // Fetch vehicles from the database
        var AllVehicules = await _dbContext.GetVehiculesAsync();
        Console.WriteLine($"Number of vehicules in AllVehicules {AllVehicules.Count}");

        // Filter by vehicle type

        // Filter the list using LINQ if it's already in memory (List<Vehicle>)
        selectedVehicles = AllVehicules.Where(v => v.type == ReservationSearchDetails.TypeVehicule).ToList();
        Console.WriteLine($"Number of vehicules in SelectedVehicules {selectedVehicles.Count}");

        // Iterate through vehicles and apply checks
        foreach (var vehicule in selectedVehicles)
        {
            Console.WriteLine($"Processing vehicule ID: {vehicule?.vehiculeId}");
            if (vehicule != null)
            {
                // If the selected vehicle type is "Auto"
                if (ReservationSearchDetails.TypeVehicule == "Auto")
                {
                    if (IsAutoSelected == false)
                        IsAutoSelected = true;
                    string selectedCategory = CategorieAuto;
                    // Check if the vehicle is of type "Auto" and matches the category and options
                    if (vehicule.categorieAuto == selectedCategory)
                    {
                        if (await CheckOptions(vehicule, optionsChecked))
                                //if (vehicule.categorieAuto == selectedCategory)
                        {
                            await CheckStation(vehicule);
                        }
                    }
                }
                else
                {
                    if (IsAutoSelected == true)
                        IsAutoSelected = false;
                    // If the selected type is not "Auto", simply check the station
                    await CheckStation(vehicule);
                }
            }
        }
        isLoading = false;
        Console.WriteLine("AddVehiculesBasedOnAllUserInputs() finished");
        IsBusy = false;
        return true;
    }

    private async Task<bool> CheckCategorieAuto(Vehicule vehicule)
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
    private async Task<bool> CheckOptions(Vehicule vehicule, string checkedOption = "")
{
    // Get the selected options from the UI
    HashSet<string> addOptions = new HashSet<string>();
    HashSet<string> removalOptions = new HashSet<string>();

    // Assuming that IsCheckedMP3, IsCheckedAC, etc. are boolean flags based on user selections
    if (IsCheckedMP3) addOptions.Add("MP3");
    if (IsCheckedAC) addOptions.Add("AC");
    if (IsCheckedGPS) addOptions.Add("GPS");
    if (IsCheckedChildSeat) addOptions.Add("ChildSeat");

    if (vehicule ==  null)
            return false;
        if (_dbContext._dbConnection == null)
        {
            Console.WriteLine("Database connection is null.");
            string message = "There is an issue with the database connection";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
        if (!string.IsNullOrEmpty(checkedOption))
        {
            removalOptions.Add(checkedOption);
            IsOptions = false;
        }
        else
        {
            IsOptions = true;
        }

        // Fetch the AutoOptions from the database for the current vehicle (Auto)
        //await _dbContext.TestDatabaseConnection();
        //if (vehicule == null)
        //    Console.WriteLine("Vehicule is null!");
    var autoOptions = await _dbContext._dbConnection
        .Table<AutoOption>()
        .Where(option => option.AutoId == vehicule.vehiculeId)  // Make sure the AutoOption belongs to this Auto
        .ToListAsync();
    // Convert the list of AutoOptions to a HashSet for easy comparison
    HashSet<string> vehicleOptions = new HashSet<string>(autoOptions.Select(option => option.Option));

    // Determine if the vehicle has all the selected options
    bool allValuesInList = addOptions.All(item => vehicleOptions.Contains(item));
    bool anyValueInList = addOptions.Any(item => vehicleOptions.Contains(item));
    bool containsAnyValue = removalOptions.Any(item => vehicleOptions.Contains(item));

        // Convert the AutoOptions to a list of strings (this will be used for CarOptions)
        List<string> vehicleOpts = autoOptions.Select(option => option.Option).ToList();

        // Set CarOptions to the current Vehicule (in-memory, won't be saved in DB)
        vehicule.CarOptions = vehicleOpts;

        if (vehicleOptions.Count == 0)
            IsOptions = false; 
        else
            IsOptions = true;

        Console.WriteLine($"VehiculeID: {vehicule.vehiculeId} options: {string.Join(", ", vehicule.CarOptions)}");
    // If any removal option is selected, the vehicle should be excluded
    if (containsAnyValue)
            return false;
    // If no options are checked, and the vehicle has no options, it's valid
    else if (addOptions.Count == 0 && vehicleOptions.Count == 0)
            return true;
    else if (addOptions.Count == 0 && vehicleOptions.Count > 0)
            return false;
            // If all the selected options are present and the vehicle doesn't have any options, it's valid
    else if (allValuesInList && vehicleOptions.Count > 0)
            return true;
    else
            return false;
}
    private async Task<bool> CheckStation(Vehicule vehicule)
    {
        // Check if vehicule (from myVehicules[i] above) is at a selected station & if it is available
        foreach (int station in _dbContext.selectedStationID)
        {
            Console.WriteLine($"CheckStation on vehicule with ID {vehicule?.vehiculeId} at {vehicule?.vehiculeStationId} and station with ID {station}");
            if (vehicule.vehiculeStationId == station)
            {
                if (await IsCarAvailable(ReservationDetails.Reservations, vehicule.vehiculeId, ReservationSearchDetails.RequestedStartTime, ReservationSearchDetails.RequestedEndTime))
                {
                    // Add the vehicule directly to the CollectionView
                    Vehicules.Add(vehicule);
                    Console.WriteLine($"vehicule with ID {vehicule?.vehiculeId} has been added to Vehicules");
                    Console.WriteLine($"Number of vehicules : {Vehicules.Count}");
                    return true;
                }
            }
        }
        return true;
    }
    private async void Reserve(Vehicule vehicule)
    {
        IsBusy = true;
        Console.WriteLine("Reserve called for vehicule: " + vehicule.vehiculeId);
        Console.WriteLine("Vehicule debug:");
        Console.WriteLine($"ID: {vehicule?.vehiculeId}");
        Console.WriteLine($"Station ID: {vehicule?.vehiculeStationId}");
        Console.WriteLine($"Type: {vehicule?.type}");
        Console.WriteLine($"Categorie Auto: {vehicule?.categorieAuto}");
        Console.WriteLine($"AutoOptions: {(vehicule?.AutoOptions != null ? string.Join(", ", vehicule.AutoOptions) : "null")}");
        // ID for Logged in member, will need to be changed to retrieve MemberID from MembreDetails
        var ActiveMemberID = await _dbContext.GetLoggedInMemberIdAsync();
        Console.WriteLine("ActiveMemberID is " + ActiveMemberID);

        TimeSpan interval = ReservationSearchDetails.RequestedEndTime - ReservationSearchDetails.RequestedStartTime;
        TimeSpan threshold6Hours = TimeSpan.FromHours(6);
        TimeSpan threshold30Mins = TimeSpan.FromMinutes(30);
        await Task.Yield();
        if (ReservationSearchDetails.RequestedEndTime < ReservationSearchDetails.RequestedStartTime)
        {
            string message = "The End Time cannot be BEFORE the Start Time! \n\n Please enter a valid time.";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
        //else if (ReservationSearchDetails.RequestedStartTime < DateTime.Now)
        //{
        //    string message = "The Start time cannot be before now! \n\n Please enter a valid time.";
        //    await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        //}
        else if (EndDate < StartDate)
        {
            if (IsBusy == true)
                IsBusy = false;
            string message = "The End Date cannot be BEFORE the Start Date! \n\n Please enter a valid Date.";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
        else if (interval > threshold6Hours)
        {
            {
                if (IsBusy == true)
                    IsBusy = false;
                string message = "RentARide only provides short-time rentals! \n\n Please enter a time interval of less than 6 hours.";
                await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
            }
        }
        else if (interval < threshold30Mins)
        {
            if (IsBusy == true)
                IsBusy = false;
            string message = "The minimum rental period is 30 minutes! \n\n Please enter a valid time interval.";
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
        else
        {
            await _dbContext.CreerReservation(ActiveMemberID.Value, ReservationSearchDetails.RequestedStartTime, ReservationSearchDetails.RequestedEndTime,vehicule);
            if (await _dbContext.OnReservationAdded())
            {
                if (await AddVehiculesBasedOnAllUserInputs())
                {
                    Console.WriteLine($"Number of past reservations is {_dbContext.ReservationsResultPast.Count}");
                    Console.WriteLine($"Number of current reservations is {_dbContext.ReservationsResultCurrent.Count}");
                    IsBusy = false;
                    await Shell.Current.GoToAsync("Mainpage");
                    Console.WriteLine($"Number of current reservations is {_dbContext.ReservationsResultCurrent.Count}");
                }
            }
        }
    }
    private async void SetWelcomeMessage()
    {
        await ApplicationDbContext.Instance.SetWelcomeMessageAsync();
        WelcomeMessage = ApplicationDbContext.Instance.WelcomeMessage;
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
    private async Task BackToMainPage()
    {
        await Shell.Current.GoToAsync("Mainpage");
    }
    [RelayCommand]
    private async Task Logout()
    {
        var ActiveMemberID = await _dbContext.GetLoggedInMemberIdAsync();
        await _dbContext.LogoutAsync(ActiveMemberID.Value);
        await Shell.Current.GoToAsync("Loginpage");
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace RentARideDB.Models
{
    public partial class Reservation_BASE : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int ReservationID { get; set; }
        [ObservableProperty]
        private DateTime startDate;
        [ObservableProperty]
        private DateTime endDate;
        [ObservableProperty]
        private string typeVehicule;
        [ObservableProperty]
        private string categorieAuto;
        [ObservableProperty]
        private int stationId;
        [ObservableProperty]
        private string stationAddress;
        [ObservableProperty]
        private int vehiculeID; // Foreign key to Vehicule
        [ObservableProperty]
        private int memberID;
        //[ObservableProperty]
        //public string autoOptionsString;
        //public bool IsChecked { get; set; }
        //[ObservableProperty]
        //public List<string> autoOptions;
        [Ignore]
        public ObservableCollection<Reservation> Reservations { get; } = new();
        //[ObservableProperty]
        //public List<Vehicule> searchResults = new();
        public Reservation_BASE()
        {
            
        }

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RentARideDB.Models
{
   
    public partial class Station : ObservableObject
    {
        [ObservableProperty] public string stationId;
        [ObservableProperty] private string stationAddress;
        [ObservableProperty] private int parkSpaces;
        [ObservableProperty] private int occupiedSpaces;
        [ObservableProperty] private int freeSpaces;
        [ObservableProperty] private int bikeSpaces;
        [ObservableProperty] private int freeBikeSpaces;
        [ObservableProperty] private int occupiedbikeSpaces;
        private int index;
        public List<string> selectedStationID = new();

        public Station()
        {
            
        }
        public Station(int index,string id, string address, int spaces, int bikeSpaces)
        {
            this.stationId = id;
            this.stationAddress = address;
            this.parkSpaces = spaces;
            this.index = index;
            this.bikeSpaces = bikeSpaces;
            
        }
        //public Station StationDetails { get; set; }
        //Station[] myStations = new Station[10];
        //public void CreerStation(string id, string address, int spaces)
        //{
        //    myStations[index] = new Station (id, address, spaces);
        //}
    }
}

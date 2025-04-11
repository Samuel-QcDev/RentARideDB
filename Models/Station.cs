using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace RentARideDB.Models
{
   
    public partial class Station : ObservableObject
    {
        [PrimaryKey, AutoIncrement] 
        public int StationId {  get; set; }
        [ObservableProperty] private string stationAddress;
        [ObservableProperty] private int parkSpaces;
        [ObservableProperty] private int occupiedSpaces;
        [ObservableProperty] private int freeSpaces;
        [ObservableProperty] private int bikeSpaces;
        [ObservableProperty] private int freeBikeSpaces;
        [ObservableProperty] private int occupiedbikeSpaces;
        private int index;
        //[Ignore]
        //public List<int> selectedStationID { get; set; } = new();

        public Station()
        {
            
        }
        public Station(string address, int spaces, int bikeSpaces)
        {
            this.stationAddress = address;
            this.parkSpaces = spaces;
            //this.index = index;
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

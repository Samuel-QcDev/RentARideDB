using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Reflection;
using System.Collections.ObjectModel;
using RentARideDB.Tools;
using SQLite;

namespace RentARideDB.Models
{
    public partial class Reservation : Reservation_BASE
    {
        [PrimaryKey, AutoIncrement]
        public int ReservationID { get; set; }
        public int BikeReturnStationID { get; set; }
        [ObservableProperty]
        private DateTime startTime;
        [ObservableProperty]
        private DateTime endTime;




        public Reservation()
        {
            
        }
        public Reservation(string memberid, DateTime requestedStartTime, DateTime requestedEndTime, string typeVehicule, string stationID, string vehiculeId)
        {
            this.MemberID = memberid;
            this.StartTime = Utils.RoundToNearest30Minutes(requestedStartTime);
            this.EndTime = Utils.RoundToNearest30Minutes(requestedEndTime);
            this.TypeVehicule = typeVehicule;
            this.StationId = stationID;
            this.VehiculeID = vehiculeId;
        }
        public Reservation(string memberid, DateTime requestedStartTime, DateTime requestedEndTime, Vehicule vehicule)
        {
            this.MemberID = memberid;
            this.StartTime = Utils.RoundToNearest30Minutes(requestedStartTime);
            this.EndTime = Utils.RoundToNearest30Minutes(requestedEndTime);
            this.TypeVehicule = vehicule.type;
            this.StationId = vehicule.vehiculeStationId;
            this.VehiculeID = vehicule.vehiculeId; // Set the VehiculeID from the vehicule object (handle null case)
        }

        //public Reservation[] myReservations = new Reservation[100];


        //[RelayCommand]
        //private async Task Reserve()
        //{
        //    int indexRes = Reservations.Count;
        //    string resID;
        //    // ID for Logged in member, will need to be changed to retrieve MemberID from MembreDetails
        //    string currentMemberID = "MEM007";
            
        //    if (indexRes < 100)
        //    {
        //        resID = "RES00" + (indexRes).ToString();
        //    }
        //    else
        //    {
        //        resID = "RES0" + (indexRes).ToString();
        //    }

        //    CreerReservation(indexRes, new Reservation(resID, currentMemberID, requestedStartTime, requestedEndTime, );
        //    await Shell.Current.GoToAsync("Mainpage");//Change this code for a method to add current reservation to MyReservationsList
        //}

        //public Reservation(DateTime date, int hreDebut, int minsDebut, 
        //    int hreFin, int minsFin, string type, string station, 
        //    [ Optional ] string categorie, [ Optional ] Enum options)
        //{
        //    StartTime = new DateTime(date.Year, date.Month, date.Day, hreDebut, minsDebut, 0);
        //    EndTime = new DateTime(date.Year, date.Month, date.Day, hreFin, minsFin, 0);
        //    TypeVehicule = type;
        //    CategorieAuto = categorie;
        //    StationId = station;
        //    Options = options;


        //}
    }
}

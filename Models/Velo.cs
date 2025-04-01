using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RentARideDB.Models

{
    public partial class Velo : Vehicule
    {
        public int BikeReturnStationID { get; set; }

        public Velo()
        {

        }
        public Velo(string stationID)
        {
            this.type = "Velo";
            this.vehiculeStationId = stationID;
        }
        public override string ToString()
        {
            return type + " " + vehiculeId+ " " + vehiculeStationId;
        }
    }
}



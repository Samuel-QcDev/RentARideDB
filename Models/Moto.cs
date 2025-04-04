using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RentARideDB.Models
{

    public partial class Moto : Vehicule
    {
        public Moto() 
        { 
        }
        public Moto(int stationID)
        {
            this.type = "Moto";
            this.vehiculeStationId = stationID;
        }
        public override string ToString()
        {
            return type + " " + vehiculeId + " " + vehiculeStationId;
        }
    }
}

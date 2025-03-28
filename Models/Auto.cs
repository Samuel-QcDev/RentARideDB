using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RentARideDB.Models
{
    public partial class Auto : Vehicule
    {
        public string categorieAuto { get; set; }
        public string autoId { get; set; }


        public Auto()
        {
            
        }
        public Auto(string id, string stationID,string type, List<string> carOptions)
        {
            this.type = "Auto";
            this.vehiculeId = id;
            this.vehiculeStationId = stationID;
            this.categorieAuto = type;
            this.autoOptions = (List<string>)carOptions;
        }
        public override string ToString()
        {
            return type + " " + vehiculeId  + " " + vehiculeStationId + " " + categorieAuto + " " + String.Join(",",AutoOptions);
        }
    }
}

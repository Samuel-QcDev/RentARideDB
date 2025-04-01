using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace RentARideDB.Models
{
    public partial class Auto : Vehicule
    {
        public string categorieAuto { get; set; }
        [PrimaryKey, AutoIncrement]
        public int autoId { get; set; }

        [Ignore]  // Ignore this property for SQLite storage
        public List<string> AutoOptions { get; set; }

        public Auto()
        {
            AutoOptions = new List<string>();
        }
        public Auto(string vehiculeId, string stationID,string type, List<string> carOptions)
        {
            this.type = "Auto";
            this.vehiculeStationId = stationID;
            this.categorieAuto = type;
            this.AutoOptions = carOptions ?? new List<string>();
        }
        public override string ToString()
        {
            return type + " " + vehiculeId  + " " + vehiculeStationId + " " + categorieAuto + " " + String.Join(",",AutoOptions);
        }
    }
}

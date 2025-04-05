using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace RentARideDB.Models
{
    public partial class Vehicule : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int vehiculeId { get; set; }
        public int vehiculeStationId {  get; set; }
        [Ignore] // Prevent SQLite from mapping this property as a column
        public Station Station { get; set; } // Navigation property to Station (used for querying)
        public int StationId { get; set; } // Foreign Key column
        public string type {  get; set; }
        public string categorieAuto { get; set; }
        [Ignore]  // Ignore this property for SQLite storage
        public List<AutoOption> AutoOptions { get; set; }
        //public List<string> CarOptions { get; set; }
        public Vehicule()
        {
            //AutoOptions = new List<AutoOption>();
            //CarOptions = new List<string>();
        }

    }
}

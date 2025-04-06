using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using RentARideDB.Services;
using SQLite;

namespace RentARideDB.Models
{

    public partial class Vehicule : ObservableObject
    {
        private readonly ApplicationDbContext _dbContext;

        [PrimaryKey, AutoIncrement]
        public int vehiculeId { get; set; }
        public int vehiculeStationId {  get; set; }
        [Ignore] // Prevent SQLite from mapping this property as a column
        public Station Station { get; set; } // Navigation property to Station (used for querying)
        //public int StationId { get; set; } // Foreign Key column
        public string type {  get; set; }
        public string categorieAuto { get; set; }
        [Ignore]  // Ignore this property for SQLite storage
        public List<AutoOption> AutoOptions { get; set; }
        [Ignore]  // Ignore this property for SQLite storage
        public List<string> CarOptions { get; set; } = new List<string>();
        [Ignore]
        public string CarOptionsString
        {
            get
            {
                return CarOptions != null && CarOptions.Any()
                    ? string.Join(", ", CarOptions)
                    : "None";
            }
        }
        public Vehicule()
        {

        }
        public Vehicule(string type, int stationID)
        {
            this.type = type;
            this.vehiculeStationId = stationID;
        }
        public Vehicule(string type, int stationID, string categorie, List<string> carOptions)
        {
            this.AutoOptions = new List<AutoOption>();
            this.type = type;
            this.vehiculeStationId = stationID;
            this.categorieAuto = categorie;
        
            //CarOptionsString = string.Join(", ", this.CarOptions);
        }
        // Method to populate the CarOptions list for a specific Vehicule
        //public async Task LoadCarOptionsAsync()
        //{
        //    var autoOptions = await _dbContext._dbConnection
        //        .Table<AutoOption>()
        //        .Where(option => option.AutoId == this.vehiculeId)
        //        .ToListAsync();

        //    // Populate the CarOptions property with the list of options as strings
        //    this.CarOptions = autoOptions.Select(option => option.Option).ToList();
        //}
        public override string ToString()
        {
            if (CarOptions != null && CarOptions.Count > 0)
            {
                string carOptionsString = CarOptionsString; // Using the computed CarOptionsString
                return $"{type} ID: {vehiculeId} Station: {vehiculeStationId} {categorieAuto} Options: {carOptionsString}";
            }
            else if (CarOptions != null && CarOptions.Count == 0)
            {
                return $"{type} ID: {vehiculeId} Station: {vehiculeStationId} {categorieAuto}";
            }
            else
            {
                return $"{type} {vehiculeId} {vehiculeStationId}";
            }
        }
    }
}

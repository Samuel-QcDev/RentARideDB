using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public int Id { get; set; }
        public string vehiculeId { get; set; }
        public string vehiculeStationId {  get; set; }
        // Navigation property to access the related Station
        [Ignore] // Prevents AutoMapping for SQLite
        public Station Station { get; set; }
        public string type {  get; set; }
        //[Ignore]  // Ignore this property for SQLite storage
        //public List<AutoOption> AutoOptions { get; set; }

        //[ObservableProperty]
        //public string autoOptionsString;
        public Vehicule()
        {

        }

        //public event PropertyChangedEventHandler PropertyChanged;

        //void onPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RentARideDB.Models
{

    public partial class Vehicule : ObservableObject
    {
        public string vehiculeId;
        public string vehiculeStationId;
        public string type;
        [ObservableProperty]
        public List<string> autoOptions = new();

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

using System;
using System.Collections.Generic;
using static System.Collections.IEnumerable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using RentARideDB.ViewModel;
using RentARideDB.Views;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RentARideDB.Models
{

    public partial class ReservationSearch : Reservation_BASE
    {
        public int ReservationSearchID { get; set; }
        public int BikeReturnStationID { get; set; }

        //[ObservableProperty]
        //private TimeSpan endTime;
        [ObservableProperty]
        private DateTime requestedStartTime;
        [ObservableProperty]
        private DateTime requestedEndTime;

        public List<int> indexVehiculesToBeRemoved = new();
        public List<int> indexVehiculesToBeAdded = new();

        private void OnTimeChanged(TimeSpan newTime)
        {
            // Implement your logic to handle the time change
            // For example, update other properties or perform actions based on the new time
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}

using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace RentARideDB.Models
{
    public partial class ReservationResult : Reservation_BASE
    {
        public string ReservationResultsID { get; set; }
        [Ignore]
        public ObservableCollection<Reservation> ReservationsResult { get; } = new();


    }
}

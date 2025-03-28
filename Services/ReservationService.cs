using CommunityToolkit.Mvvm.Input;
using RentARideDB.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentARideDB.Services
{
    public class ReservationService
    {
        private static ReservationService _instance;
        public ObservableCollection<Reservation> ReservationsResultPast { get; set; }
        public ObservableCollection<Reservation> ReservationsResultCurrent { get; set; }
        public ReservationService() 
        {
            ReservationsResultPast = new ObservableCollection<Reservation>();
            ReservationsResultCurrent = new ObservableCollection<Reservation>();
        }
        List<ReservationSearch> reservationsList;
        public static ReservationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ReservationService();
                }
                return _instance;
            }
        }
        public void AddReservation(Reservation reservation)
        {
            ReservationsResultCurrent.Add(reservation);
        }
        public void CancelReservation(Reservation reservation)
        {
            ReservationsResultCurrent.Remove(reservation);
        }
        public async Task<List<ReservationSearch>> GetReservations()
        {
            if (reservationsList?.Count > 0)
                return reservationsList;

        return reservationsList;
        }
    }
}

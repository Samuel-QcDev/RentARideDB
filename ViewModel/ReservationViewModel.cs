using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentARideDB.Models;
using RentARideDB.Services;
using CommunityToolkit.Mvvm.Input;

namespace RentARideDB.ViewModel
{
    public partial class ReservationViewModel : LocalBaseViewModel
    {
        private readonly ApplicationDbContext _dbContext;
        public ReservationSearch ReservationSearchDetails { get; set; }


        public Reservation ReservationDetails { get; set; }



        public ReservationViewModel(ApplicationDbContext dbContext)
        {
            ReservationDetails = new Reservation();
        }

    }
}

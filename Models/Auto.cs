﻿using System;
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
        //public string categorieAuto { get; set; }


        //public List<string> AutoOptions { get; set; }

        public Auto()
        {
            AutoOptions = new List<AutoOption>();
            //AutoOptions = new List<string>();
        }
        public Auto(int stationID, string type, List<string> carOptions)
        {
            this.AutoOptions = new List<AutoOption>();
            this.type = "Auto";
            this.vehiculeStationId = stationID;
            this.categorieAuto = type;
            //this.CarOptions = carOptions;
        }
    }
}

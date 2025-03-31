using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace RentARideDB.Models
{

    public class Membre
    {
        [PrimaryKey, AutoIncrement]
        public int MemberID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string MemberEmail { get; set; }
        public string MemberPassword { get; set; }
        public string Level { get; set; }

        public Membre()
        {

        }
        public Membre(string firstName, string password, string email)
        {
            this.FirstName = firstName;
            this.MemberPassword = password;
            //this.Level = level;
            this.MemberEmail = email;
        }
    }
}

using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentARideDB.Models
{
    public class Session
    {
        [PrimaryKey, AutoIncrement]
        public int SessionID { get; set; }  // Unique identifier for the session
        public int MemberID { get; set; }  // MemberID foreign key to Membre
        public bool IsActive { get; set; }  // Indicates whether the session is active
        public DateTime LastLogin { get; set; }  // Timestamp of last login

        public Session() { }

        public Session(int memberId)
        {
            MemberID = memberId;
            IsActive = true;  // Mark the session as active when created
            LastLogin = DateTime.Now;  // Set the current time as the login time
        }
    }
}

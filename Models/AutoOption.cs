using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentARideDB.Models
{
    public class AutoOption
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Option { get; set; }
        public int AutoId { get; set; }  // Foreign key to Auto
        public AutoOption()
        {
            
        }
    }
}

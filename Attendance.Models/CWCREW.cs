using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Attendance.Models
{
    public class CWCREW
    {
        [Key]
        public int id { get; set; }
        public string LASTNAME { get; set; }
        public string FIRSTNAME { get; set; }
    }
}

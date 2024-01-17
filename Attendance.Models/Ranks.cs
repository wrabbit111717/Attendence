using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Attendance.Models
{
   public class Ranks
    {
        [Key]
        public int id { get; set; }
        public string rank { get; set; }
        public string rank_alt { get; set; }
    }
}

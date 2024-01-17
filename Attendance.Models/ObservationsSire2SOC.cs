using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Attendance.Models
{
    public class ObservationsSire2SOC
    {
        [Key]
        public int id { get; set; }
        public int obs_id { get; set; }
        public int soc_id { get; set; }
        public int soc_type { get; set; }
    }
}

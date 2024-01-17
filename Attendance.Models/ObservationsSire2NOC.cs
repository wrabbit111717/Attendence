using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Attendance.Models
{
    public class ObservationsSire2NOC
    {
        [Key]
        public int id { get; set; }
        public int obs_id { get; set; }
        public int noc_id{ get; set; }
        public int noc_type { get; set; }
    }
}

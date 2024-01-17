using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Attendance.Models
{
    public class ObservationsSire2Assignees
    {
        [Key]
        public int id { get; set; }
        public int? obs_id { get; set; }
        public int? assignee_id { get; set; }
        public int? rank_id { get; set; }
    }
}

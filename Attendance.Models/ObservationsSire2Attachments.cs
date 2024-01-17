using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Attendance.Models
{
   public class ObservationsSire2Attachments
    {
        [Key]
        public int id { get; set; }
        public int? obs_id { get; set; }
        public string attachment_name { get; set; }
        public byte[] attachment { get; set; }
    }
}

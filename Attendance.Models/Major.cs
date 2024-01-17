using System.ComponentModel.DataAnnotations;

namespace Attendance.Models
{
    public class Major
    {
        [Key]
        public int MajorId { get; set; }
        public string MajorName { get; set; }
        public string MajorCode { get; set; }
    }
}
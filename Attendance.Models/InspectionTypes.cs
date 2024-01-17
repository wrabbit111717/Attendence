using System.ComponentModel.DataAnnotations;

namespace Attendance.Models
{
    public class InspectionTypes
    {
        [Key]
        public int InspectionTypeId { get; set; }

        public string InspectionType { get; set; }
        public string InspectionCode { get; set; }

        public bool? bitReport { get; set; }
    }
}

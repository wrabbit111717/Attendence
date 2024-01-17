using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Attendance.Services.DTOs
{
    public class BriefcasePayloadDTO
    {
        // TODO: From zero or empty to null
        public int? Id { get; set; }
        [StringLength(450)]
        public string UserId { get; set; }
        [Required]
        public int VesselId { get; set; }
        [Required]
        public int InspectionTypeId { get; set; }
        [Required]
        public int InspectionSourceId { get; set; }
        [StringLength(64)]
        public string InspectionCode { get; set; }
        [Required]
        public DateTime VettingDate { get; set; }
        [StringLength(50)]
        public string InspectorName { get; set; }
        [Required]
        public int PortId { get; set; }
        public string Comments { get; set; }
        [Required]
        public List<int> Questionnaires { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Attendance.Services.DTOs
{
    public class BriefcaseDTO
    {
        public int Id { get; set; }
        public UserDTO User { get; set; }
        public string UserId { get; set; }
        public VesselDTO Vessel { get; set; }
        public int VesselId { get; set; }
        public InspectionTypeDTO InspectionType { get; set; }
        public int InspectionTypeId { get; set; }

        public int InspectionSourceId { get; set; }
        public string InspectionSourceCode { get; set; }
        public string InspectionCode { get; set; }
        public DateTime VettingDate { get; set; }
        public string InspectorName { get; set; }
        public int PortId { get; set; }
        public string PortName { get; set; }
        public string PortCountry { get; set; }
        public string Comments { get; set; }
        public bool Sent { get; set; }
        public ICollection<VIQInfoDTO> Questionnaires { get; set; }
    }
}

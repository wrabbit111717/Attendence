using Attendance.Services.DTOs;
using System.Collections.Generic;

namespace Attendance.Services.ViewModels.Briedcases
{
    public class BriefcaseViewModel
    {
        public List<VesselDTO> Vessels { get; set; } = new List<VesselDTO>();
        public List<InspectionTypeDTO> InspectionTypes { get; set; } = new List<InspectionTypeDTO>();
        public List<InspectionSourceDTO> InspectionSources { get; set; } = new List<InspectionSourceDTO>();
        public List<VIQInfoDTO> Questionnaires { get; set; } = new List<VIQInfoDTO>();
        public BriefcaseDTO Data { get; set; } = null;

    }
}

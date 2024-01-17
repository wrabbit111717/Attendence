using System;
using System.ComponentModel.DataAnnotations;

namespace Attendance.Services.ViewModels.VettingInfos
{
    public class ImportVettingInfoViewModel
    {
        [Required]
        public int? QId { get; set; }

        public string VesselName { get; set; }
        public string InspectorName { get; set; }

        [Required]
        public string CompanyRepresentativeName { get; set; }

        public string VettingCode { get; set; }

        [Required]
        public Guid? VetGUI { get; set; }
        public int? VesselId { get; set; }

        public string Port { get; set; }
        public DateTime VetDate { get; set; }
        public string Comments { get; set; }

        [Required]
        public Guid? VIQGUI { get; set; }

        public string InspectionType { get; set; }
        public int InspectionTypeId { get; set; }
        public int VetId { get; set; }
    }
}
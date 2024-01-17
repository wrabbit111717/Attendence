using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Attendance.Models
{
    public class VettingInfo
    {
        [Key]
        public int VetId { get; set; }

        public string InspectorName { get; set; }
        public string InspectorSirName { get; set; }
        public string Port { get; set; }
        public string Country { get; set; }
        public DateTime VetDate { get; set; }
        
        public DateTime? VetTime { get; set; }
        public string Password { get; set; }
        public string Comments { get; set; }
        public string VesselName { get; set; }
        public string VettingCode { get; set; }
        public int QId { get; set; }
        public Guid VetGUI { get; set; }
        public int InspectionTypeId { get; set; }
        public InspectionTypes InspectionType { get; set; }
        public int? VesselId { get; set; }
        public int? CountryId { get; set; }
        public int? PortId { get; set; }

        [Required]
        public string CompanyRepresentativeName { get; set; }

        public DateTime? RegistrationDate { get; set; }
        public int? MajorId { get; set; }
        public string RegisterName { get; set; }
        public int? Answered { get; set; }
        public int? Negative { get; set; }
        public int? Positive { get; set; }
        public int? SourceId { get; set; }
        public int? UserId { get; set; }
        public byte? CarriedOutStatus { get; set; }
        public byte? Status { get; set; }
    }

    public class VettingInfoDetail
    {
        public int VetId { get; set; }

        public string InspectionTypeName { get; set; }

        public string InspectorName { get; set; }
        public string InspectorSirName { get; set; }
        public string Port { get; set; }
        public string Country { get; set; }
        public DateTime? VetDate { get; set; }
        public string VetShortDate { get; set; }
        public DateTime? VetTime { get; set; }
        public string Password { get; set; }
        public string Comments { get; set; }
        public string VesselName { get; set; }
        public string VettingCode { get; set; }
        public int QId { get; set; }
        public Guid VetGUI { get; set; }
        public int InspectionTypeId { get; set; }
        public int? VesselId { get; set; }
        public int? CountryId { get; set; }
        public int? PortId { get; set; }

        public string CompanyRepresentativeName { get; set; }

        public DateTime? RegistrationDate { get; set; }
        public string RegistrationShortDate { get; set; }
        public int? MajorId { get; set; }
        public string Major { get; set; }
        public string RegisterName { get; set; }
        public int? Answered { get; set; }
        public int? Negative { get; set; }
        public int? Positive { get; set; }
        public int? SourceId { get; set; }
        public int? UserId { get; set; }
        public string QuestTitle { get; set; }
        public string QuestVersion { get; set; }
        public byte? CarriedOutStatus { get; set; }
        public byte? Status { get; set; }
        public bool IsCrawEvaluation { get; set; }
        public bool IsObservationAttchment { get; set; }
        public List<ObservationsSire2ViewModel> listObservationsSire2 { get; set; }
        public List<ObservationsSire2Assignees> listObservationsSire2Assignees { get; set; }
        public List<ObservationsSire2Attachments> listObservationsSire2Attachments { get; set; }
    }
}
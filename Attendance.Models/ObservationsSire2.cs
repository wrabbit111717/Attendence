using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Attendance.Models
{
    public class ObservationsSire2
    {
        [Key]
        public int id { get; set; }
        public int? vetid { get; set; }
        public int? qid { get; set; }
        public Guid? objectid { get; set; }
        public int? part_id { get; set; }
        public string obs_text { get; set; }
        public int? answer_id { get; set; }
        public int? significance_id { get; set; }
        public int? solved_onboard { get; set; }
        public int? verified { get; set; }
    }
    public class ObservationsSire2ViewModel
    {
        [Key]
        public int id { get; set; }
        public int? vet_id { get; set; }
        public int? qid { get; set; }
        public int? verified { get; set; }
        public Guid? objectid { get; set; }
        public string obs_text_full { get; set; }
    }
    public class ObservationInfo
    {
        public List<ObservationsSire2ViewModel> listObservationsSire2 { get; set; }
        public List<ObservationsSire2Assignees> listObservationsSire2Assignees { get; set; }
        public List<ObservationsSire2Attachments> listObservationsSire2Attachments { get; set; }
    }
    public class ObservationAssigneeInfo
    {
        public int ObservationID { get; set; }
        public int? VetID { get; set; }
        public int? pageNumber { get; set; }
        public string OnservationText { get; set; }
        public List<AssigneeViewModel> listAssigneeViewModel { get; set; }
    }
    public class ObservationNoc
    {
        public int ObservationID { get; set; }
        public string HumanNOCPifName { get; set; }
        public string ProcessNOCNoc { get; set; }
        public string HardwareCauseAnalysisSire2Noc { get; set; }
    }

    public class ObservationSoc
    {
        public int ObservationID { get; set; }
        public string HumanSOCRank { get; set; }
        public string ProcessSOCLevel1 { get; set; }
        public string ProcessSOCLevel2 { get; set; }
        public string ProcessSOCLevel3 { get; set; }
        public string ProcessSOCLevel4 { get; set; }
        public string HardwareCCodingSire2SOCLevel1 { get; set; }
        public string HardwareCCodingSire2SOCLevel2 { get; set; }
        public string HardwareCCodingSire2SOCLevel3 { get; set; }
    }

    public class AssigneeViewModel
    {
        public Int32 AssigneeID { get; set; }
        public string AssigneeFullName { get; set; }
        public string OfficerName { get; set; }
    }
}

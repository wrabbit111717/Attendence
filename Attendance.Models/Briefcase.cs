using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Models
{
    public class Briefcase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(450)]
        public string UserId { get; set; }
        [Required]
        public string CompanyRepresentativeName { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        [Required]
        public int VesselId { get; set; }
        [ForeignKey("VesselId")]
        public Vessel Vessel { get; set; }
        [Required]
        public int InspectionTypeId { get; set; }
        [ForeignKey("InspectionTypeId")]
        public InspectionTypes InspectionType { get; set; }
        [Required]
        public int InspectionSourceId { get; set; }
        [Required]
        public string InspectionSourceCode { get; set; }
        [Required]
        [StringLength(64)]
        public string InspectionCode { get; set; }
        [Required]
        public DateTime VettingDate { get; set; }
        [StringLength(50)]
        [DefaultValue(null)]
        public string InspectorName { get; set; } = null;
        [Required]
        public int PortId { get; set; }
        [Required]
        public string PortName { get; set; }
        [Required]
        public string PortCountry { get; set; }
        [DefaultValue(null)]
        public string Comments { get; set; }
        [DefaultValue(false)]
        public bool? Sent { get; set; } = false;
        [Required]
        public ICollection<BriefcaseQuestionnaire> Questionnaires { get; set; } = new Collection<BriefcaseQuestionnaire>();
    }
}

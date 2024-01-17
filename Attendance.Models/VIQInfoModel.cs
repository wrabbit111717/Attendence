using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Models
{
    [Table("VIQInfo")]
    public class VIQInfoModel
    {
        [Key]
        public int QId { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public int? NumOfQuestions { get; set; }
        public byte? Finalized { get; set; }
        public string Comments { get; set; }
        public Guid? VIQGUI { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string version { get; set; }
        public int? SecurityColumn { get; set; }
        public bool? Visible { get; set; }
        public List<UserQuestionnaire> UserQuestionnaires { get; set; } = new List<UserQuestionnaire>();
    }
}
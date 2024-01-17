using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Models
{
    public class BriefcaseQuestionnaire
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int BriefcaseId { get; set; }
        [ForeignKey("BriefcaseId")]
        public Briefcase Briefcase { get; set; }
        [Required]
        public int QId { get; set; }
        [ForeignKey("QId")]
        public VIQInfoModel Questionnaire { get; set; }
    }
}

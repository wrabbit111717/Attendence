using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Models
{
    // Legacy class UserQuestionnaire (table)
    public partial class UserQuestionnaire
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }

        [ForeignKey("QId")]
        public VIQInfoModel VIQInfo { get; set; }

        public int QId { get; set; }
    }
}

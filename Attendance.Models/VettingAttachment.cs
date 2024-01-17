using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Models
{
    public class VettingAttachment
    {
        [Key]
        public int Id { get; set; }
        public Vetting Vetting { get; set; }
        [ForeignKey("FK_VettingAttachment_Vetting"), Column(Order = 0)]
        public int VETId { get; set; }
        [ForeignKey("FK_VettingAttachment_Vetting"), Column(Order = 1)]
        public System.Guid ObjectId { get; set; }
        public byte[] commentFile { get; set; }
        public string commentFileName { get; set; }
    }
}

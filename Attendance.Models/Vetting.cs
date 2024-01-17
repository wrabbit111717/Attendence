using System.Collections.Generic;

namespace Attendance.Models
{
    public class Vetting
    {
        public int VETId { get; set; }
        public System.Guid ObjectId { get; set; }
        public string answer { get; set; }
        public int? significance { get; set; }
        public string comments { get; set; }
        public List<VettingAttachment> VettingAttachments { get; set; } = new List<VettingAttachment>();
        public byte? AssigneeRank { get; set; }
        public int? VerifiedinVetting { get; set; }
    }

    public class SMVetting
    {
        public int VETId { get; set; }

        public System.Guid ObjectId { get; set; }

        public string answer { get; set; }
        public string significance { get; set; }
        public string comments { get; set; }
    }
}
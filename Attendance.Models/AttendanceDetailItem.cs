using System;

namespace Attendance.Models
{
    public class AttendanceDetailItem
    {
        public string DisplayCode { get; set; }
        public string DefaultCode { get; set; }
        public string CategoryQuestion { get; set; }
        public string Answer { get; set; }
        public int Significance { get; set; }
        public string SignificanceName { get; set; }
        public string comments { get; set; }
        public string CategoryQuestionDescription { get; set; }
        public string CategoryQuestionComment { get; set; }
        public Guid? ObjectId { get; set; }
        public byte[] commentFile { get; set; }
        public string commentFileName { get; set; }
        public string title { get; set; }
        public int? qid { get; set; }
        public byte? AssigneeRank { get; set; }
        public int? VerifiedinVetting { get; set; }

    }
}

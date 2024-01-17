using System;

namespace Attendance.Models
{
    public class VIQDetailItem
    {
        public int Id { get; set; }
        public Guid ObjectId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int ObjectType { get; set; }
        public string Type { get; set; }
        public int? GlobalDisplayIndex { get; set; }
        public int? ShowAfterId { get; set; }

    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Attendance.Models
{
    public class VIQ
    {
        [Key]
        public int Id { get; set; }
        public int QId { get; set; }
        public int? ObjectType { get; set; }
        public int? CategoryId { get; set; }
        public System.Guid? QuestionId { get; set; }
        public System.Guid? CommentId { get; set; }
        public System.Guid? ParentId { get; set; }
        public int? ParentType { get; set; }
        public int? DisplayIndex { get; set; }
        public int? DisplayLevel { get; set; }
        public System.Guid? ObjectId { get; set; }
        public System.Guid? ParentCategory { get; set; }
        public int? GlobalDisplayIndex { get; set; }
        public int? Children { get; set; }
        public string DisplayCode { get; set; }
        public string InternalDisplayCode { get; set; }
        public int? ShowAfterId { get; set; }
    }
}
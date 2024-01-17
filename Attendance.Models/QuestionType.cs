using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Models
{
    public class QuestionType
    {
        [Key]
        public int TypeId { get; set; }

        public string TypeCode { get; set; }
        public string TypeDescription { get; set; }
    }
}

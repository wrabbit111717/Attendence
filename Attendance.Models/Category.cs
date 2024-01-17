using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        public string CategoryCode { get; set; }
        public string CategoryDescription { get; set; }

        public Guid CategoryNewID { get; set; }
        public int? Children { get; set; }

        [NotMapped]
        public bool IsAlreadyAddedCategory { get; set; }
    }
}

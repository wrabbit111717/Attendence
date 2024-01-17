using System.ComponentModel.DataAnnotations;

namespace Attendance.Models
{
    public class CrewRank
    {
        [Key]
        public int CrewRankID { get; set; }
        public int? Rank { get; set; }
        public string CrewRankDesc { get; set; }
    }
}

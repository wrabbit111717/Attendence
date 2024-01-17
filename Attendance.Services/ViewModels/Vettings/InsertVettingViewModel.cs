using System;
using System.ComponentModel.DataAnnotations;

namespace Attendance.Services.ViewModels.Vettings
{
    public class InsertVettingViewModel
    {
        [Required]
        public int? VetId { get; set; }

        [Required]
        public int? QId { get; set; }
    }
}
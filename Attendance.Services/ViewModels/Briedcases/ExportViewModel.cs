using Attendance.Models;
using System.ComponentModel.DataAnnotations;

namespace Attendance.Services.ViewModels.Briedcases
{
    /// <summary>
    /// Export briefcase view model
    /// </summary>
    // TODO: Remove
    public class ExportViewModel
    {
        public int? InspectionTypeId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string VettingType { get; set; }

        public string ExportPassword { get; set; }

        public VIQInfoModel[] VIQInfoModels { get; set; }
    }
}
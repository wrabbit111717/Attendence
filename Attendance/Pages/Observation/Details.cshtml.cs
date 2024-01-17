using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Linq;

using Microsoft.AspNetCore.Http;

using Attendance.Data;
using Attendance.Providers;
using Attendance.Services.Services;
using Attendance.Models;

namespace Attendance.Pages.Observation
{

    public class DetailsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int id { get; set; }
        [BindProperty]
        public Models.ObservationAssigneeInfo ObservationAssigneeInfo { get; set; }
        public List<ObservationSoc> ObservationSoc { get; set; }
        public List<ObservationNoc> ObservationNoc { get; set; }
        public List<ObservationsSire2Attachments> ObservationsSire2Attachments { get; set; }
        private readonly IAttendanceService _attendanceService;
        private readonly IVettingsService _vettingService;

        public BufferedSingleFileUploadDb FileUpload { get; set; }

        public PageAlertType AlertType = PageAlertType.Info;

        public DetailsModel(IAttendanceService attendanceService, IVettingsService vettingService)
        {
            _attendanceService = attendanceService;
            _vettingService = vettingService;
        }

        public class BufferedSingleFileUploadDb
        {
            [Display(Name = "File")]
            public IList<IFormFile> CommentFile { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            ObservationAssigneeInfo = await _attendanceService.GetObservationAssigneeAsync(id) ?? new ObservationAssigneeInfo();

            ObservationsSire2Attachments = await _vettingService.GetObservationAttchments(id) ?? new List<ObservationsSire2Attachments>();

            ObservationSoc = await _attendanceService.GetObservationSocListAsync(id);
            ObservationNoc = await _attendanceService.GetObservationNocListAsync(id);

            return Page();
        }

        public async Task<IActionResult> OnPostObservationDownloadAsync(int? obs_id, int? id)
        {
            var observationImg = await _vettingService.GetObservationAttchments(obs_id);
            if (observationImg == null)
            {
                return NotFound();
            }

            var selectedAttachment = observationImg.FirstOrDefault(x => x.id == id);
            
            return await Task.Run(() => File(selectedAttachment.attachment, "application/octet-stream", selectedAttachment.attachment_name));
        }
        public async Task<IActionResult> OnPostObservationDeleteAsync(int? obs_id, int? id)
        {
            await _vettingService.DeleteObservationAttchmentAsync(obs_id, id);
            OnLog(AppMessages.Observation_DeleteAttachmentSucceeded, PageAlertType.Success);
            return RedirectToPage("/Observation/Details", new
            {
                id = obs_id,
            });

        }
        public async Task<IActionResult> OnPostObservationDownloadAllAsync(int? obs_Id)
        {
            var observationAttachments = await _vettingService.GetObservationAttchments(obs_Id);
            var zipName = $"Attandence-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";
            using (MemoryStream ms = new MemoryStream())
            {
                //required: using System.IO.Compression;
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    //QUery the Products table and get all image content
                    observationAttachments.ToList().ForEach(file =>
                    {
                        var entry = zip.CreateEntry(file.attachment_name);
                        using (var fileStream = new MemoryStream(file.attachment))
                        using (var entryStream = entry.Open())
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    });
                }
                return File(ms.ToArray(), "application/zip", zipName);
            }
        }

        private void OnLog(string message, PageAlertType alertType)
        {
            AlertType = alertType;
            TempData["message"] = message;
            TempData["result"] = alertType.GetDescription();
        }
    }
}

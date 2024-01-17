using Attendance.Data;
using Attendance.Infrastructure.Data;
using Attendance.Providers;
using Attendance.Services.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Pages.VettingInfo
{
    public class AttachmentModel : PageModel
    {
        [BindProperty]
        public BufferedSingleFileUploadDb FileUpload { get; set; }
        public PageAlertType AlertType = PageAlertType.Info;
        private readonly IVettingsService _vettingService;
        private readonly AttendanceContext _context;

        public List<Attendance.Models.VettingAttachment> vettingAttachments { get; set; }
        public List<Attendance.Models.ObservationsSire2Attachments> observationsSire2Attachments { get; set; }
        public Attendance.Models.Vetting Vetting { get; set; }
        public Attendance.Models.VettingInfoDetail VettingInfoDetail { get; set; }


        public AttachmentModel(IVettingsService vettingService, AttendanceContext context)
        {
            _vettingService = vettingService;
            _context = context;
        }

        public class BufferedSingleFileUploadDb
        {
            [Display(Name = "File")]
            public IList<IFormFile> CommentFile { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id, Guid? objectId, string vesselName, string inspecName, string compRep, string insCode, bool isObservationAttchment, int? VetId)
        {
            if (!isObservationAttchment)
            {
                if (id == null || objectId == null)
                {
                    return NotFound();
                }
                Vetting = await _context.Vetting.Where(x => x.VETId == id && x.ObjectId == objectId).Include(x => x.VettingAttachments).FirstOrDefaultAsync();
                vettingAttachments = Vetting.VettingAttachments;
                VettingInfoDetail = new Models.VettingInfoDetail();
                VettingInfoDetail.VesselName = vesselName;
                VettingInfoDetail.InspectorName = inspecName;
                VettingInfoDetail.CompanyRepresentativeName = compRep;
                VettingInfoDetail.VettingCode = insCode;
                observationsSire2Attachments = new List<Models.ObservationsSire2Attachments>();
            }
            else
            {
                VettingInfoDetail = new Models.VettingInfoDetail();
                VettingInfoDetail.VesselName = vesselName;
                VettingInfoDetail.InspectorName = inspecName;
                VettingInfoDetail.CompanyRepresentativeName = compRep;
                VettingInfoDetail.VettingCode = insCode;
                VettingInfoDetail.VetId = Convert.ToInt32(VetId);
                observationsSire2Attachments = await _vettingService.GetObservationAttchments(id);
            }
            VettingInfoDetail.IsObservationAttchment = isObservationAttchment;
            return Page();
        }

        #region Comments Attachments
        public async Task<IActionResult> OnPostUploadAsync(int? vetId, Guid? objectId, string vesselName, string inspectorName, string compRep, string insCode)
        {
            if (FileUpload.CommentFile.Count > 0)
            {
                foreach (var itemFile in FileUpload.CommentFile)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await itemFile.CopyToAsync(memoryStream);

                        if (memoryStream.Length < 2097152)
                        {
                            await _vettingService.UpdateCommentFileAsync(memoryStream, itemFile.FileName, vetId, objectId);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "The file is too large.");
                            OnLog("The file is too large.", PageAlertType.Danger);
                        }
                    }
                }
                OnLog(AppMessages.Vetting_UploadCommentSucceeded1, PageAlertType.Success);
            }
            return RedirectToPage("/VettingInfo/Attachment", "", new
            {
                id = vetId,
                objectId = objectId,
                vesselname = vesselName,
                inspecName = inspectorName, 
                compRep = compRep,
                insCode = insCode
            });
        }
        public async Task<IActionResult> OnPostDownloadAsync(int? vetId, Guid? objectId, int? id)
        {

            var vetting = await _vettingService.GetWithVetAndObjectIdAsync(vetId, objectId);
            if (vetting == null || vetting.VettingAttachments == null || vetting.VettingAttachments.Count == 0)
            {
                return NotFound();
            }
            var selectedAttachment = vetting.VettingAttachments.FirstOrDefault(x => x.Id == id);
            return await Task.Run(() => File(selectedAttachment.commentFile, "application/octet-stream", selectedAttachment.commentFileName));

        }
        public async Task<IActionResult> OnPostDeleteAsync(int? vetId, Guid? objectId, string vesselName, string inspectorName, string compRep, string insCode, int? id)
        {
            await _vettingService.DeleteCommentFileAsync(vetId, objectId, id);
            OnLog(AppMessages.Vetting_DeleteCommentSucceeded, PageAlertType.Success);
            return RedirectToPage("/VettingInfo/Attachment", "", new
            {
                id = vetId,
                objectId = objectId,
                vesselname = vesselName,
                inspecName = inspectorName,
                compRep = compRep,
                insCode = insCode,
                isObservationAttchment = false
            });
        }
        public async Task<IActionResult> OnPostDownloadAllAsync(int? vetId, Guid? objectId)
        {
            var vetting = await _vettingService.GetWithVetAndObjectIdAsync(vetId, objectId);
            var zipName = $"Attandence-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";
            using (MemoryStream ms = new MemoryStream())
            {
                //required: using System.IO.Compression;
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    //QUery the Products table and get all image content
                    vetting.VettingAttachments.ToList().ForEach(file =>
                    {
                        var entry = zip.CreateEntry(file.commentFileName);
                        using (var fileStream = new MemoryStream(file.commentFile))
                        using (var entryStream = entry.Open())
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    });
                }
                return File(ms.ToArray(), "application/zip", zipName);
            }
        }
        #endregion

        #region Observation Attchments
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
        public async Task<IActionResult> OnPostObservationDeleteAsync(int? vetId, int? obs_id, Guid? objectId, string vesselName, string inspectorName, string compRep, string insCode, int? id)
        {
            await _vettingService.DeleteObservationAttchmentAsync(obs_id, id);
            OnLog(AppMessages.Observation_DeleteAttachmentSucceeded, PageAlertType.Success);
            return RedirectToPage("/VettingInfo/Attachment", "", new
            {
                id = obs_id,
                VetId = vetId,
                objectId = objectId,
                vesselname = vesselName,
                inspecName = inspectorName,
                compRep = compRep,
                insCode = insCode,
                isObservationAttchment = true
            });
        }
        public async Task<IActionResult> OnPostObservationDownloadAllAsync(int? obsId)
        {
            var observationAttachments = await _vettingService.GetObservationAttchments(obsId);
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
        #endregion
        private void OnLog(string message, PageAlertType alertType)
        {
            AlertType = alertType;
            TempData["message"] = message;
            TempData["result"] = alertType.GetDescription();
        }
    }
}

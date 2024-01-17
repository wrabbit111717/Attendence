using Attendance.Infrastructure.Data;
using Attendance.Models;
using Attendance.Services.ViewModels.Vettings;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Services.Services
{
    /// <summary>
    /// Create Vetting service for Main Database
    /// </summary>
    public interface IVettingsService
    {
        /// <summary>
        /// Get vetting
        /// </summary>
        /// <param name="vett"></param>
        /// <returns>byte[]</returns>
        Task<Vetting> GetWithVetAndObjectIdAsync(int? vetId, Guid? objectId);

        /// <summary>
        /// Insert new Vetting
        /// </summary>
        /// <param name="model">Insert model item</param>
        /// <returns>True/False</returns>
        Task<bool> CreateAsync(InsertVettingViewModel model);

        /// <summary>
        /// Update existing Vetting
        /// </summary>
        /// <param name="model">Update model item</param>
        /// <returns>True/False</returns>
        Task<bool> UpdateAsync(UpdateVettingViewModel model);
        Task<bool> UpdateCommentFileAsync(MemoryStream stream, string fileName, int? vetId, Guid? objectId);
        Task<bool> UpdateAsyncFromSDF(SMVetting vett, int? insertedVetId);
        Task<bool> DeleteCommentFileAsync(int? vetId, Guid? objectId, int? attachmentId);
        Task<List<ObservationsSire2Attachments>> GetObservationAttchments(int? obs_id);
        Task<bool> DeleteObservationAttchmentAsync(int? obsId, int? id);
    }

    public class VettingsService : IVettingsService
    {
        private readonly AttendanceContext _dbContext;

        public VettingsService(
            AttendanceContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CreateAsync(InsertVettingViewModel model)
        {
            if (!model.QId.HasValue || !model.VetId.HasValue)
            {
                throw new System.Exception("VetId or ObjectId can not be null");
            }

            string query = AppResource.SQL_Copy_Questionnaires;
            var parameters = new SqlParameter[2]
            {
                new SqlParameter("@vetId",model.VetId),
                new SqlParameter("@qId",model.QId)
            };
            var saved = await _dbContext.Database.ExecuteSqlRawAsync(query, parameters);
            return saved > 0;
        }

        public async Task<Vetting> GetWithVetAndObjectIdAsync(int? vetId, Guid? objectId)
        {
            return await _dbContext.Vetting.Where(x => x.VETId == vetId && x.ObjectId == objectId).Include(x => x.VettingAttachments).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(UpdateVettingViewModel model)
        {
            if (!model.QId.HasValue || !model.VetId.HasValue)
            {
                throw new System.Exception("VetId or ObjectId can not be null");
            }

            string query = AppResource.SQL_Update_Questionnaires_By_Vet;
            var parameters = new SqlParameter[4]
            {
                new SqlParameter("@answer",model.Answer),
                new SqlParameter("@sig",model.Significance),
                new SqlParameter("@cmt",model.Comments),
                new SqlParameter("@vetId",model.VetId),
            };
            var saved = await _dbContext.Database.ExecuteSqlRawAsync(query, parameters);
            return saved > 0;
        }

        public async Task<bool> UpdateAsyncFromSDF(SMVetting vett, int? insertedVetId)
        {
            string query = AppResource.SQL_Update_Questionnaires_By_ObjId;
            var parameters = new SqlParameter[5]
            {
                new SqlParameter("@answer",vett.answer == null ? "0" : vett.answer),
                new SqlParameter("@sig",vett.significance == null ? "0" : vett.significance),
                new SqlParameter("@cmt",vett.comments == null ? "" : vett.comments),
                //Below code line added because wee need  an unique identifier to update spesific vetId objects in database so we got this info from insertion of vet than just pass here and use it 
                //We need to prevent database override in all vet infos so we need a unique identifier which is new inserted vetId
                new SqlParameter("@vetId",insertedVetId),
                new SqlParameter("@objId",vett.ObjectId)
            };
            var saved = await _dbContext.Database.ExecuteSqlRawAsync(query, parameters);
            return saved > 0;
        }

        public async Task<bool> UpdateCommentFileAsync(MemoryStream stream, string fileName, int? vetId, Guid? objectId)
        {
            if (!vetId.HasValue || !objectId.HasValue || objectId.Value == Guid.Empty || vetId.Value == 0)
            {
                throw new System.Exception("VetId or ObjectId can not be null");
            }
            var vetting = await GetWithVetAndObjectIdAsync(vetId, objectId);

            VettingAttachment attachment = new VettingAttachment();
            attachment.commentFileName = fileName;
            attachment.commentFile = stream.ToArray();
            attachment.Vetting = vetting;
            attachment.ObjectId = vetting.ObjectId;
            attachment.VETId = vetting.VETId;
            vetting.VettingAttachments.Add(attachment);
            _dbContext.Vetting.Update(vetting);
            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<bool> DeleteCommentFileAsync(int? vetId, Guid? objectId, int? id)
        {
            if (!vetId.HasValue || !objectId.HasValue || objectId.Value == Guid.Empty || vetId.Value == 0 || !id.HasValue || id.Value == 0)
            {
                throw new System.Exception("VetId or ObjectId can not be null");
            }
            var vetting = await GetWithVetAndObjectIdAsync(vetId, objectId);
            var deletedAttachment = vetting.VettingAttachments.Where(x => x.Id == id).FirstOrDefault();
            vetting.VettingAttachments.Remove(deletedAttachment);
            _dbContext.Vetting.Update(vetting);
            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }
        public async Task<List<ObservationsSire2Attachments>> GetObservationAttchments(int? obs_id)
        {
            return await _dbContext.ObservationsSire2Attachments.Where(x => x.obs_id == obs_id).ToListAsync();
        }
        public async Task<bool> DeleteObservationAttchmentAsync(int? obsId,  int? id)
        {
            if (!obsId.HasValue || !id.HasValue || obsId.Value == 0 || id.Value == 0)
            {
                throw new Exception("id or obsId can not be null");
            }
            var observationAttchment = await GetObservationAttchments(obsId);
            var deletedAttachment = observationAttchment.Where(x => x.id == id).FirstOrDefault();
            _dbContext.ObservationsSire2Attachments.Remove(deletedAttachment);
            var saved = await _dbContext.SaveChangesAsync();
            return saved > 0;
        }
    }
}
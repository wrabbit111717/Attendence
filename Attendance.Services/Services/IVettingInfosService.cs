
using Attendance.Infrastructure.Data;
using Attendance.Models;
using Attendance.Services.ViewModels.VettingInfos;
using Attendance.Services.ViewModels.Vettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Services.Services
{
    /// <summary>
    /// Create Vetting service for Main Database
    /// </summary>
    public interface IVettingInfosService
    {
        /// <summary>
        /// Import VettingInfo from Compact Db to Main Db
        /// </summary>
        /// <param name="model">Import model item</param>
        /// <returns>True/False</returns>
        Task<bool> CreateAsync(ImportVettingInfoViewModel model, string dataSource, string sdfPassword);
    }

    public class VettingInfosService : IVettingInfosService
    {
        private readonly ICompactVettingsService _compactVettingService;
        private readonly IVettingsService _vettingsService;
        private readonly IConfiguration _configuration;
        private readonly AttendanceContext _dbContext;

        public VettingInfosService(
            AttendanceContext dbContext
            , IVettingsService vettingsService
            , ICompactVettingsService compactVettingsService
            , IConfiguration configuration
            )
        {
            _vettingsService = vettingsService;
            _compactVettingService = compactVettingsService;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<bool> CreateAsync(ImportVettingInfoViewModel model, string dataSource, string sdfPassword)
        {
            bool questionExist = await _dbContext.VIQInfo.AnyAsync(i => i.VIQGUI == model.VIQGUI);
            if (!questionExist)
                throw new Exception("Questionnaire is not registered import aborted");

            bool vesselExist = await _dbContext.Vessel.AnyAsync(i => i.VesselName == model.VesselName);
            if (!vesselExist)
                throw new Exception("Vessel name is not registered import aborted");

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // insert new VettingInfo
                    await AddToDbAsync(model);

                    // Copy Questionnaire context

                    var insertVettingModel = new InsertVettingViewModel()
                    {
                        QId = model.QId,
                        VetId = await GetVetIdAsync(model.VetGUI.Value)
                    };
                    // Below line code added to be sure about vet created successfully , if not we need to throw exception because vetId is crucial in our business process
                    if (!await _vettingsService.CreateAsync(insertVettingModel))
                    {
                        throw new Exception("Vetting couldn't be created successfully");
                    }

                    // Update Questionnaire
                    //var updateVettingModel = new UpdateVettingViewModel()
                    //{
                    //    VetId        = insertVettingModel.VetId,
                    //    QId          = insertVettingModel.QId,
                    //    Answer       = "0",
                    //    Significance = 0,
                    //    Comments     = ""
                    //};
                    //await _vettingsService.UpdateAsync(updateVettingModel);

                    //Update SMVetting
                    List<SMVetting> smvettings = await _compactVettingService.GetSMVetting(model.VetId, dataSource, sdfPassword);
                    foreach (SMVetting smvetting in smvettings)
                    {
                        // insertVettingModel.VetId sended to UpdateAsyncFromSDF method 
                        await _vettingsService.UpdateAsyncFromSDF(smvetting, insertVettingModel.VetId);
                    }

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw ex;
                }
            }
        }

        #region Helpers

        private Task<int> GetVetIdAsync(Guid vetGUI)
        {
            return _dbContext
                .VettingInfo
                .Where(i => i.VetGUI == vetGUI)
                .Select(i => i.VetId)
                .FirstAsync();
        }

        private async Task<bool> AddToDbAsync(ImportVettingInfoViewModel model)
        {
            if (await _dbContext.VettingInfo.AnyAsync(i => i.VetGUI == model.VetGUI))
                throw new Exception("This VetGUI already exist in VettingInfo table.");
            var createVetInfo = new VettingInfo()
            {
                QId = model.QId.Value,
                VetGUI = model.VetGUI.Value,
                InspectorName = model.InspectorName,
                CompanyRepresentativeName = model.CompanyRepresentativeName,
                VesselId = model.VesselId,
                VesselName = model.VesselName,
                Port = model.Port,
                VetDate = model.VetDate,
                VettingCode = model.VettingCode,
                InspectionTypeId = model.InspectionTypeId,
                Comments = model.Comments,
                RegistrationDate = DateTime.Now
            };

            _dbContext.VettingInfo.Add(createVetInfo);

            return (await _dbContext.SaveChangesAsync()) > 0;
        }

        #endregion Helpers
    }
}
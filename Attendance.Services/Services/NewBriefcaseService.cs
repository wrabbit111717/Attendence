using Attendance.Infrastructure.Data;
using Attendance.Models;
using Attendance.Services.DTOs;
using Attendance.Services.ViewModels.Briedcases;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Operations.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Attendance.Services.Services
{
    // TODO: Remove 'New' prefix
    public class NewBriefcaseService : INewBriefcaseService
    {
        private readonly AttendanceContext _context;
        private readonly OperationsDbContext _operationsContext;
        private readonly IMapper _mapper;
        public NewBriefcaseService(AttendanceContext context, OperationsDbContext operationsContext, IMapper mapper)
        {
            _context = context;
            _operationsContext = operationsContext;
            _mapper = mapper;
        }
        private async Task<List<VesselDTO>> GetVesselsAsync()
        {
            var items = await _mapper.ProjectTo<VesselDTO>(
                _context.Vessel
            ).ToListAsync();
            return items;
        }
        private async Task<List<InspectionTypeDTO>> GetInspectionTypesAsync()
        {
            var items = await _mapper.ProjectTo<InspectionTypeDTO>(
                _context.InspectionTypes
            ).ToListAsync();
            return items;
        }
        private async Task<List<InspectionSourceDTO>> GetInspectionSourcesAsync()
        {
            var items = await _mapper.ProjectTo<InspectionSourceDTO>(
                _context.InspectionSource
            ).ToListAsync();
            return items;
        }
        private async Task<List<VIQInfoDTO>> GetQuestionnaires()
        {
            var items = await _mapper.ProjectTo<VIQInfoDTO>(
                _context.VIQInfo
                .OrderByDescending(_ => _.QId)
            ).ToListAsync();
            return items;
        }
        private async Task<UserDTO> GetUserDetails(string userId)
        {
            var user = await _mapper.ProjectTo<UserDTO>(
                _context.Users
            ).FirstOrDefaultAsync(_ => _.Id == userId);
            return user;
        }
        public async Task<List<BriefcaseDTO>> GetBriefcases(string userId)
        {
            var briefcases = await _mapper.ProjectTo<BriefcaseDTO>(
                _context.Briefcase.Where(_ => _.UserId == userId)
                .OrderByDescending(_ => _.VettingDate)
            ).ToListAsync();
            return briefcases;
        }
        public async Task<BriefcaseViewModel> GetBriefcase(string userId, int? briefcaseId = null)
        {
            BriefcaseDTO data = null;
            InspectionSourceDTO inspectionSource = null;
            if (briefcaseId != null)
            {
                data = await _mapper.ProjectTo<BriefcaseDTO>(
                    _context.Briefcase
                ).FirstOrDefaultAsync(_ => _.Id == briefcaseId);
                if (data == null)
                {
                    throw new KeyNotFoundException();
                }
                if (data.UserId != userId)
                {
                    throw new UnauthorizedAccessException();
                }
            }
            var model = new BriefcaseViewModel()
            {
                Vessels = await GetVesselsAsync(),
                InspectionTypes = await GetInspectionTypesAsync(),
                InspectionSources = await GetInspectionSourcesAsync(),
                Questionnaires = await GetQuestionnaires(),
                Data = data
            };
            return model;
        }
        public async Task<List<PortDTO>> GetPortList(string query)
        {
            var ports = await _mapper.ProjectTo<PortDTO>(
                _operationsContext.Port
                .Where(_ => _.Country.Region.Name.StartsWith(query) || _.Country.Region.Area.Name.StartsWith(query) || _.Country.Region.Area.Code.StartsWith(query) || _.Country.Name.StartsWith(query) || _.Country.Alpha2.StartsWith(query) || _.Name.StartsWith(query) || _.Code.StartsWith(query))
                .OrderBy(_ => _.Name).ThenBy(_ => _.Code).Take(20)
            ).ToListAsync();
            return ports;
        }
        public async Task<PortDTO> GetPort(int portId)
        {
            var port = await _mapper.ProjectTo<PortDTO>(
                _operationsContext.Port
                .Where(_ => _.Id == portId)
            ).FirstOrDefaultAsync();
            return port;
        }
        public async Task<int> CreateBriefcase(string userId, BriefcasePayloadDTO payload)
        {
            var vesselName = await _context.Vessel.Where(_ => _.VesselId == payload.VesselId).Select(_ => _.VesselName).FirstAsync();
            var inspectionTypeCode = await _context.InspectionTypes.Where(_ => _.InspectionTypeId == payload.InspectionTypeId).Select(_ => _.InspectionCode).FirstAsync();
            var inspectionSourceCode = await _context.InspectionSource.Where(_ => _.InspectionSourceId == payload.InspectionSourceId).Select(_ => _.SourceCode).FirstAsync();
            var inspectionCode = $"{vesselName.Substring(0, 3).ToUpper()}-{payload.VettingDate.ToString("ddMM")}-{inspectionTypeCode.ToUpper()}-{Regex.Replace(inspectionSourceCode.ToUpper(), @"\s+", "")}";
            var port = await GetPort(payload.PortId);
            var user = await GetUserDetails(userId);
            var briefcase = new Briefcase()
            {
                UserId = user.Id,
                CompanyRepresentativeName = user.FullName,
                VesselId = payload.VesselId,
                InspectionTypeId = payload.InspectionTypeId,
                InspectionSourceId = payload.InspectionSourceId,
                InspectionSourceCode = inspectionSourceCode,
                PortId = port.Id,
                PortName = port.Name,
                PortCountry = port.Country,
                VettingDate = payload.VettingDate,
                InspectorName = string.IsNullOrEmpty(payload.InspectorName) ? null : payload.InspectorName,
                Comments = payload.Comments,
                InspectionCode = inspectionCode
            };
            _context.Briefcase.Add(briefcase);
            await _context.SaveChangesAsync();
            var briefcaseQuestionnaires = new List<BriefcaseQuestionnaire>();
            foreach (var qId in payload.Questionnaires)
            {
                briefcaseQuestionnaires.Add(new BriefcaseQuestionnaire()
                {
                    BriefcaseId = briefcase.Id,
                    QId = qId
                });
            }
            await _context.BriefcaseQuestionnaires.AddRangeAsync(briefcaseQuestionnaires);
            await _context.SaveChangesAsync();
            return briefcase.Id;
        }
        public async Task UpdateBriefcase(string userId, int briefcaseId, BriefcasePayloadDTO payload)
        {
            var vesselName = await _context.Vessel.Where(_ => _.VesselId == payload.VesselId).Select(_ => _.VesselName).FirstAsync();
            var inspectionTypeCode = await _context.InspectionTypes.Where(_ => _.InspectionTypeId == payload.InspectionTypeId).Select(_ => _.InspectionCode).FirstAsync();
            var inspectionSourceCode = await _context.InspectionSource.Where(_ => _.InspectionSourceId == payload.InspectionSourceId).Select(_ => _.SourceCode).FirstAsync();
            var inspectionCode = $"{vesselName.Substring(0, 3).ToUpper()}-{payload.VettingDate.ToString("ddMM")}-{inspectionTypeCode.ToUpper()}-{Regex.Replace(inspectionSourceCode.ToUpper(), @"\s+", "")}";
            var port = await GetPort(payload.PortId);
            var user = await GetUserDetails(userId);

            var briefcase = await _context.Briefcase.FirstAsync(_ => _.Id == briefcaseId);
            briefcase.UserId = user.Id;
            briefcase.CompanyRepresentativeName = user.FullName;
            briefcase.VesselId = payload.VesselId;
            briefcase.InspectionTypeId = payload.InspectionTypeId;
            briefcase.InspectionSourceId = payload.InspectionSourceId;
            briefcase.PortId = port.Id;
            briefcase.PortName = port.Name;
            briefcase.PortCountry = port.Country;
            briefcase.VettingDate = payload.VettingDate;
            briefcase.InspectorName = payload.InspectorName;
            briefcase.Comments = payload.Comments;
            briefcase.InspectionCode = inspectionCode;
            await _context.SaveChangesAsync();

            var briefcaseQuestionnaires = await _context.BriefcaseQuestionnaires.Where(_ => _.BriefcaseId == briefcaseId).ToListAsync();

            var itemsToRemove = briefcaseQuestionnaires.Where(_ => !payload.Questionnaires.Contains(_.QId)).ToList();

            if (itemsToRemove.Count > 0)
            {
                _context.BriefcaseQuestionnaires.RemoveRange(itemsToRemove);
                await _context.SaveChangesAsync();
            }

            var itemsToAdd = payload.Questionnaires.Where(_ => !briefcaseQuestionnaires.Select(_ => _.QId).Contains(_)).ToList();

            foreach (var qId in itemsToAdd)
            {
                await _context.BriefcaseQuestionnaires.AddAsync(new BriefcaseQuestionnaire()
                {
                    BriefcaseId = briefcase.Id,
                    QId = qId
                });
            }

            await _context.SaveChangesAsync();

        }

    }
}

using Attendance.Services.DTOs;
using Attendance.Services.ViewModels.Briedcases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance.Services.Services
{
    public interface INewBriefcaseService
    {
        public Task<BriefcaseViewModel> GetBriefcase(string userId, int? briefcaseId = null);
        public Task<List<PortDTO>> GetPortList(string query);
        public Task<PortDTO> GetPort(int portId);
        public Task<int> CreateBriefcase(string userId, BriefcasePayloadDTO payload);
        public Task UpdateBriefcase(string userId, int briefcaseId, BriefcasePayloadDTO payload);
        public Task<List<BriefcaseDTO>> GetBriefcases(string userId);

    }
}

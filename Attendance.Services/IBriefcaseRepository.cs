using Attendance.Models;
using System.Collections.Generic;

namespace Attendance.Services
{
    public interface IBriefcaseRepository
    {
        IEnumerable<VIQInfoModel> GetRegisteredQuestionnaires();

        bool AddQuestionnaire(VIQInfoModel vIQInfoModel);

        void RemoveQuestionnaire(int qID);
    }
}
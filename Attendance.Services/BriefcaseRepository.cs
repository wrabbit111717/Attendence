using Attendance.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Attendance.Services
{
    public class BriefcaseRepository : IBriefcaseRepository
    {
        private List<VIQInfoModel> _registeredQuestionnaires;

        public BriefcaseRepository()
        {
            _registeredQuestionnaires = new List<VIQInfoModel>();
        }

        public IEnumerable<VIQInfoModel> GetRegisteredQuestionnaires()
        {
            return _registeredQuestionnaires;
        }

        public bool AddQuestionnaire(VIQInfoModel vIQInfoModel)
        {
            VIQInfoModel info = _registeredQuestionnaires.Find(e => e.QId == vIQInfoModel.QId);
            if (info == null)
            {
                _registeredQuestionnaires.Add(vIQInfoModel);
                return true;
            }

            return false;
        }

        public void RemoveQuestionnaire(int qID)
        {
            VIQInfoModel info = _registeredQuestionnaires.Find(e => e.QId == qID);
            _registeredQuestionnaires.Remove(info);
        }
    }
}

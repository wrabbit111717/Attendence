using Attendance.Models;
using System.Collections.Generic;
using System.Linq;

namespace Attendance.Services
{
    /// <summary>
    /// Briefcase Import repository
    /// </summary>
    public interface IBriefcaseImportRepository
    {
        /// <summary>
        /// Compact data source
        /// </summary>
        string DataSource { get; set; }

        /// <summary>
        /// Get all import items
        /// </summary>
        /// <returns></returns>
        IEnumerable<VIQInfoModel> SdfDBItems();

        /// <summary>
        /// Clear all import items
        /// </summary>
        void ClearSdfDBItems();

        /// <summary>
        /// Add import item to repository
        /// </summary>
        /// <param name="vIQInfoModel"></param>
        /// <returns></returns>
        bool AddSdfDBItems(params VIQInfoModel[] vIQInfoModel);

        /// <summary>
        /// Remove import item in repository
        /// </summary>
        /// <param name="qID"></param>
        void RemoveSdfDBItem(int qID);

        /// <summary>
        /// Get select Attendances
        /// </summary>
        /// <returns>Attendances list</returns>
        IEnumerable<VettingInfo> GetAttendanceItems();

        /// <summary>
        /// Set Attendances
        /// </summary>
        /// <param name="vettingInfos">Attendances</param>
        void SetAttendanceItems(IEnumerable<VettingInfo> vettingInfos);
    }

    public class BriefcaseImportRepository : IBriefcaseImportRepository
    {
        private readonly List<VIQInfoModel> _sdfDBItems;
        private readonly List<VettingInfo>  _attendanceItems;

        public string DataSource { get; set; }

        public BriefcaseImportRepository()
        {
            _sdfDBItems      = new List<VIQInfoModel>();
            _attendanceItems = new List<VettingInfo>();
        }

        public bool AddSdfDBItems(params VIQInfoModel[] vIQInfoModel)
        {
            var addItems = vIQInfoModel.Where(m => !_sdfDBItems.Any(i => i.QId == m.QId));
            if (!addItems.Any())
                return false;

            _sdfDBItems.AddRange(addItems);
            return true;
        }

        public void ClearSdfDBItems()
        {
            _sdfDBItems.Clear();
        }

        public void RemoveSdfDBItem(int qID)
        {
            _sdfDBItems.RemoveAll(i => i.QId == qID);
        }

        public IEnumerable<VIQInfoModel> SdfDBItems()
        {
            return _sdfDBItems;
        }

        public IEnumerable<VettingInfo> GetAttendanceItems()
        {
            return _attendanceItems;
        }

        public void SetAttendanceItems(IEnumerable<VettingInfo> vettingInfos)
        {
            _attendanceItems.Clear();
            _attendanceItems.AddRange(vettingInfos);
        }
    }
}
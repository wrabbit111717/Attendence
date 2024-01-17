using Attendance.Models;
using Attendance.Services.Extensions;
using Attendance.Services.Providers;
using Attendance.Services.ViewModels.VettingInfos;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Services.Services
{
    /// <summary>
    /// Create Vetting service for Compact Database
    /// </summary>
    public interface ICompactVettingsService
    {
        /// <summary>
        /// Get Vettings by a Questionnaire in Compact Db
        /// </summary>
        /// <param name="qId">questionnaire ID</param>
        /// <param name="dataSource">Compact data source</param>
        /// <param name="sdfPassword">Password for Compact Database</param>
        /// <returns>Vetting list</returns>
        Task<IEnumerable<VettingInfo>> VettingsByQuestionAsync(int qId, string dataSource, string sdfPassword);
        Task<List<SMVetting>> GetSMVetting(int? VetID, string dataSource, string sdfPassword);

        /// <summary>
        /// Get VettingInfo by Id
        /// </summary>
        /// <param name="vId">Vetting Id</param>
        Task<ImportVettingInfoViewModel> VettingInfoByIdAsync(int vId, string dataSource, string sdfPassword);
    }

    public class CompactVettingsService : ICompactVettingsService
    {
        private readonly ISqlConnectionProvider _sqlConnect;
        private readonly ISqlSettingBuilder _connectionBuilder;

        public CompactVettingsService(
            ISqlConnectionProvider sqlConnect
            , ISqlSettingBuilder connectionBuilder
            )
        {
            _sqlConnect = sqlConnect;
            _connectionBuilder = connectionBuilder;
        }

        public async Task<ImportVettingInfoViewModel> VettingInfoByIdAsync(int vId, string dataSource, string sdfPassword)
        {
            _sqlConnect.Open(_connectionBuilder.BuildCompactConnectionString(dataSource, sdfPassword));

            string query = AppResource.SQL_GetVettingInfoByVetId;
            var vidParam = new SqlCeParameter("@p0", vId);
            DataTable table = await _sqlConnect.QueryAsync(query, vidParam);

            return table.ToObjects<ImportVettingInfoViewModel>().FirstOrDefault();
        }

        public async Task<List<SMVetting>> GetSMVetting(int? VetID, string dataSource, string sdfPassword)
        {
            if (_sqlConnect.IsOpen == false)
            {
                _sqlConnect.Open(_connectionBuilder.BuildCompactConnectionString(dataSource, sdfPassword));
            }

            string query = "SELECT * FROM SMVetting WHERE VETId=@p0";
            var vidParam = new SqlCeParameter("@p0", VetID);
            DataTable table = await _sqlConnect.QueryAsync(query, vidParam);

            return table.ToObjects<SMVetting>();
        }

        public async Task<IEnumerable<VettingInfo>> VettingsByQuestionAsync(int qId, string dataSource, string sdfPassword)
        {
            var filePath = Path.Combine(Path.GetTempPath(), dataSource);
            var connectionString = _connectionBuilder.BuildCompactConnectionString(filePath, sdfPassword);
            _sqlConnect.Open(connectionString);
            string query = AppResource.SQL_GetVettingInfosByQid;
            var qidParam = new SqlCeParameter("@qid", qId);
            var table = await _sqlConnect.QueryAsync(query, qidParam);
            var info = table.ToObjects<VettingInfo>();
            return info;
        }
    }
}
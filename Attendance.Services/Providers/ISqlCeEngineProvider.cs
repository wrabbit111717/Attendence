using System;
using System.Data.SqlServerCe;
using System.IO;

namespace Attendance.Services.Providers
{
    /// <summary>
    /// Sql Compact Engine
    /// </summary>
    public interface ISqlCeEngineProvider : IDisposable
    {
        void CreateDatabase(string datasource, string password = null);

        bool ValidateDataSource(string dataSource);

        string CurrentConnectString { get; }
    }

    public class SqlCeEngineProvider : ISqlCeEngineProvider
    {
        private bool _isDisposed;
        private readonly ISqlSettingBuilder _connectionBuilder;
        private readonly SqlCeEngine _engine;
        public string CurrentConnectString { get; private set; }

        public SqlCeEngineProvider(ISqlSettingBuilder connectionBuilder)
        {
            _connectionBuilder = connectionBuilder;
            _engine = new SqlCeEngine();
        }

        public void CreateDatabase(string datasource, string password = null)
        {
            CurrentConnectString = _connectionBuilder.BuildCompactConnectionString(datasource, password);
            _engine.LocalConnectionString = CurrentConnectString;
            _engine.CreateDatabase();
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = !_isDisposed;
                _engine.Dispose();
            }
        }

        public bool ValidateDataSource(string dataSource)
        {
            FileInfo fi = new FileInfo(dataSource);
            try
            {
                if (fi.Exists)
                {
                    fi.Delete();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
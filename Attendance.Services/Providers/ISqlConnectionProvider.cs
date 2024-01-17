using System;
using System.Data;
using System.Data.SqlServerCe;
using System.Threading.Tasks;

namespace Attendance.Services.Providers
{
    /// <summary>
    /// Create a common sql connection
    /// </summary>
    public interface ISqlConnectionProvider : IDisposable
    {
        /// <summary>
        /// Check Sql Connection state
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Open Sql Connection
        /// </summary>
        /// <param name="connectString"></param>
        void Open(string connectString);

        /// <summary>
        /// Close Sql Connection
        /// </summary>
        void Close();

        /// <summary>
        /// Send Sql Query to Database
        /// </summary>
        /// <param name="query">Query string</param>
        /// <param name="paramaters">Query parameters</param>
        /// <returns>Table</returns>
        DataTable Query(string query, params object[] paramaters);

        /// <summary>
        /// Send Sql Query to Database in asynchronous
        /// </summary>
        /// <param name="query">Query string</param>
        /// <param name="paramaters">Query parameters</param>
        /// <returns></returns>
        Task<DataTable> QueryAsync(string query, params object[] paramaters);

        /// <summary>
        /// Send Sql Command to Database
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="paramaters">Command parameters</param>
        /// <returns></returns>
        int Command(string command, params object[] paramaters);

        /// <summary>
        /// Send Sql Command to Database in asynchronous
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="paramaters">Command parameters</param>
        /// <returns></returns>
        Task<int> CommandAsync(string query, params object[] paramaters);
    }

    public class SqlCompactConnectionProvider : ISqlConnectionProvider
    {
        private SqlCeConnection _connection;
        private bool _isDisposed;
        public bool IsOpen { get; private set; }

        public SqlCompactConnectionProvider()
        {
            _connection = new SqlCeConnection();
        }

        ~SqlCompactConnectionProvider()
        {
            Dispose();
        }

        public void Close()
        {
            _connection.Close();
            IsOpen = false;
        }

        public void Dispose()
        {
            if (IsOpen && !_isDisposed)
            {
                Close();
                _connection.Dispose();
                _isDisposed = !_isDisposed;
            }
        }

        public void Open(string connectString)
        {
            _connection.ConnectionString = connectString;
            // prevent db file created with earlier version
            try
            {
                var engine = new SqlCeEngine(_connection.ConnectionString);
                engine.Upgrade();
            }
            catch { }

            // Open
            _connection.Open();
            IsOpen = true;
        }

        public DataTable Query(string query, params object[] paramaters)
        {
            var command = new SqlCeCommand(query, _connection);
            if (paramaters.Length > 0)
                command.Parameters.AddRange(paramaters);

            var reader = command.ExecuteReader();

            var table = new DataTable();
            table.Load(reader);
            return table;
        }

        public async Task<DataTable> QueryAsync(string query, params object[] paramaters)
        {
            var command = new SqlCeCommand(query, _connection);
            if (paramaters.Length > 0)
                command.Parameters.AddRange(paramaters);

            var reader = await command.ExecuteReaderAsync();

            var table = new DataTable();
            table.Load(reader);
            return table;
        }

        public int Command(string query, params object[] paramaters)
        {
            using (var command = new SqlCeCommand(query, _connection))
            {
                if (paramaters.Length > 0)
                    command.Parameters.AddRange(paramaters);

                return command.ExecuteNonQuery();
            }
        }

        public Task<int> CommandAsync(string query, params object[] paramaters)
        {
            using (var command = new SqlCeCommand(query, _connection))
            {
                if (paramaters.Length > 0)
                    command.Parameters.AddRange(paramaters);

                return command.ExecuteNonQueryAsync();
            }
        }
    }
}
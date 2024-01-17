namespace Attendance.Services.Providers
{
    /// <summary>
    /// Create a Setting Builder for SqlConnection
    /// </summary>
    public interface ISqlSettingBuilder
    {
        /// <summary>
        /// Build Sql connection string for Compact Sql Server
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        string BuildCompactConnectionString(string dataSource, string password);
    }

    public class SqlConnectionBuilder : ISqlSettingBuilder
    {
        public string BuildCompactConnectionString(string dataSource, string password)
        {
            return $"DataSource=\"{dataSource}\";Password='{password}'";
        }
    }
}
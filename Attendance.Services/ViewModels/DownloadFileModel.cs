namespace Attendance.Services.ViewModels
{
    /// <summary>
    /// Common download file model
    /// </summary>
    public class DownloadFileModel
    {
        /// <summary>
        /// Name of download file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Content of download file
        /// </summary>
        public byte[] FileContent { get; set; }
    }
}
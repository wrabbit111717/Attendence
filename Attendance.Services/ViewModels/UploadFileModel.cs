namespace Attendance.Services.ViewModels
{
    /// <summary>
    /// Common Upload file model
    /// </summary>
    public class UploadFileModel
    {
        /// <summary>
        /// Name of upload file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Type of upload file
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Content of upload file
        /// </summary>
        public byte[] FileContent { get; set; }
    }
}
namespace Attendance.Services.ViewModels.Vettings
{
    public class UpdateVettingViewModel : InsertVettingViewModel
    {
        public string Answer { get; set; }
        public int Significance { get; set; }
        public string Comments { get; set; }
        public byte[] CommentFile { get; set; }
        public string CommentFileName { get; set; }

    }
}
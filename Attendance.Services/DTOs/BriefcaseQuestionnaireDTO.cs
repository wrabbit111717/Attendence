namespace Attendance.Services.DTOs
{
    public class BriefcaseQuestionnaireDTO
    {
        public int Id { get; set; }
        public BriefcaseDTO Briefcase { get; set; }
        public VIQInfoDTO Questionnaire { get; set; }
    }
}

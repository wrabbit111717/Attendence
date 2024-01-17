using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Attendance.Models
{
    public class FormDetail
    {
        public int vetid { get; set; }
        public int serviceid { get; set; }
        public int? characterscore { get; set; }
        public int? workingexperience { get; set; }
        public int? motivation { get; set; }
        public int? teamwork { get; set; }
        public int? smscompliance { get; set; }
        public int? communication { get; set; }
        public int? decisionmaking { get; set; }
        public int? managerialskill { get; set; }
        public int? potentialcareerdevelopment { get; set; }
        public int? wasnavigationasessment { get; set; }
        public int? timeinrank { get; set; }
        public int? timeonboard { get; set; }
        public int? timewithcompany { get; set; }
        public int? agree { get; set; }
        public string crewname { get; set; }
        public string crewrank { get; set; }
        public string crewcomment { get; set; }
        public string personalcomment { get; set; }
        public string shipfacilities { get; set; }
        public string assessmentsummary { get; set; }
        public string nonconformance { get; set; }
    }

    public class CrewEvaluationInfo
    {
        public int vetid { get; set; }
        public int? characterscore { get; set; }
        public int? workingexperience { get; set; }
        public int? motivation { get; set; }
        public int? teamwork { get; set; }
        public int? smscompliance { get; set; }
        public int? communication { get; set; }
        public int? decisionmaking { get; set; }
        public int? managerialskill { get; set; }
        public int? potentialcareerdevelopment { get; set; }
        public string crewname { get; set; }
        public string crewrank { get; set; }
        public string crewcomment { get; set; }
        public int? rank { get; set; }
    }

}

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel;

namespace Attendance.Models
{

    // NOTE => Below scripts need to run one by one 

    //ALTER TABLE[AttendanceV1prod].[dbo].[QuestionPoolNew]
    //ADD CONSTRAINT PK_QuestionId PRIMARY KEY(questionid)

    //ALTER TABLE[AttendanceV1prod].[dbo].[QuestionPoolNew] ADD ParentId uniqueidentifier NULL;

    //ALTER TABLE[AttendanceV1prod].[dbo].[QuestionPoolNew]
    //ADD CONSTRAINT[FK_QuestionPool_Parent] FOREIGN KEY([ParentId])
    //REFERENCES[AttendanceV1prod].[dbo].[QuestionPoolNew] ([questionid])

    public class QuestionPool
    {
        [Key]
        public Guid questionid { get; set; }

        public string questioncode { get; set; }

        public string question { get; set; }

        public string comment { get; set; }

        public int? CategoryID { get; set; }

        public string CategoryCode { get; set; }

        public Guid? CategoryNewID { get; set; }

        public int? Origin { get; set; }

        public int QuestionTypeID { get; set; }

        public QuestionPool Parent { get; set; }
        public Guid? ParentId { get; set; }
        public IList<QuestionPool> Children { get; set; }

        [NotMapped]
        public bool IsAlreadyAddedQuestion { get; set; }

    }
}

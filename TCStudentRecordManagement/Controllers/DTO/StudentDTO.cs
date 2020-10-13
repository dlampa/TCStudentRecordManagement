using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCStudentRecordManagement.Models;

namespace TCStudentRecordManagement.Controllers.DTO
{
    public class StudentDetailDTO
    {
        public int StudentID { get; set; }
        public UserDTO User { get; set; }
        public CohortDTO Cohort { get; set; }
        public string BearTracksID { get; set; }

        public StudentDetailDTO()
        {

        }

        public StudentDetailDTO(Student studentInput)
        {
            StudentID = studentInput.StudentID;
            User = new UserDTO(studentInput.UserData);
            Cohort = new CohortDTO(studentInput.CohortMember);
            BearTracksID = studentInput.BearTracksID;
        }
    }

    public class StudentDTO
    {
        public int StudentID { get; set; }
        public int UserID { get; set; }
        public int CohortID { get; set; }
        
        public string BearTracksID { get; set; }

        public StudentDTO()
        {

        }

        public StudentDTO(Student studentInput)
        {
            StudentID = studentInput.StudentID;
            UserID = studentInput.UserID;
            CohortID = studentInput.CohortID;
            BearTracksID = studentInput.BearTracksID;
        }
    }


}

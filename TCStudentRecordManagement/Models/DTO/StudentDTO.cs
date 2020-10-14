using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCStudentRecordManagement.Models;

namespace TCStudentRecordManagement.Models.DTO
{
    /// <summary>
    /// StudentDTO class strips all the virtual properties from the Student object.
    /// </summary>
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

    /// <summary>
    /// StudentDetailDTO class adds some of the virtual properties of the Student model, such as Cohort membership and User data (both as DTO)
    /// </summary>
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


    /// <summary>
    /// StudentModDTO is used to capture immutable properties needed to modify Student records in the database. It combines both Student and User object properties.
    /// </summary>
    public class StudentModDTO
    {
        public int StudentID { get; set; }
        public int CohortID { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public string Email { get; set; }
        public bool Active { get; set; }

        public string BearTracksID { get; set; }

        public StudentModDTO()
        {

        }
    }

}

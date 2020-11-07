using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCStudentRecordManagement.Models.DTO
{
    public class StaffDTO
    {
        public int UserID { get; set; }
        public int StaffID { get; set; }
        public bool SuperUser { get; set; }
       

        public StaffDTO()
        {

        }

        public StaffDTO(Staff inputStaff)
        {
            if (inputStaff != null)
            {
                UserID = inputStaff.UserID;
                StaffID = inputStaff.StaffID;
                SuperUser = inputStaff.SuperUser;

            }
        }

    }
}

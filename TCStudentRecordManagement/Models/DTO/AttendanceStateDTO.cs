using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCStudentRecordManagement.Models.DTO
{
    /// <summary>
    /// AttendanceStateDTO class is used to represent the mutable properties of the AttendanceState objects/records
    /// </summary>
    public class AttendanceStateDTO
    {
        public int StateID { get; set; }
        public string Description { get; set; }
        public AttendanceStateDTO()
        {

        }

        public AttendanceStateDTO(AttendanceState inputAttendanceState)
        {
            StateID = inputAttendanceState.StateID;
            Description = inputAttendanceState.Description;
        }
    }

}

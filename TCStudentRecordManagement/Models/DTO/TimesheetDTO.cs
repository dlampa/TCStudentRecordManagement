using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCStudentRecordManagement.Models.DTO
{
    /// <summary>
    /// TimesheetDTO class is used to encapsulate mutable and immutable properties for Attendance objects (records). Used for transformation of
    /// records for addition and listing
    /// </summary>
    public class TimesheetDTO
    {
        public int RecordID { get; set; }

        public int StudentID { get; set; }

        public int AssignmentID { get; set; }

        public DateTime Date { get; set; }

        public decimal TimeAllocation { get; set; }

        public TimesheetDTO()
        {

        }

        public TimesheetDTO(Timesheet inputTimesheet)
        {
            RecordID = inputTimesheet.RecordID;
            StudentID = inputTimesheet.StudentID;
            AssignmentID = inputTimesheet.AssignmentID;
            Date = inputTimesheet.Date;
            TimeAllocation = inputTimesheet.TimeAllocation;
        }



    }
}

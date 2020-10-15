using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

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

    } // End of TimesheetDTO

    /// <summary>
    /// TimesheetGetDTO class is used to encapsulate all parameters specified for the GetTimesheetRecord API target
    /// </summary>
    public class TimesheetGetDTO
    {
        public int RecordID { get; set; }
        public int StudentID { get; set; }
        public int UnitID { get; set; }
        public int TypeID { get; set; }
        public int AssignmentID { get; set; }
        public int CohortID { get; set; }
        public DateTime Date { get; set; }

        // Hide property from JSON serializer
        // Ref: https://stackoverflow.com/a/55806177/12802214

        [JsonIgnore]
        public DateTime StartDate { get; set; }

        [JsonIgnore]
        public DateTime EndDate { get; set; }

        public decimal TimeAllocation { get; set; }

        public TimesheetGetDTO()
        {

        }

        public TimesheetGetDTO(Timesheet inputTimesheet)
        {
            RecordID = inputTimesheet.RecordID;
            StudentID = inputTimesheet.StudentID;
            UnitID = inputTimesheet?.AssignmentAlloc?.UnitID ?? 0;
            TypeID = inputTimesheet?.AssignmentAlloc?.TypeID ?? 0;
            AssignmentID = inputTimesheet.AssignmentID;
            CohortID = inputTimesheet?.ForStudent?.CohortID ?? 0;
            TimeAllocation = inputTimesheet.TimeAllocation;
            Date = inputTimesheet.Date;
        }

    }

}

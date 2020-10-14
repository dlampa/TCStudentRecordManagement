using System;

namespace TCStudentRecordManagement.Models.DTO
{
    /// <summary>
    /// AttendanceDTO class is used to encapsulate mutable and immutable properties for Attendance objects (records). Used for transformation of
    /// records for addition and listing
    /// </summary>
    public class AttendanceDTO
    {
        public int RecordID { get; set; }
        public int StudentID { get; set; }
        public int AttendanceStateID { get; set; }
        public int StaffID { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; }

        public AttendanceDTO()
        {

        }

        public AttendanceDTO(Attendance inputAttendance)
        {
            RecordID = inputAttendance.RecordID;
            StudentID = inputAttendance.StudentID;
            AttendanceStateID = inputAttendance.AttendanceStateID;
            StaffID = inputAttendance.StaffID;
            Date = inputAttendance.Date;
            Comment = inputAttendance.Comment;
        }

        

    }
    /// <summary>
    /// AttendanceModDTO class is used to encapsulate mutable properties for Attendance objects (records)
    /// </summary>
    public class AttendanceModDTO
    {
        public int RecordID { get; set; }
       
        public int AttendanceStateID { get; set; }
       
        public string Comment { get; set; }

        public AttendanceModDTO()
        {

        }
        
        public AttendanceModDTO(Attendance inputAttendance)
        {
            RecordID = inputAttendance.RecordID;         
            AttendanceStateID = inputAttendance.AttendanceStateID;
            Comment = inputAttendance.Comment;
        }
    }
}

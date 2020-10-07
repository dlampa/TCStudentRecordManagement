using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TCStudentRecordManagement.Models
{
    [Table("attendance")]
    public class Attendance
    {
        [Key]
        [Column("RecordID", TypeName = "int")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordID { get; set; }

        [Column("StudentID", TypeName = "int")]
        [Required]
        public int StudentID { get; set; }

        [Column("AttendanceStateID", TypeName = "int")]
        [Required]
        public int AttendanceStateID { get; set; }

        [Column("StaffID", TypeName = "int")]
        [Required]
        public int StaffID { get; set; }

        [Column("Date", TypeName = "datetime2")]
        [Required]
        public DateTime Date { get; set; }

        [ForeignKey(nameof(AttendanceStateID))]
        [InverseProperty(nameof(AttendanceState.AttendancesOfType))]
        public virtual AttendanceState AttendanceType { get; set; }

        [ForeignKey(nameof(StudentID))]
        [InverseProperty(nameof(Student.AttendanceRecord))]
        public virtual Student StudentDetails {get; set;}

        [ForeignKey(nameof(StaffID))]
        [InverseProperty(nameof(User.StaffAttendanceRecords))]
        public virtual User RecordedBy { get; set; }

        public Attendance()
        {

        }


    }
}

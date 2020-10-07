using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCStudentRecordManagement.Models
{
    [Table("students")]
    public class Student
    {
        [Key]
        [Column("StudentID", TypeName = "int")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentID { get; set; }

        [Column("UserID", TypeName = "int")]
        public int UserID { get; set; }

        [Column("CohortID", TypeName = "int")]
        [Required]
        public int CohortID { get; set; }

        [Column("BearTracksID", TypeName = "varchar(10)")]
        public string BearTracksID { get; set; }

        [ForeignKey(nameof(UserID))]
        public virtual User UserData { get; set; }

        [ForeignKey(nameof(CohortID))]
        [InverseProperty(nameof(Cohort.Students))]
        public virtual Cohort CohortMember { get; set; }

        [InverseProperty(nameof(Timesheet.ForStudent))]
        public virtual List<Timesheet> Timesheets { get; set; }

        [InverseProperty(nameof(Attendance.StudentDetails))]
        public virtual List<Attendance> AttendanceRecord { get; set; }

        public Student()
        {

        }

    }
}


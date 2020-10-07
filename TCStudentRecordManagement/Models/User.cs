using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCStudentRecordManagement.Models
{
    // Entity framework model for the Users table

    [Table("users")]
    public partial class User
    {
        [Key]
        [Column("UserID", TypeName = "int")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [Column("Firstname", TypeName = "nvarchar(50)")]
        [Required]
        public string Firstname { get; set; }

        [Column("Lastname", TypeName = "nvarchar(50)")]
        [Required]
        public string Lastname { get; set; }

        [Column("Email", TypeName = "varchar(320)")]
        [Required]
        public string Email { get; set; }

        [Column("Rights", TypeName = "tinyint")]
        [Required]
        public int Rights { get; set; }

        [InverseProperty(nameof(Notice.Staff))]
        public virtual List<Notice> Notices { get; set; }

        [InverseProperty(nameof(Student.UserData))]
        public virtual Student StudentData { get; set; }

        [InverseProperty(nameof(Attendance.RecordedBy))]
        public virtual List<Attendance> StaffAttendanceRecords { get; set; }

        public User()
        {
            Notices = new List<Notice>();
            StaffAttendanceRecords = new List<Attendance>();
        }


    }
}

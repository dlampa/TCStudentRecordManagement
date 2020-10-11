using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TCStudentRecordManagement.Models
{
    [Table("staff")]
    public class Staff
    {
        [Key]
        [Column("StaffID", TypeName = "int")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StaffID { get; set; }

        [Column("UserID", TypeName = "int")]
        public int UserID { get; set; }

        [Column("SuperUser", TypeName = "bit")]
        public bool SuperUser { get; set; }

        [InverseProperty(nameof(Notice.ByStaff))]
        public virtual List<Notice> Notices { get; set; }

        [ForeignKey(nameof(UserID))]
        [InverseProperty(nameof(User.StaffData))]
        public virtual User UserData { get; set; }

        [InverseProperty(nameof(Attendance.RecordedBy))]
        public virtual List<Attendance> AttendanceTaken { get; set; }

        public Staff()
        {
            Notices = new List<Notice>();
            AttendanceTaken = new List<Attendance>();
        }

    }
}

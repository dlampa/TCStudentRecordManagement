using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TCStudentRecordManagement.Models
{
    [Table("attendance_states")]
    public class AttendanceState
    {
        [Key]
        [Column("StateID", TypeName = "int")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StateID { get; set; }

        [Column("Description", TypeName = "varchar(50)")]
        [Required]
        public string Description { get; set; }

        [InverseProperty(nameof(Attendance.AttendanceType))]
        public virtual List<Attendance> AttendancesOfType { get; set; }

        public AttendanceState()
        {
            AttendancesOfType = new List<Attendance>();
        }

    }
}

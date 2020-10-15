using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCStudentRecordManagement.Models
{
    [Table("timesheets")]
    public class Timesheet
    {
        [Key]
        [Column("RecordID", TypeName = "int")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordID { get; set; }

        [Column("StudentID", TypeName = "int")]
        [Required]
        public int StudentID { get; set; }

        [Column("AssignmentID", TypeName = "int")]
        [Required]
        public int AssignmentID { get; set; }

        [Column("Date", TypeName = "date")]
        [Required]
        public DateTime Date { get; set; }

        // TODO: Introduce precision
        [Column("TimeAllocation", TypeName = "decimal(3,2)")]
        [Required]
        public decimal TimeAllocation { get; set; }

        [ForeignKey(nameof(StudentID))]
        [InverseProperty(nameof(Student.Timesheets))]
        public virtual Student ForStudent { get; set; }

        [ForeignKey(nameof(AssignmentID))]
        [InverseProperty(nameof(Task.Timesheets))]
        public virtual Task AssignmentAlloc { get; set; }


        public Timesheet()
        {

        }


    }

}

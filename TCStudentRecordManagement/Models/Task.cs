using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCStudentRecordManagement.Models
{
    [Table("tasks")]
    public class Task
    {
        [Key]
        [Column("TaskID", TypeName = "int")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskID { get; set; }

        [Column("UnitID", TypeName = "int")]
        [Required]
        public int UnitID { get; set; }

        [Column("TypeID", TypeName = "int")]
        [Required]
        public int TypeID { get; set; }

        [Column("CohortID", TypeName = "int")]
        [Required]
        public int CohortID { get; set; }

        [Column("Title", TypeName = "varchar(100)")]
        [Required]
        public string Title { get; set; }

        [Column("StartDate", TypeName = "datetime2")]
        [Required]
        public DateTime StartDate { get; set; }

        [Column("EndDate", TypeName = "datetime2")]
        [Required]
        public DateTime EndDate { get; set; }

        [Column("DocURL", TypeName = "varchar(255)")]
        public string DocURL { get; set; }

        [ForeignKey(nameof(UnitID))]
        [InverseProperty(nameof(Unit.Tasks))]
        public virtual Unit FromUnit { get; set; }

        [ForeignKey(nameof(TypeID))]
        [InverseProperty(nameof(TaskType.Tasks))]
        public virtual TaskType Type { get; set; }

        [ForeignKey(nameof(CohortID))]
        [InverseProperty(nameof(Cohort.Tasks))]
        public virtual Cohort AssignedCohort { get; set; }

        [InverseProperty(nameof(Timesheet.AssignmentAlloc))]
        public virtual List<Timesheet> Timesheets { get; set; }

        public Task()
        {
            Timesheets = new List<Timesheet>();
        }


    }
}

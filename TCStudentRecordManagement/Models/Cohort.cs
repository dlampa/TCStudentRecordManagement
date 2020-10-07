using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCStudentRecordManagement.Models
{
    [Table("cohorts")]
    public class Cohort
    {
        [Key]
        [Column("CohortID", TypeName = "int")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CohortID { get; set; }

        [Column("Name", TypeName = "nvarchar(50)")]
        [Required]
        public string Name { get; set; }

        [Column("StartDate", TypeName = "datetime2")]
        [Required]
        public DateTime StartDate { get; set; }

        [Column("EndDate", TypeName = "datetime2")]
        [Required]
        public DateTime EndDate { get; set; }

        [InverseProperty(nameof(Student.CohortMember))]
        public virtual List<Student> Students { get; set; }

        [InverseProperty(nameof(Assignment.AssignmentCohort))]
        public virtual List<Assignment> Assignments { get; set; }

        [InverseProperty(nameof(Notice.ForCohort))]
        public virtual List<Notice> Notices { get; set; }

        public Cohort()
        {
            Students = new List<Student>();
            Assignments = new List<Assignment>();
            Notices = new List<Notice>();
        }


    }
}

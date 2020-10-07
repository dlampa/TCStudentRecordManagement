using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCStudentRecordManagement.Models
{
    [Table("assignments")]
    public class Assignment
    {
        [Key]
        [Column("AssignmentID", TypeName = "int")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssignmentID { get; set; }

        [Column("TopicID", TypeName = "int")]
        [Required]
        public int TopicID { get; set; }

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

        [ForeignKey(nameof(TopicID))]
        [InverseProperty(nameof(Topic.Assignments))]
        public virtual Topic AssignmentTopic { get; set; }

        [ForeignKey(nameof(TypeID))]
        [InverseProperty(nameof(AssignmentType.Assignments))]
        public virtual AssignmentType Type { get; set; }

        [ForeignKey(nameof(CohortID))]
        [InverseProperty(nameof(Cohort.Assignments))]
        public virtual Cohort AssignmentCohort { get; set; }

        [InverseProperty(nameof(Timesheet.AssignmentAlloc))]
        public virtual List<Timesheet> Timesheets { get; set; }

        public Assignment()
        {
            Timesheets = new List<Timesheet>();
        }


    }
}

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

        public Cohort()
        {
            Students = new List<Student>();
        }


    }
}

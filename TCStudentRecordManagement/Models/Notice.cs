using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TCStudentRecordManagement.Models
{
    [Table("notices")]
    public class Notice
    {
        [Key]
        [Column("NoticeID", TypeName = "int")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NoticeID { get; set; }

        [Column("CohortID", TypeName = "int")]
        public int CohortID { get; set; }

        [Column("StaffID", TypeName = "int")]
        [Required]
        public int StaffID { get; set; }

        [Column("ValidFrom", TypeName = "datetime2")]
        [Required]
        public DateTime ValidFrom { get; set; }

        [Column("Markdown", TypeName = "nvarchar(max)")]
        public string Markdown { get; set; }

        [Column("HTML", TypeName = "nvarchar(max)")]
        public string HTML { get; set; }

        [ForeignKey(nameof(CohortID))]
        [InverseProperty(nameof(Cohort.Notices))]
        public virtual Cohort ForCohort { get; set; }

        [ForeignKey(nameof(StaffID))]
        [InverseProperty(nameof(Staff.Notices))]
        public virtual Staff ByStaff { get; set; }

        public Notice()
        {

        }
    }
}

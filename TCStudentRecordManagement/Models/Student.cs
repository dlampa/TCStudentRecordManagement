using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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
        public virtual User User { get; set; }

        [ForeignKey(nameof(CohortID))]
        [InverseProperty(nameof(Cohort.Students))]
        public virtual Cohort CohortMember { get; set; }

    

        public Student()
        {

        }

    }

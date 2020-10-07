using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCStudentRecordManagement.Models
{
    [Table("assignment_types")]
    public class AssignmentType
    {
        [Key]
        [Column("TypeID", TypeName = "int")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TypeID { get; set; }

        [Column("Description", TypeName = "varchar(50)")]
        [Required]
        public string Description { get; set; }

        [InverseProperty(nameof(Assignment.TypeID))]
        public virtual List<Assignment> Assignments { get; set; }

        public AssignmentType()
        {
            Assignments = new List<Assignment>();
        }


    }
}

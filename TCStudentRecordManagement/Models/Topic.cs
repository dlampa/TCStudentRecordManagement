using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCStudentRecordManagement.Models
{
    [Table("topics")]
    public class Topic
    {
        [Key]
        [Column("TopicID", TypeName = "int")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TopicID { get; set; }

        [Column("Description", TypeName = "varchar(50)")]
        [Required]
        public string Description { get; set; }

        [InverseProperty(nameof(Assignment.AssignmentTopic))]
        public virtual List<Assignment> Assignments { get; set; }

        public Topic()
        {
            Assignments = new List<Assignment>();
        }

    }
}


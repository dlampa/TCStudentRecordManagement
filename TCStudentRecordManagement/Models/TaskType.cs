using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCStudentRecordManagement.Models
{
    [Table("task_types")]
    public class TaskType
    {
        [Key]
        [Column("TypeID", TypeName = "int")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TypeID { get; set; }

        [Column("Description", TypeName = "varchar(50)")]
        [Required]
        public string Description { get; set; }

        [InverseProperty(nameof(Task.Type))]
        public virtual List<Task> Tasks { get; set; }

        public TaskType()
        {
            Tasks = new List<Task>();
        }


    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCStudentRecordManagement.Models
{
    [Table("units")]
    public class Unit
    {
        [Key]
        [Column("UnitID", TypeName = "int")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UnitID { get; set; }

        [Column("Description", TypeName = "varchar(50)")]
        [Required]
        public string Description { get; set; }

        [InverseProperty(nameof(Task.FromUnit))]
        public virtual List<Task> Tasks { get; set; }

        public Unit()
        {
            Tasks = new List<Task>();
        }

    }
}


using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCStudentRecordManagement.Models
{
    // Entity framework model for the Users table

    [Table("users")]
    public partial class User
    {
        [Key]
        [Column("UserID", TypeName = "int")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [Column("Firstname", TypeName = "nvarchar(50)")]
        [Required]
        public string Firstname { get; set; }

        [Column("Lastname", TypeName = "nvarchar(50)")]
        [Required]
        public string Lastname { get; set; }

        [Column("Email", TypeName = "varchar(320)")]
        [Required]
        public string Email { get; set; }

        [Column("Active", TypeName = "bit")]
        [Required]
        public bool Active { get; set; }

        [InverseProperty(nameof(Student.UserData))]
        public virtual Student StudentData { get; set; }

        [InverseProperty(nameof(Staff.UserData))]
        public virtual Staff StaffData{ get; set; }
       
        public User()
        {
 
        }


    }
}

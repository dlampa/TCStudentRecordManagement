using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.UserSecrets;

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

        [Column("Rights", TypeName = "tinyint")]
        [Required]
        public int Rights { get; set; }

        public User()
        {

        }


    }
}

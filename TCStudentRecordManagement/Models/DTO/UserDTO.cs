using TCStudentRecordManagement.Models;

namespace TCStudentRecordManagement.Models.DTO
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }

        public UserDTO()
        {

        }

        public UserDTO(User inputUser)
        {
            if (inputUser != null)
            {
                UserID = inputUser.UserID;
                Firstname = inputUser.Firstname;
                Lastname = inputUser.Lastname;
                Email = inputUser.Email;
                Active = inputUser.Active;

            }
        }

    }
}

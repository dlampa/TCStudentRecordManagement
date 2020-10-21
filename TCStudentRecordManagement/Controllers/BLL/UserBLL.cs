using System;
using System.Linq;
using TCStudentRecordManagement.Models;
using TCStudentRecordManagement.Controllers.Exceptions;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TCStudentRecordManagement.Models.DTO;

namespace TCStudentRecordManagement.Controllers.BLL
{

    internal class UserBLL
    {
        private readonly DataContext _context;

        internal UserBLL(DataContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Business logic for creating users in database
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="email"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        internal object AddUserBLL(string firstname, string lastname, string email, bool active)
        {
            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            // Email check regex in Dictionary below
            // Ref: https://stackoverflow.com/a/45177249/12802214

            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "firstname parameter cannot be empty.", firstname == null || firstname.Trim() == string.Empty },
                { "lastname parameter cannot be empty", lastname == null || lastname.Trim() == string.Empty },
                { "Email needs to be a valid email address", email == null || !Regex.IsMatch(email, @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$") },
                { "Email address needs to be unique in the database (user record already exists)", email != null && _context.Users.Any(x => x.Email == email) && _context.Students.Any(x => x.UserID == _context.Users.Where(y => y.Email == email).First().UserID) },
                { "Email address domain not found in the list of valid domains", email != null && !APIConfig.Configuration.GetSection("ValidEmailDomains").GetChildren().ToArray().Select(x => x.Value).ToArray().Contains(email.Split("@")?[1]??"") }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                // New database names will be trimmed and in Proper Case.
                string dbFirstname = String.Join(" ", firstname.Trim().Split(" ").Select(x => $"{x.Substring(0, 1).ToUpper()}{x.Substring(1)?.ToLower()}").ToArray());
                string dbLastname = String.Join(" ", lastname.Trim().Split(" ").Select(x => $"{x.Substring(0, 1).ToUpper()}{x.Substring(1)?.ToLower()}").ToArray());

                UserDTO newUser = new UserDTO
                {
                    Firstname = dbFirstname,
                    Lastname = dbLastname,
                    Email = email,
                    Active = active
                };


                return newUser;
            }
            else
            {
                return exceptionList;
            }

        } // End of AddUserBLL

        /// <summary>
        /// Business logic for modifying User records
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        internal object ModifyUserBLL(UserDTO user)
        {
            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            // Email check regex in Dictionary below
            // Ref: https://stackoverflow.com/a/45177249/12802214

            // Check is virtually identical to that of the AddStudent method, except for the e-mail address unique check
            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "firstname parameter cannot be empty.", user.Firstname == null || user.Firstname.Trim() == string.Empty },
                { "lastname parameter cannot be empty", user.Lastname == null || user.Lastname.Trim() == string.Empty },
                { "Email needs to be a valid email address", user.Email == null || !Regex.IsMatch(user.Email, @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$") },
                { "Email address needs to be unique in the database (student record already exists)", user.Email != null && !(_context.Users.Any(x => x.Email == user.Email && x.UserID != _context.Users.Where(y => y.Email == user.Email).First().UserID)) },
                { "Email address domain not found in the list of valid domains", user.Email != null && !APIConfig.Configuration.GetSection("ValidEmailDomains").GetChildren().ToArray().Select(x => x.Value).ToArray().Contains(user.Email.Split("@")?[1]??"") }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                // New database names will be trimmed and in Proper Case.
                string dbFirstname = String.Join(" ", user.Firstname.Trim().Split(" ").Select(x => $"{x.Substring(0, 1).ToUpper()}{x.Substring(1)?.ToLower()}").ToArray());
                string dbLastname = String.Join(" ", user.Lastname.Trim().Split(" ").Select(x => $"{x.Substring(0, 1).ToUpper()}{x.Substring(1)?.ToLower()}").ToArray());

                // Create a dictionary containing all data needed to create the user and pass back to API Target.
                UserDTO modifiedDbObject = new UserDTO { UserID = user.UserID, Firstname = dbFirstname, Lastname = dbLastname, Email = user.Email, Active = user.Active};
                return modifiedDbObject;
            }
            else
            {
                return exceptionList;
            }

        } // End of ModifyUser

    } // End of UserBLL


}
    

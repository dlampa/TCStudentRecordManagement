using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TCStudentRecordManagement.Models.DTO;
using TCStudentRecordManagement.Controllers.Exceptions;
using TCStudentRecordManagement.Models;

namespace TCStudentRecordManagement.Controllers.BLL
{
    internal class StudentBLL
    {
        private readonly DataContext _context;

        public StudentBLL(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Business logic for listing Student records
        /// </summary>
        /// <param name="cohortID"></param>
        /// <returns></returns>
        internal object StudentDetailBLL(int cohortID)
        {
            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "Specified CohortID does not exist", !(_context.Cohorts.Any(x => x.CohortID == cohortID) || cohortID == 0) },
                { "Specified cohort is inactive.", cohortID != 0 && !_context.Cohorts.Any(x => x.CohortID == cohortID && DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                return cohortID;
            }
            else
            {
                return exceptionList;
            }
        } // End of StudentDetailBLL

        /// <summary>
        /// Business logic for adding Student records
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="email"></param>
        /// <param name="active"></param>
        /// <param name="cohortID"></param>
        /// <param name="bearTracksID"></param>
        /// <param name="userIsSuperAdmin"></param>
        /// <returns></returns>
        internal object AddStudent(string firstname, string lastname, string email, bool active, int cohortID, string bearTracksID, bool userIsSuperAdmin)
        {
            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            // Email check regex in Dictionary below
            // Ref: https://stackoverflow.com/a/45177249/12802214

            // Due to the number of checks, this approach is more appropriate
            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "firstname parameter cannot be empty.", firstname == null || firstname.Trim() == string.Empty },
                { "lastname parameter cannot be empty", lastname == null || lastname.Trim() == string.Empty },
                { "Email needs to be a valid email address", email == null || !Regex.IsMatch(email, @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$") },
                { "Email address needs to be unique in the database (student record already exists)", email != null && _context.Users.Any(x => x.Email == email) && _context.Students.Any(x => x.UserID == _context.Users.Where(y => y.Email == email).First().UserID) },
                { "Email address domain not found in the list of valid domains", email != null && !APIConfig.Configuration.GetSection("ValidEmailDomains").GetChildren().ToArray().Select(x => x.Value).ToArray().Contains(email.Split("@")?[1]??"") },
                { "Specified CohortID does not exist", !_context.Cohorts.Any(x => x.CohortID == cohortID) },
                { "Specified cohort is inactive.", !(_context.Cohorts.Any(x => x.CohortID == cohortID && DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate) || userIsSuperAdmin) }
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

                // Create a dictionary containing all data needed to create the user and pass back to API Target
                Dictionary<string, object> newDbObject = new Dictionary<string, object>()
                {
                    { "Firstname", dbFirstname },
                    { "Lastname", dbLastname },
                    { "Email", email },
                    { "Active", active },
                    { "CohortID", cohortID },
                    { "BearTracksID", bearTracksID },
                    { "UserAlreadyInUsersTable", _context.Users.Any(x => x.Email == email) }
                };

                return newDbObject;
            }
            else
            {
                return exceptionList;
            }

        } // End of AddStudent

        /// <summary>
        /// Business logic for modifying Student records
        /// </summary>
        /// <param name="student"></param>
        /// <param name="userIsSuperAdmin"></param>
        /// <returns></returns>
        internal object ModifyStudent(StudentModDTO student, bool userIsSuperAdmin)
        {
            // Create a new APIException object to store possible exceptions as checks are performed. 
            APIException exceptionList = new APIException();

            // Email check regex in Dictionary below
            // Ref: https://stackoverflow.com/a/45177249/12802214

            // Check is virtually identical to that of the AddStudent method, except for the e-mail address unique check
            Dictionary<string, bool> exceptionTests = new Dictionary<string, bool>()
            {
                { "firstname parameter cannot be empty.", student.Firstname == null || student.Firstname.Trim() == string.Empty },
                { "lastname parameter cannot be empty", student.Lastname == null || student.Lastname.Trim() == string.Empty },
                { "Email needs to be a valid email address", student.Email == null || !Regex.IsMatch(student.Email, @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$") },
                { "Email address needs to be unique in the database (student record already exists)", student.Email != null && _context.Users.Any(x => x.Email == student.Email) && _context.Students.Any(x => x.UserID != _context.Users.Where(y => y.Email == student.Email).First().UserID) },
                { "Email address domain not found in the list of valid domains", student.Email != null && !APIConfig.Configuration.GetSection("ValidEmailDomains").GetChildren().ToArray().Select(x => x.Value).ToArray().Contains(student.Email.Split("@")?[1]??"") },
                { "Specified CohortID does not exist", !_context.Cohorts.Any(x => x.CohortID == student.CohortID) },
                { "Specified cohort is inactive.", !(_context.Cohorts.Any(x => x.CohortID == student.CohortID && DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate) || userIsSuperAdmin) }
            };

            foreach (KeyValuePair<string, bool> kvp in exceptionTests)
            {
                if (kvp.Value) exceptionList.AddExMessage(kvp.Key);
            }

            if (!exceptionList.HasExceptions)
            {
                // New database names will be trimmed and in Proper Case.
                string dbFirstname = String.Join(" ", student.Firstname.Trim().Split(" ").Select(x => $"{x.Substring(0, 1).ToUpper()}{x.Substring(1)?.ToLower()}").ToArray());
                string dbLastname = String.Join(" ", student.Lastname.Trim().Split(" ").Select(x => $"{x.Substring(0, 1).ToUpper()}{x.Substring(1)?.ToLower()}").ToArray());

                // Create a dictionary containing all data needed to create the user and pass back to API Target.
                StudentModDTO modifiedDbObject = new StudentModDTO { StudentID = student.StudentID, Firstname = dbFirstname, Lastname = dbLastname, Email = student.Email, Active = student.Active, CohortID = student.CohortID, BearTracksID = student.BearTracksID };
                return modifiedDbObject;
            }
            else
            {
                return exceptionList;
            }

        } // End of ModifyStudent

    } // End of StudentBLL
}

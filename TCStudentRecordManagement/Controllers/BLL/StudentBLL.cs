using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCStudentRecordManagement.Controllers.Exceptions;
using TCStudentRecordManagement.Models;

namespace TCStudentRecordManagement.Controllers.BLL
{
    public class StudentBLL
    {
        private readonly DataContext _context;

        public StudentBLL(DataContext context)
        {
            _context = context;
        }

        public object AddStudent(string firstname, string lastname, string email, bool active, int cohortID, string bearTracksID, bool userIsSuperAdmin)
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
                { "Email adddress needs to be unique in the database", email != null && _context.Users.Any(x => x.Email == email) },
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

                // Create an anonymous object containing all data needed to create the user and pass back to API Target
                object newDbObject = new { Firstname = dbFirstname, Lastname = dbLastname, Email = email, Active = active, CohortID = cohortID, BearTracksID = bearTracksID };

                return newDbObject;
            }
            else
            {
                return exceptionList;
            }

        } // End of AddStudent
    }
}
